using System;
using System.Collections.Generic;
using Delta.DIAAuto.DIASECSGEM;
using Delta.DIAAuto.DIASECSGEM.Equipment;
using Delta.DIAAuto.DIASECSGEM.GEMDataModel;

namespace WaferMeasurementFlow.Managers
{
    /// <summary>
    /// SECS/GEM 基礎管理器，封裝 DIASECS SDK 的標準生命週期與狀態切換
    /// </summary>
    public abstract class BaseSecsManager : IDisposable
    {
        public event EventHandler? StatusChanged;
        public event EventHandler<string>? LogReceived;

        public SecsDriverState DriverStatus { get; protected set; } = SecsDriverState.Disconnected;
        public SecsCommState CommState { get; protected set; } = SecsCommState.Disabled;
        public SecsControlMode ControlMode { get; protected set; } = SecsControlMode.EquipmentOffline;
        public int LastInitResult { get; protected set; } = -1;
        public SecsConfig Config { get; private set; } = new SecsConfig();

        protected readonly DIASecsGemController _gemController;
        private readonly System.Threading.Timer _statusCheckTimer;

        public DIASecsGemController Controller => _gemController;

        protected BaseSecsManager()
        {
            _gemController = new DIASecsGemController();
            _gemController.DebugOutLogEvent += OnDebugLog;
            _gemController.InitialCompleted += InternalOnInitialCompleted;
            
            _statusCheckTimer = new System.Threading.Timer(CheckStatusInternal, null, 1000, 1000);
        }

        public virtual void InitializeController()
        {
            try
            {
                _gemController.SECSDriverSetting.EnableLog = Config.EnableLog;
                _gemController.SECSDriverSetting.SECS_Connect_Mode = eSECS_Comm_Mode.HSMS_MODE;
                _gemController.SECSDriverSetting.HSMS.Mode = Config.ConnectMode.Equals("Active", StringComparison.OrdinalIgnoreCase)
                    ? eHsmsConnectMode.Active : eHsmsConnectMode.Passive;
                _gemController.SECSDriverSetting.HSMS.DeviceId = (ushort)Config.DeviceId;
                _gemController.SECSDriverSetting.HSMS.T3 = Config.T3Timeout;
                _gemController.SECSDriverSetting.HSMS.T5 = Config.T5Timeout;
                _gemController.SECSDriverSetting.HSMS.LocalIP = "127.0.0.1";
                _gemController.SECSDriverSetting.HSMS.LocalPort = Config.LocalPort.ToString();

                if (_gemController.SECSDriverSetting.HSMS.Mode == eHsmsConnectMode.Active)
                {
                    _gemController.SECSDriverSetting.HSMS.PassiveIP = Config.ValidIpAddress;
                    _gemController.SECSDriverSetting.HSMS.PassivePort = Config.RemotePort.ToString();
                }

                string err;
                LastInitResult = _gemController.Initialize(out err, new List<ulong>());
                Log(LastInitResult == 0 ? "DIASECSGEM Initialize Completed." : $"Initialize Failed ({LastInitResult}): {err}");
                NotifyChanged();
            }
            catch (Exception ex) { Log("InitializeController Exception: " + ex.Message); }
        }

        public virtual void StartDriver()
        {
            if (DriverStatus == SecsDriverState.Connected) return;
            if (LastInitResult != 0) { Log("Initialize not completed."); return; }
            try
            {
                string err;
                int res = _gemController.DriverStart(out err);
                if (res == 0)
                {
                    DriverStatus = SecsDriverState.Connecting;
                    Log("Driver Started. Waiting for InitialCompleted...");
                }
                else Log($"DriverStart Failed: {err}");
                NotifyChanged();
            }
            catch (Exception ex) { Log("StartDriver Exception: " + ex.Message); }
        }

        public void StopDriver()
        {
            _gemController?.DriverStop();
            DriverStatus = SecsDriverState.Disconnected;
            CommState = SecsCommState.Disabled;
            NotifyChanged();
        }

        private void InternalOnInitialCompleted(object sender, EventArgs e)
        {
            Log("InitialCompleted. Auto EnableComm & OnlineRemote...");
            string err;
            _gemController.EnableComm(out err);
            System.Threading.Thread.Sleep(500);
            _gemController.OnlineRemote(out err);
            OnInitialCompleted();
        }

        protected virtual void OnInitialCompleted() { }

        public void EnableComm() { _gemController.EnableComm(out _); NotifyChanged(); }
        public void DisableComm() { _gemController.DisableComm(out _); NotifyChanged(); }
        public void GoOffline() { _gemController.OffLine(out _); NotifyChanged(); }
        public void GoOnlineLocal() { _gemController.OnLineLocal(out _); NotifyChanged(); }
        public void GoOnlineRemote() { _gemController.OnlineRemote(out _); NotifyChanged(); }

        public long UpdateSV(int id, object val) => _gemController.UpdateSV((ulong)id, val, out _);
        public long EventReport(int id) => _gemController.EventReportSend((ulong)id, out _);
        public long AlarmReport(int id, bool set) => _gemController.AlarmReportSend((ulong)id, set, out _);

        protected void Log(string msg) { LogReceived?.Invoke(this, msg); }
        protected void NotifyChanged() => StatusChanged?.Invoke(this, EventArgs.Empty);

        private void CheckStatusInternal(object state)
        {
            if (_gemController == null) return;
            
            string drv = _gemController.SECSDriverStatus.ToString();
            if (drv == "Connection") DriverStatus = SecsDriverState.Connected;
            else if (drv == "Listening") DriverStatus = SecsDriverState.Listening;
            
            string comm = _gemController.CommunicationState.ToString();
            if (comm == "Communicating") CommState = SecsCommState.Communicating;
            else if (comm == "Disabled") CommState = SecsCommState.Disabled;

            if (_gemController.GetSV(6, out _, out var val, out _) == 0 && val != null)
            {
                try {
                    byte sv = Convert.ToByte(val);
                    ControlMode = sv switch { 1 => SecsControlMode.EquipmentOffline, 4 => SecsControlMode.OnlineLocal, 5 => SecsControlMode.OnlineRemote, _ => ControlMode };
                } catch {}
            }
            NotifyChanged();
        }

        private void OnDebugLog(object sender, GemLogArgs e) => Log(e.Message);

        public virtual void Dispose()
        {
            _statusCheckTimer?.Dispose();
            // DIASecsGemController doesn't implement IDisposable in some versions, 
            // but we should call stop if available.
            _gemController?.DriverStop();
        }
    }
}
