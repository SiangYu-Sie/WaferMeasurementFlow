using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Delta.DIAAuto.DIASECSGEM;
using Delta.DIAAuto.DIASECSGEM.Equipment;
using Delta.DIAAuto.DIASECSGEM.GEMDataModel;

namespace WaferMeasurementFlow.Managers
{
    public enum SecsDriverState { Disconnected, Listening, Connecting, Connected }
    public enum SecsCommState { Disabled, NotCommunicating, WaitCRFromHost, WaitDelay, Communicating }
    public enum SecsControlMode { EquipmentOffline, HostOffline, OnlineLocal, OnlineRemote }

    public class SecsConfig
    {
        public bool EnableLog { get; set; } = true;
        public string ConnectMode { get; set; } = "Passive"; // Active or Passive
        public string ValidIpAddress { get; set; } = "127.0.0.1";
        public int LocalPort { get; set; } = 5000;
        public int RemotePort { get; set; } = 5000;
        public int DeviceId { get; set; } = 1;
        public int T3Timeout { get; set; } = 45;
        public int T5Timeout { get; set; } = 10;
        public int RetryInterval { get; set; } = 5000;
    }

    public class SecsManager : IDisposable
    {
        public event EventHandler? StatusChanged;
        public event EventHandler<string>? LogReceived;

        public bool IsDriverConnecting { get; private set; } = false;
        public SecsDriverState DriverStatus { get; private set; } = SecsDriverState.Disconnected;
        public SecsCommState CommState { get; private set; } = SecsCommState.Disabled;
        public SecsControlMode ControlMode { get; private set; } = SecsControlMode.EquipmentOffline;
        public int LastInitResult { get; private set; } = -1; // -1: Not started, 0: Success, >0: Error code
        public SecsConfig Config { get; private set; } = new SecsConfig();

        // 核心 Controller，由這裡自我管理，不依賴 DemoForm
        private DIASecsGemController _gemController;
        private Forms.SecsMonitorForm _monitorForm;
        private readonly System.Threading.Timer _statusCheckTimer;

        public DIASecsGemController Controller => _gemController;

        public SecsManager()
        {
            _gemController = new DIASecsGemController();

            // Subscribe Events
            _gemController.DebugOutLogEvent += OnDebugLog;

            // Start a timer to poll status because some events might not fire or we want distinct state
            _statusCheckTimer = new System.Threading.Timer(CheckStatus, null, 1000, 1000);
        }

        private void NotifyChanged()
        {
            StatusChanged?.Invoke(this, EventArgs.Empty);
        }

        private void CheckStatus(object state)
        {
            try
            {
                if (_gemController == null) return;

                // 1. Driver Status
                // Use ToString() to avoid missing enum type compile error
                string drvState = _gemController.SECSDriverStatus.ToString();
                switch (drvState)
                {
                    case "Disconnection":
                        DriverStatus = SecsDriverState.Disconnected;
                        break;
                    case "Listening":
                        DriverStatus = SecsDriverState.Listening;
                        break;
                    case "Connecting":
                        DriverStatus = SecsDriverState.Connecting;
                        break;
                    case "Connection":
                        DriverStatus = SecsDriverState.Connected;
                        break;
                }

                // 2. Comm Status
                string commState = _gemController.CommunicationState.ToString();
                switch (commState)
                {
                    case "Disabled":
                        CommState = SecsCommState.Disabled;
                        break;
                    case "NotCommunicating":
                        CommState = SecsCommState.NotCommunicating;
                        break;
                    case "Communicating":
                        CommState = SecsCommState.Communicating;
                        break;
                    default:
                        if (commState.Contains("Wait")) CommState = SecsCommState.WaitCRFromHost;
                        break;
                }

                // 3. Control State (Read SVID 6)
                // Use "out var" to avoid needing explicit ItemFmt type
                if (_gemController.GetSV(6, out var fmt, out var val, out var err) == 0 && val != null)
                {
                    try
                    {
                        byte svVal = Convert.ToByte(val);
                        switch (svVal)
                        {
                            case 1: ControlMode = SecsControlMode.EquipmentOffline; break;
                            case 3: ControlMode = SecsControlMode.HostOffline; break;
                            case 4: ControlMode = SecsControlMode.OnlineLocal; break;
                            case 5: ControlMode = SecsControlMode.OnlineRemote; break;
                        }
                    }
                    catch { }
                }

                NotifyChanged();
            }
            catch { }
        }

        // --- Driver Control ---
        public void StartDriver()
        {
            if (DriverStatus == SecsDriverState.Connected) return;

            try
            {
                _gemController.SECSDriverSetting.EnableLog = Config.EnableLog;
                // Use default enums if possible, or cast int.
                // Assuming standard enums available or compiled against.

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
                int res = _gemController.Initialize(out err, new List<ulong>());
                LastInitResult = res;

                if (res != 0)
                {
                    Log($"Initialize Failed: {err}");
                    return;
                }

                res = _gemController.DriverStart(out err);
                if (res != 0) Log($"DriverStart Failed: {err}");
                else
                {
                    IsDriverConnecting = true;
                    DriverStatus = SecsDriverState.Connecting;
                }

                NotifyChanged();
            }
            catch (Exception ex)
            {
                Log("StartDriver Exception: " + ex.Message);
            }
        }

