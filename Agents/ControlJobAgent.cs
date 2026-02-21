using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaferMeasurementFlow.Models;
using System.Linq;
using WaferMeasurementFlow.Managers;

namespace WaferMeasurementFlow.Agents
{
    public class ControlJobAgent
    {
        // SEMI Limits
        private const int MaxPJQueueSize = 20;
        private const int MaxCJQueueSize = 10;

        private readonly RobotAgent _robotAgent;
        private readonly Dictionary<int, LoadPortAgent> _loadPorts;
        private readonly AlignerAgent _alignerAgent;
        private readonly ProcessManager _processManager;
        private readonly WaferManager _waferManager;
        private readonly RecipeManager _recipeManager;

        // SECS SDK 雙向同步
        private SecsManager? _secsManager;

        private readonly List<ProcessJob> _processJobs = new List<ProcessJob>();
        private readonly List<ControlJob> _controlJobs = new List<ControlJob>();

        public IReadOnlyList<ProcessJob> ProcessJobs => _processJobs;
        public IReadOnlyList<ControlJob> ControlJobs => _controlJobs;

        public ControlJobAgent(RobotAgent robotAgent, Dictionary<int, LoadPortAgent> loadPorts, AlignerAgent alignerAgent, ProcessManager processManager, WaferManager waferManager, RecipeManager recipeManager)
        {
            _robotAgent = robotAgent;
            _loadPorts = loadPorts;
            _alignerAgent = alignerAgent;
            _processManager = processManager;
            _waferManager = waferManager;
            _recipeManager = recipeManager;

            SystemEventBus.LoadPortStateChanged += OnLoadPortStateChanged;
        }

        /// <summary>
        /// 綁定 SecsManager（由 Equipment 或 SecsManager 呼叫），啟用雙向同步
        /// </summary>
        public void SetSecsManager(SecsManager secsManager)
        {
            _secsManager = secsManager;
            SystemEventBus.PublishLog("ControlJobAgent: SecsManager 已綁定，本地 PJ/CJ 變更將同步到 SDK。");
        }

        /// <summary>
        /// 同步 PJ 到 SDK（容錯，失敗不阻斷流程）
        /// </summary>
        private void SyncPJToSDK(ProcessJob pj)
        {
            if (_secsManager == null) return;
            try
            {
                byte[] slot = new byte[25]; // 預設全 slot
                _secsManager.Gem300.CreateProcessJob(pj.Id, pj.Recipe.Id, pj.PRProcessStart, pj.CarrierID, slot, out var err);
                if (!string.IsNullOrEmpty(err))
                    SystemEventBus.PublishLog($"[SDK Sync] PJ '{pj.Id}' sync warning: {err}");
            }
            catch (Exception ex)
            {
                SystemEventBus.PublishLog($"[SDK Sync] PJ '{pj.Id}' sync failed: {ex.Message}");
            }
        }

        /// <summary>
        /// 同步 CJ 到 SDK
        /// </summary>
        private void SyncCJToSDK(ControlJob cj)
        {
            if (_secsManager == null) return;
            try
            {
                var carrierList = new List<string> { cj.CarrierInputSpec };
                var pjIds = cj.ProcessJobs.Select(p => p.Id).ToArray();
                byte prOrder = (byte)cj.ProcessOrderMgmt;
                _secsManager.Gem300.CreateControlJobInSDK(cj.Id, carrierList, pjIds, prOrder, true, out var err);
                if (!string.IsNullOrEmpty(err))
                    SystemEventBus.PublishLog($"[SDK Sync] CJ '{cj.Id}' sync warning: {err}");
            }
            catch (Exception ex)
            {
                SystemEventBus.PublishLog($"[SDK Sync] CJ '{cj.Id}' sync failed: {ex.Message}");
            }
        }

        /// <summary>
        /// 同步 PJ 狀態到 SDK
        /// </summary>
        private void SyncPJStateToSDK(string pjId, ProcessJobState state)
        {
            if (_secsManager == null) return;
            try { _secsManager.Gem300.ChangeProcessJobState(pjId, (byte)state); }
            catch { }
        }

