using System;
using System.Collections.Generic;
using Delta.DIAAuto.DIASECSGEM;
using Delta.DIAAuto.DIASECSGEM.Equipment;
using WaferMeasurementFlow.Agents;
using Delta.DIAAuto.DIASECSGEM.GEMDataModel;
using WaferMeasurementFlow.Services;

namespace WaferMeasurementFlow.Managers
{
    /// <summary>
    /// Wafer 流程專用的 SECS 管理器，負責封裝 SDK 呼叫與基礎事務
    /// 不包含任何 UI 介面，完全依賴事件驅動與外部 UI 互動。
    /// </summary>
    public class SecsManager : BaseSecsManager
    {
        private ControlJobAgent? _controlJobAgent;
        
        // 將 PJ/CJ 操作抽離到專屬的 Service
        public Gem300Service Gem300 { get; private set; }

        public event EventHandler<string>? RemotePJCJCreated;
        public event EventHandler? ShowMonitorRequested; // UI 解耦

        public SecsManager() : base()
        {
            _gemController.CreateObjectRequestCommand += OnCreateObjectRequest;
            Gem300 = new Gem300Service(this, Log); // 注入自身控制器
        }

        public void SetControlJobAgent(ControlJobAgent agent)
        {
            _controlJobAgent = agent;
            Log("SecsManager: ControlJobAgent 已綁定。");
        }

        // 外部 UI 按鈕點擊時呼叫此方法，由 Manager 觸發事件給 MainForm 開啟
        public void RequestShowMonitor()
        {
            ShowMonitorRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnCreateObjectRequest(object sender, CreateObjectRequestArgs e)
        {
            Log($"[GEM300] Received Remote CreateObjectRequest. Message: {e.ReceiveMessageName}");
            // 解析 S16F11/S16F15 並呼叫 Gem300 Service 處理
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