        public void StopDriver()
        {
            try
            {
                _gemController?.DriverStop();
            }
            catch { }

            DriverStatus = SecsDriverState.Disconnected;
            IsDriverConnecting = false;
            CommState = SecsCommState.Disabled;
            NotifyChanged();
        }

        public void ShowMonitor()
        {
            if (_monitorForm == null || _monitorForm.IsDisposed)
            {
                _monitorForm = new Forms.SecsMonitorForm(this);
            }
            _monitorForm.Show();
            _monitorForm.BringToFront();
        }

        // --- Communication ---
        public void EnableComm()
        {
            string err;
            _gemController.EnableComm(out err);
            _gemController.OnlineRemote(out err);
            NotifyChanged();
        }

        public void DisableComm()
        {
            string err;
            _gemController.DisableComm(out err);
            NotifyChanged();
        }

        // --- Control State ---
        public void GoOffline()
        {
            string err;
            _gemController.OffLine(out err);
            NotifyChanged();
        }

        public void GoOnlineLocal()
        {
            string err;
            _gemController.OnLineLocal(out err);
            NotifyChanged();
        }

        public void GoOnlineRemote()
        {
            string err;
            _gemController.OnlineRemote(out err);
            NotifyChanged();
        }

        // --- GEM Methods Re-implementation ---
        public long UpdateLoadPortAccessMode(string objID, byte accessMode)
        {
            if (!(objID == "1" || objID == "2")) return -1;

            string err;
            ObjectInstance updateObj = new ObjectInstance();
            updateObj.ObjID = objID;
            updateObj.ObjType = ObjectTypeKey.LOADPORT;
            updateObj.ObjSpec = string.Empty;

            ulong attrId = (objID == "1") ? 2006UL : 2007UL;
            updateObj.ListObjectAttributes.Add(new ObjectAttribute(attrId, "ACCESSMODE", accessMode));

            return _gemController.UpdateObject(updateObj, out err);
        }

        public long UpdateLoadPortTransferState(string objID, byte transferState)
        {
            return 0;
        }

        public long UpdateLoadPortAssociationState(string objID, byte assocState) { return 0; }

        public int LoadportMatchToRun(string lpno, ref string[] lotID, ref string[] substrateID) { return 0; }

        public int CreateCarrier(string foupid, string loc) { return 0; }
        public int DeleteCarrier(string foupid, out string err) { err = ""; return 0; }

        // --- SV / EC / EVENTS ---
        public long UpdateSV(int id, object val, out string errLog)
        {
            return _gemController.UpdateSV((ulong)id, val, out errLog);
        }

        public long UpdateEC(int id, object val, out string errLog)
        {
            return _gemController.UpdateEC((ulong)id, val, true, out errLog);
        }

        public long EventReportSend(int id, out string errLog)
        {
            return _gemController.EventReportSend((ulong)id, out errLog);
        }

        public long AlarmReportSend(int id, bool bSet, out string errLog)
        {
            return _gemController.AlarmReportSend((ulong)id, bSet, out errLog);
        }

        // --- Logging ---
        private void OnDebugLog(object sender, GemLogArgs e)
        {
            string msg = $"[{e.Type}] {e.Message}";
            Log(msg);
        }

        private void Log(string msg)
        {
            LogReceived?.Invoke(this, msg);
        }

        public void Dispose()
        {
            _statusCheckTimer?.Dispose();
            _gemController?.Close();
        }


        // --- Stubs to fix compilation errors from Equipment.cs usage ---
        public int GetControlJobAttr(string r, out string c, out string p, out string d, out string m, out string s, out string pa, out string pc, out byte po, out bool bs, out byte st, out string err)
        { c = p = d = m = s = pa = pc = err = ""; po = 0; bs = false; st = 0; return 0; }

        public int GetProcessJobAttr(string o, out string p, out byte ps, out string c, out byte[] sl, out byte pt, out bool bs, out byte rm, out string ri, out string rv, out string err)
        { p = c = ri = rv = err = ""; ps = pt = rm = 0; sl = new byte[0]; bs = false; return 0; }

        public int SetCarrierStatus_ID(string d, object st) { return 0; }
        public int GetCarrierContentMap(string f, out string[] l, out string[] s, out string e) { l = s = new string[0]; e = ""; return 0; }
        public int SetCarrierAttr_Location(string f, string l, byte c = 25) { return 0; }
        public int SetCarrierAttr_SlotMap(string f, int[] s) { return 0; }
        public int SetCarrierStatus_SlotMap(string f, object s) { return 0; }
        public int SetCarrierStatus_Accessing(string f, object s) { return 0; }
        public int ChangeControlJobState(string c, object s, int u) { return 0; }
        public int SetControlJobCurrentPrJob(string c, string p) { return 0; }
        public int ChangeProcessJobState(string p, object s) { return 0; }
        public int CreateSubstrate(string o, string l, string loc, string spec = "") { return 0; }
        public int SetSubstrateAttr_Location(string o, string l) { return 0; }
        public int UpdateMeasurementData(string[] a, double[] h1, double[] w1, double[] h2, double[] w2) { return 0; }
        public int UpdateMeasurementMax(double h1, double w1, double h2, double w2) { return 0; }
        public int DeleteControlJobWithAssociatedProcessJob(string o, out string e) { e = ""; return 0; }
        public int DeleteControlJob(string o, out string e) { e = ""; return 0; }
    }
}