        /// <summary>
        /// 同步 CJ 狀態到 SDK
        /// </summary>
        private void SyncCJStateToSDK(string cjId, ControlJobState state)
        {
            if (_secsManager == null) return;
            try { _secsManager.Gem300.ChangeControlJobState(cjId, (byte)state, 0); }
            catch { }
        }

        // =============================================
        // Step 1: SEMI E40 - PRJobCreate
        // =============================================
        public ProcessJob CreateProcessJob(string pjId, string recipeId, string carrierID, List<string>? prMtlNameList = null, bool prProcessStart = true, bool fromRemote = false)
        {
            // 2. 隊列空間檢查
            if (_processJobs.Count(p => p.State != ProcessJobState.PROCESS_COMPLETE) >= MaxPJQueueSize)
                throw new Exception("E40 Error: Process Job Queue is full (Max: " + MaxPJQueueSize + "). Please wait for jobs to complete.");

            // 2. ID 唯一性
            if (_processJobs.Any(p => p.Id == pjId))
                throw new Exception($"E40 Error: PRJobID '{pjId}' already exists.");

            var recipe = _recipeManager.GetRecipe(recipeId) ?? new Recipe(recipeId);
            var pj = new ProcessJob(pjId, recipe);
            pj.CarrierID = carrierID;
            pj.PRProcessStart = prProcessStart;
            pj.PRMtlNameList = prMtlNameList ?? new List<string>();
            pj.State = ProcessJobState.POOLED;

            _processJobs.Add(pj);
            SystemEventBus.PublishLog($"E40: PRJobCreate → PJ '{pjId}' Created. State: POOLED. (RecID: {recipeId}, CarrierID: {carrierID}, PRProcessStart: {prProcessStart}, Materials: {pj.PRMtlNameList.Count})");

            // 雙向同步到 SDK（Remote 來的不需要再寫回 SDK，物件已存在）
            if (!fromRemote)
                SyncPJToSDK(pj);
            SystemEventBus.PublishJobChanged();

            return pj;
        }

        // =============================================
        // Step 2: SEMI E94 - Object Create (CJ)
        // =============================================
        public ControlJob CreateControlJob(string cjId, List<string> processingCtrlSpec, string carrierInputSpec, string mtrlOutSpec = "", ProcessOrderMgmt processOrderMgmt = ProcessOrderMgmt.LIST, int targetPortId = 0, bool fromRemote = false)
        {
            if (processingCtrlSpec == null || processingCtrlSpec.Count == 0)
                throw new Exception("E94 Error: ProcessingCtrlSpec must contain at least one PRJobID.");

            // 2. 隊列空間檢查
            if (_controlJobs.Count(c => c.State != ControlJobState.COMPLETED && c.State != ControlJobState.ABORTED) >= MaxCJQueueSize)
                throw new Exception("E94 Error: Control Job Queue is full (Max: " + MaxCJQueueSize + ").");

            // 2. ID 唯一性
            if (_controlJobs.Any(c => c.Id == cjId))
                throw new Exception($"E94 Error: ControlJob '{cjId}' already exists.");

            // 1. 作業關聯性限制：確認 PJ 存在且為 POOLED
            var resolvedPJs = new List<ProcessJob>();
            foreach (var pjId in processingCtrlSpec)
            {
                var pj = _processJobs.FirstOrDefault(p => p.Id == pjId);
                if (pj == null)
                    throw new Exception($"E94 Error: PRJobID '{pjId}' does not exist. PJ must be created first.");
                
                // 1. PJ 狀態必須是 POOLED (尚未被其他 CJ 佔用或執行中)
                if (pj.State != ProcessJobState.POOLED)
                    throw new Exception($"E94 Error: PRJobID '{pjId}' is not in POOLED state (current: {pj.State}). It might be assigned to another CJ.");
                
                resolvedPJs.Add(pj);
            }

            var cj = new ControlJob(cjId);
            cj.CarrierInputSpec = carrierInputSpec;
            cj.MtrlOutSpec = mtrlOutSpec;
            cj.ProcessOrderMgmt = processOrderMgmt;
            cj.TargetPortId = targetPortId;
            cj.ProcessJobs.AddRange(resolvedPJs);
            cj.State = ControlJobState.QUEUED;

            // 1. 功能取代：PJ 狀態轉變為 SETTING_UP (或留在 POOLED 但標記為被 CJ 引用)
            foreach (var pj in resolvedPJs)
            {
                pj.State = ProcessJobState.SETTING_UP;
                if (!fromRemote)
                    SyncPJStateToSDK(pj.Id, ProcessJobState.SETTING_UP);
            }

            _controlJobs.Add(cj);
            SystemEventBus.PublishLog($"E94: ControlJob '{cjId}' Created. State: QUEUED. (ProcessingCtrlSpec: [{string.Join(", ", processingCtrlSpec)}], CarrierInputSpec: {carrierInputSpec})");

            // 雙向同步到 SDK（Remote 來的不需要再寫回 SDK，物件已存在）
            if (!fromRemote)
                SyncCJToSDK(cj);
            SystemEventBus.PublishJobChanged();

            return cj;
        }

