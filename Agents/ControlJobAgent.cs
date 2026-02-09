using System.Threading.Tasks;
using WaferMeasurementFlow.Models;
using System.Linq;
using WaferMeasurementFlow.Managers;

namespace WaferMeasurementFlow.Agents
{
    public class ControlJobAgent
    {
        private readonly RobotAgent _robotAgent;
        private readonly Dictionary<int, LoadPortAgent> _loadPorts; // Updated to Dictionary
        private readonly AlignerAgent _alignerAgent;
        private readonly ProcessManager _processManager;
        private readonly WaferManager _waferManager;

        public ControlJobAgent(RobotAgent robotAgent, Dictionary<int, LoadPortAgent> loadPorts, AlignerAgent alignerAgent, ProcessManager processManager, WaferManager waferManager)
        {
            _robotAgent = robotAgent;
            _loadPorts = loadPorts;
            _alignerAgent = alignerAgent;
            _processManager = processManager;
            _waferManager = waferManager;
        }

        // SEMI E40: Create Process Job
        public ProcessJob CreateProcessJob(string pjId, string recipeId, List<Substrate> substrates)
        {
            // 1. Validate ID Uniqueness (Mock: assuming unique for now)
            // 2. Validate Recipe Exists (Mock: assuming recipe string is valid)

            var recipe = new Recipe(recipeId);
            var pj = new ProcessJob(pjId, recipe);
            pj.SubstratesToProcess.AddRange(substrates);

            // E40: Initial state is POOLED (Waiting for assignment to a CJ)
            // For simplicty in our enum, we might not have POOLED, treating QUEUED/IDLE as POOLED context.
            // Let's assume standard behavior: Created but not running.
            SystemEventBus.PublishLog($"E40: ProcessJob '{pjId}' Created. State: POOLED. (Recipe: {recipeId}, Wafers: {substrates.Count})");

            return pj;
        }

        // SEMI E94: Create Control Job
        public ControlJob CreateControlJob(string cjId, List<ProcessJob> pjs, Carrier sourceCarrier)
        {
            // 1. Validate PJ Existence: PJs must be created first (passed in logic)
            if (pjs == null || pjs.Count == 0)
            {
                throw new System.Exception("E94 Error: ProcessingCtrlSpec must contain at least one Process Job.");
            }

            var cj = new ControlJob(cjId);

            // 2. Set ProcessingCtrlSpec (Add PJs)
            cj.ProcessJobs.AddRange(pjs);

            // 3. Set CarrierInputSpec
            // In a real object, we would store the CarrierID. 
            // cj.CarrierInputSpec = sourceCarrier.Id;

            // 4. Initial State: QUEUED
            cj.State = ControlJobState.QUEUED;

            SystemEventBus.PublishLog($"E94: ControlJob '{cjId}' Created. State: QUEUED. (Contains {pjs.Count} PJs, Source: {sourceCarrier.Id})");

            return cj;
        }

        public async Task ExecuteControlJob(ControlJob controlJob)
        {
            if (controlJob.State != ControlJobState.QUEUED)
            {
                // Strictly speaking, E94 execution starts from Queued.
                SystemEventBus.PublishLog($"Warning: Executing ControlJob '{controlJob.Id}' from state {controlJob.State}. Standard assumes QUEUED.");
            }

            SystemEventBus.PublishLog($"ControlJob '{controlJob.Id}': Starting execution (Selected).");
            controlJob.State = ControlJobState.EXECUTING;

            foreach (var processJob in controlJob.ProcessJobs)
            {
                await ExecuteProcessJob(controlJob, processJob);
            }

            controlJob.State = ControlJobState.COMPLETED;
            SystemEventBus.PublishLog($"ControlJob '{controlJob.Id}': Execution finished.");
        }

