using System;

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
}