        // =============================================
        // Execution
        // =============================================
        public async Task ExecuteControlJob(ControlJob controlJob)
        {
            if (controlJob.State != ControlJobState.QUEUED)
                SystemEventBus.PublishLog($"Warning: Executing CJ '{controlJob.Id}' from state {controlJob.State}.");

            SystemEventBus.PublishLog($"ControlJob '{controlJob.Id}': Starting execution. (CarrierInputSpec: {controlJob.CarrierInputSpec})");
            controlJob.State = ControlJobState.EXECUTING;
            SyncCJStateToSDK(controlJob.Id, ControlJobState.EXECUTING);
            SystemEventBus.PublishJobChanged();

            foreach (var processJob in controlJob.ProcessJobs)
            {
                // 若 PJ 已被 Abort，跳過
                if (processJob.State == ProcessJobState.ABORTING || processJob.State == ProcessJobState.STOPPING) continue;

                processJob.State = ProcessJobState.PROCESSING;
                SyncPJStateToSDK(processJob.Id, ProcessJobState.PROCESSING);
                // _secsManager?.SetControlJobCurrentPrJob(controlJob.Id, processJob.Id);

                await ExecuteProcessJob(controlJob, processJob);

                processJob.State = ProcessJobState.PROCESS_COMPLETE;
                SyncPJStateToSDK(processJob.Id, ProcessJobState.PROCESS_COMPLETE);
                SystemEventBus.PublishJobChanged();
            }

            // 檢查是否所有 PJ 都完成還是被 Abort
            if (controlJob.State != ControlJobState.ABORTED)
            {
                controlJob.State = ControlJobState.COMPLETED;
                SyncCJStateToSDK(controlJob.Id, ControlJobState.COMPLETED);
                SystemEventBus.PublishLog($"ControlJob '{controlJob.Id}': Execution finished.");

                if (_loadPorts.TryGetValue(controlJob.TargetPortId, out var completedPort))
                {
                    completedPort.LoadPort.State = LoadPortState.READY_TO_UNLOAD;
                    SystemEventBus.PublishLog($"LoadPort {controlJob.TargetPortId}: 加工完成，載具可卸載。");
                    SystemEventBus.Publish(completedPort.LoadPort);
                }
            }
        }

        /// <summary>
        /// 解析晶圓並驗證材料一致性
        /// </summary>
        public void ResolveWafersForCJ(ControlJob cj, LoadPortAgent portAgent)
        {
            if (portAgent.Carrier == null) return;
            cj.TargetPortId = portAgent.LoadPort.Id;

            foreach (var pj in cj.ProcessJobs)
            {
                if (pj.SubstratesToProcess.Count > 0) continue; // 已解析過

                // 5. 材料追蹤一致性：如果 PJ 指定了 PRMtlNameList (Wafer ID)，則必須與 Carrier 內容比對
                if (pj.PRMtlNameList != null && pj.PRMtlNameList.Count > 0)
                {
                    foreach (var waferId in pj.PRMtlNameList)
                    {
                        var substrate = portAgent.Carrier.SlotMap.Values.FirstOrDefault(s => s.Id == waferId);
                        if (substrate == null)
                        {
                            throw new Exception($"Material Error: Specified Wafer ID '{waferId}' in PJ '{pj.Id}' was not found in Carrier '{portAgent.Carrier.Id}'.");
                        }
                        pj.SubstratesToProcess.Add(substrate);
                    }
                    SystemEventBus.PublishLog($"PJ '{pj.Id}': 材料一致性驗證通過，匹配 {pj.SubstratesToProcess.Count} 片晶圓。");
                }
                else
                {
                    // 若無指定 ID，則依 Recipe Target Slots 解析
                    var recipe = _recipeManager.GetRecipe(pj.Recipe.Id);
                    HashSet<int>? targetSlots = null;
                    if (recipe != null && !string.IsNullOrWhiteSpace(recipe.TargetSlots))
                        targetSlots = ParseTargetSlots(recipe.TargetSlots);

                    foreach (var substrate in portAgent.Carrier.SlotMap.Values.OrderBy(s => s.Slot))
                    {
                        if (targetSlots == null || targetSlots.Contains(substrate.Slot))
                            pj.SubstratesToProcess.Add(substrate);
                    }
                    SystemEventBus.PublishLog($"PJ '{pj.Id}': 依 Slot 解析出 {pj.SubstratesToProcess.Count} 片晶圓。");
                }
            }
        }