        private async Task ExecuteProcessJob(ControlJob cj, ProcessJob pj)
        {
            SystemEventBus.PublishLog($"CJ '{cj.Id}' -> ProcessJob '{pj.Id}': Starting processing with Recipe '{pj.Recipe.Id}'.");

            foreach (var substrate in pj.SubstratesToProcess)
            {
                // Register Wafer to Manager for tracking
                _waferManager.RegisterWafer(substrate);

                // Find which LoadPort has this substrate
                LoadPortAgent? targetPortAgent = null;
                Substrate? substrateInCarrier = null;

                foreach (var portAgent in _loadPorts.Values)
                {
                    if (portAgent.Carrier != null && portAgent.Carrier.SlotMap.ContainsKey(substrate.Slot))
                    {
                        // Check if the ID matches to be sure (optional but safer)
                        // For now we trust Slot if ID matches roughly or by logic
                        var s = portAgent.Carrier.SlotMap[substrate.Slot];
                        if (s.Id == substrate.Id)
                        {
                            targetPortAgent = portAgent;
                            substrateInCarrier = s;
                            break;
                        }
                    }
                }

                if (targetPortAgent != null && substrateInCarrier != null)
                {
                    // Update State Logic with WaferManager
                    _waferManager.UpdateWaferState(substrate.Id, SubstrateState.PRESENT);

                    // 1. Move substrate to Aligner
                    await _robotAgent.MoveSubstrate(targetPortAgent, substrate.Slot, "Aligner");

                    // 2. Align Substrate
                    await _alignerAgent.AlignSubstrate(substrateInCarrier);

                    // 3. Move from Aligner to Process Chamber
                    await _robotAgent.Transfer("Aligner", "ProcessChamber1", substrateInCarrier);

                    // 4. Process (Delegated to ProcessManager)
                    // ProcessManager handles the internal PROCESSING state
                    await _processManager.ProcessSubstrate(substrateInCarrier);
                    _waferManager.UpdateWaferState(substrate.Id, SubstrateState.PROCESSED); // Update global tracker

                    // 5. Move substrate back to carrier
                    await _robotAgent.MoveSubstrate_Return(targetPortAgent, substrate.Slot, "ProcessChamber1", substrateInCarrier);

                    // Final State
                    _waferManager.UpdateWaferState(substrate.Id, SubstrateState.PRESENT);
                }
                else
                {
                    SystemEventBus.PublishLog($"Error: Could not find substrate '{substrate.Id}' (Slot {substrate.Slot}) in any loaded carrier.");
                }
            }
            SystemEventBus.PublishLog($"CJ '{cj.Id}' -> ProcessJob '{pj.Id}': Completed.");
        }
    }

    // Extending RobotAgent to have a return function for simplicity
    public partial class RobotAgent
    {
        public async Task MoveSubstrate_Return(LoadPortAgent toPortAgent, int toSlot, string fromLocation, Substrate substrate)
        {
            var toPort = toPortAgent.LoadPort;
            if (State != RobotState.IDLE)
            {
                SystemEventBus.PublishLog("Error: Robot is busy.");
                return;
            }

            // Pick sequence from process chamber
            State = RobotState.MOVING_TO_SOURCE;
            SystemEventBus.PublishLog($"Robot: Moving to {fromLocation}.");
            await Task.Delay(500);

            State = RobotState.PICKING;
            SystemEventBus.PublishLog($"Robot: Picking processed substrate '{substrate.Id}' from {fromLocation}.");
            substrate.State = SubstrateState.PICKED;
            SystemEventBus.Publish(substrate);
            await Task.Delay(500);

            // Place sequence to carrier
            State = RobotState.MOVING_TO_DEST;
            SystemEventBus.PublishLog($"Robot: Moving substrate '{substrate.Id}' to LoadPort {toPort.Id}.");
            await Task.Delay(500);

            State = RobotState.PUTTING;
            SystemEventBus.PublishLog($"Robot: Placing substrate '{substrate.Id}' into Slot {toSlot}.");
            substrate.State = SubstrateState.PRESENT; // Back in carrier
            SystemEventBus.Publish(substrate);

            if (toPort.Carrier != null)
            {
                toPort.Carrier.SlotMap[toSlot] = substrate;
                SystemEventBus.Publish(toPort.Carrier);
            }
            await Task.Delay(500);

            // Return to home
            State = RobotState.MOVING_HOME;
            SystemEventBus.PublishLog("Robot: Moving to home position.");
            await Task.Delay(500);

            State = RobotState.IDLE;
            SystemEventBus.PublishLog("Robot: Idle.");
        }
    }
}