        // =============================================
        // 4. 異常處理與材料風險
        // =============================================

        /// <summary>
        /// CJAbort / PRJobAbort: 強制立即停止，材料可能處於未知狀態
        /// </summary>
        public void AbortControlJob(string cjId)
        {
            var cj = _controlJobs.FirstOrDefault(c => c.Id == cjId);
            if (cj == null) { SystemEventBus.PublishLog($"E94 Error: CJ '{cjId}' not found."); return; }

            SystemEventBus.PublishLog($"E94: CJAbort → CJ '{cjId}' (Immediate Stop). WARNING: 材料可能處於未知狀態！");
            cj.State = ControlJobState.ABORTED;
            foreach (var pj in cj.ProcessJobs)
            {
                if (pj.State == ProcessJobState.PROCESSING)
                    pj.State = ProcessJobState.ABORTING;
                else if (pj.State != ProcessJobState.PROCESS_COMPLETE)
                    pj.State = ProcessJobState.ABORTING;
            }
            // TODO: 觸發硬體停止 (Robot Emergency Stop 等)
        }

        /// <summary>
        /// Cancel: 僅適用於 QUEUED 狀態的 CJ (尚未啟動)
        /// </summary>
        public void CancelControlJob(string cjId)
        {
            var cj = _controlJobs.FirstOrDefault(c => c.Id == cjId);
            if (cj == null) { SystemEventBus.PublishLog($"E94 Error: CJ '{cjId}' not found."); return; }

            if (cj.State != ControlJobState.QUEUED)
            {
                SystemEventBus.PublishLog($"E94 Error: Cannot Cancel CJ '{cjId}' — not QUEUED (Current: {cj.State}). Use Stop or Abort.");
                return;
            }

            SystemEventBus.PublishLog($"E94: Cancel CJ '{cjId}'. Removed from queue.");
            cj.State = ControlJobState.ABORTED;
            // 將 PJ 釋放回 POOLED 或標記為 STOPPING
            foreach (var pj in cj.ProcessJobs)
            {
                pj.State = ProcessJobState.POOLED; // 回收回池中供重複使用
            }
        }

        /// <summary>
        /// Stop: 有序停止，完成當前正在處理的 PJ 後停止，保護材料完整性
        /// </summary>
        public void StopControlJob(string cjId)
        {
            var cj = _controlJobs.FirstOrDefault(c => c.Id == cjId);
            if (cj == null) { SystemEventBus.PublishLog($"E94 Error: CJ '{cjId}' not found."); return; }

            if (cj.State != ControlJobState.EXECUTING)
            {
                SystemEventBus.PublishLog($"E94 Error: Cannot Stop CJ '{cjId}' — not EXECUTING (Current: {cj.State}). Use Cancel for QUEUED jobs.");
                return;
            }

            SystemEventBus.PublishLog($"E94: Stop CJ '{cjId}' — 完成當前 PJ 後停止 (保護材料)。");
            // 將尚未開始的 PJ 標記為 STOPPING，讓 ExecuteControlJob 迴圈跳過它們
            foreach (var pj in cj.ProcessJobs)
            {
                if (pj.State == ProcessJobState.SETTING_UP || pj.State == ProcessJobState.WAITING_FOR_START)
                    pj.State = ProcessJobState.STOPPING;
            }
        }

        // =============================================
        // 2. 隊列空間查詢 (PRGetSpace / QueueAvailableSpace)
        // =============================================

        /// <summary>E40: PRGetSpace — 查詢 PJ 隊列剩餘空間</summary>
        public int GetPJQueueSpace() => MaxPJQueueSize - _processJobs.Count(p => p.State != ProcessJobState.PROCESS_COMPLETE);

        /// <summary>E94: QueueAvailableSpace — 查詢 CJ 隊列剩餘空間</summary>
        public int GetCJQueueSpace() => MaxCJQueueSize - _controlJobs.Count(c => c.State != ControlJobState.COMPLETED && c.State != ControlJobState.ABORTED);

        // =============================================
        // 3. 修改作業的時機
        // =============================================

        /// <summary>
        /// 修改 CJ 屬性。
        /// 狀態限制：只有不處於 EXECUTING 或 COMPLETED 時才可修改。
        /// PAUSED 時允許修改尚未開始的 PJ 參數。
        /// </summary>
        public void ModifyControlJob(string cjId, string? newCarrierInputSpec = null, string? newMtrlOutSpec = null, ProcessOrderMgmt? newProcessOrder = null)
        {
            var cj = _controlJobs.FirstOrDefault(c => c.Id == cjId);
            if (cj == null) { SystemEventBus.PublishLog($"E94 Error: CJ '{cjId}' not found."); return; }

            // 3. 狀態限制
            if (cj.State == ControlJobState.EXECUTING || cj.State == ControlJobState.COMPLETED || cj.State == ControlJobState.ABORTED)
            {
                SystemEventBus.PublishLog($"E94 Error: Cannot modify CJ '{cjId}' in {cj.State} state.");
                return;
            }

            if (newCarrierInputSpec != null) cj.CarrierInputSpec = newCarrierInputSpec;
            if (newMtrlOutSpec != null) cj.MtrlOutSpec = newMtrlOutSpec;
            if (newProcessOrder.HasValue) cj.ProcessOrderMgmt = newProcessOrder.Value;

            SystemEventBus.PublishLog($"E94: CJ '{cjId}' modified. (CarrierInputSpec: {cj.CarrierInputSpec}, MtrlOutSpec: {cj.MtrlOutSpec}, ProcessOrder: {cj.ProcessOrderMgmt})");
        }

        /// <summary>
        /// Carrier 到位時自動比對 CarrierInputSpec
        /// </summary>
        private async void OnLoadPortStateChanged(LoadPort port)
        {
            if (port.State != LoadPortState.READY_TO_PROCESS) return;
            if (!_loadPorts.TryGetValue(port.Id, out var portAgent) || portAgent.Carrier == null) return;

            string arrivedCarrierID = portAgent.Carrier.Id;

            var matchedCJs = _controlJobs
                .Where(cj => cj.State == ControlJobState.QUEUED
                    && !string.IsNullOrEmpty(cj.CarrierInputSpec)
                    && cj.CarrierInputSpec == arrivedCarrierID)
                .ToList();

            if (matchedCJs.Count == 0)
            {
                SystemEventBus.PublishLog($"Port {port.Id}: Carrier '{arrivedCarrierID}' 就緒，無匹配的待執行工單。");
                return;
            }

            foreach (var cj in matchedCJs)
            {
                try
                {
                    SystemEventBus.PublishLog($"自動比對成功: Carrier '{arrivedCarrierID}' (Port {port.Id}) → CJ '{cj.Id}'");
                    ResolveWafersForCJ(cj, portAgent);
                    await ExecuteControlJob(cj);
                }
                catch (Exception ex)
                {
                    SystemEventBus.PublishLog($"Error: 自動執行 CJ '{cj.Id}' 失敗: {ex.Message}");
                    cj.State = ControlJobState.ABORTED;
                }
            }
        }

        private HashSet<int>? ParseTargetSlots(string slotsStr)
        {
            var slots = new HashSet<int>();
            if (slotsStr.Trim().ToUpper() == "ALL") return null;
            var parts = slotsStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (int.TryParse(part.Trim(), out int slot))
                    slots.Add(slot);
                else if (part.Contains("-"))
                {
                    var range = part.Split('-');
                    if (range.Length == 2 && int.TryParse(range[0], out int start) && int.TryParse(range[1], out int end))
                        for (int i = start; i <= end; i++) slots.Add(i);
                }
            }
            return slots;
        }

        private async Task ExecuteProcessJob(ControlJob cj, ProcessJob pj)
        {
            SystemEventBus.PublishLog($"CJ '{cj.Id}' -> PJ '{pj.Id}': Starting (Recipe: {pj.Recipe.Id}, CarrierID: {pj.CarrierID})");

            foreach (var substrate in pj.SubstratesToProcess)
            {
                if (cj.State == ControlJobState.ABORTED) return; // 若 CJ 被 Abort，立即停止

                _waferManager.RegisterWafer(substrate);
                LoadPortAgent? targetPortAgent = null;
                Substrate? substrateInCarrier = null;

                foreach (var portAgent in _loadPorts.Values)
                {
                    if (portAgent.Carrier != null && portAgent.Carrier.SlotMap.ContainsKey(substrate.Slot))
                    {
                        var s = portAgent.Carrier.SlotMap[substrate.Slot];
                        if (s.Id == substrate.Id) { targetPortAgent = portAgent; substrateInCarrier = s; break; }
                    }
                }

                if (targetPortAgent != null && substrateInCarrier != null)
                {
                    _waferManager.UpdateWaferState(substrate.Id, SubstrateState.PRESENT);
                    await _robotAgent.MoveSubstrate(targetPortAgent, substrate.Slot, "Aligner");
                    await _alignerAgent.AlignSubstrate(substrateInCarrier);
                    await _robotAgent.Transfer("Aligner", "ProcessChamber1", substrateInCarrier);
                    await _processManager.ProcessSubstrate(substrateInCarrier);
                    _waferManager.UpdateWaferState(substrate.Id, SubstrateState.PROCESSED);
                    await _robotAgent.MoveSubstrate_Return(targetPortAgent, substrate.Slot, "ProcessChamber1", substrateInCarrier);
                    _waferManager.UpdateWaferState(substrate.Id, SubstrateState.PRESENT);
                }
                else
                {
                    SystemEventBus.PublishLog($"Error: Could not find substrate '{substrate.Id}' (Slot {substrate.Slot}) in any loaded carrier.");
                }
            }
            SystemEventBus.PublishLog($"CJ '{cj.Id}' -> PJ '{pj.Id}': Completed.");
        }
    }

    public partial class RobotAgent
    {
        public async Task MoveSubstrate_Return(LoadPortAgent toPortAgent, int toSlot, string fromLocation, Substrate substrate)
        {
            var toPort = toPortAgent.LoadPort;
            if (State != RobotState.IDLE) { SystemEventBus.PublishLog("Error: Robot is busy."); return; }

            State = RobotState.MOVING_TO_SOURCE;
            SystemEventBus.PublishLog($"Robot: Moving to {fromLocation}.");
            await Task.Delay(500);

            State = RobotState.PICKING;
            SystemEventBus.PublishLog($"Robot: Picking '{substrate.Id}' from {fromLocation}.");
            substrate.State = SubstrateState.PICKED;
            SystemEventBus.Publish(substrate);
            await Task.Delay(500);

            State = RobotState.MOVING_TO_DEST;
            SystemEventBus.PublishLog($"Robot: Moving '{substrate.Id}' to LoadPort {toPort.Id}.");
            await Task.Delay(500);

            State = RobotState.PUTTING;
            SystemEventBus.PublishLog($"Robot: Placing '{substrate.Id}' into Slot {toSlot}.");
            substrate.State = SubstrateState.PRESENT;
            SystemEventBus.Publish(substrate);
            if (toPort.Carrier != null) { toPort.Carrier.SlotMap[toSlot] = substrate; SystemEventBus.Publish(toPort.Carrier); }
            await Task.Delay(500);

            State = RobotState.MOVING_HOME;
            await Task.Delay(500);
            State = RobotState.IDLE;
            SystemEventBus.PublishLog("Robot: Idle.");
        }
    }
}