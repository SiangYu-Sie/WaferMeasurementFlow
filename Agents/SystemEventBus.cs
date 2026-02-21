using System;
using WaferMeasurementFlow.Models;

namespace WaferMeasurementFlow.Agents
{
    public static class SystemEventBus
    {
        public static event Action<LoadPort> LoadPortStateChanged;
        public static event Action<Carrier> CarrierStateChanged;
        public static event Action<Substrate> SubstrateStateChanged;
        public static event Action<string> LogMessagePublished;
        public static event Action JobChanged;

        public static void Publish(LoadPort loadPort) => LoadPortStateChanged?.Invoke(loadPort);
        public static void Publish(Carrier carrier) => CarrierStateChanged?.Invoke(carrier);
        public static void Publish(Substrate substrate) => SubstrateStateChanged?.Invoke(substrate);
        public static void PublishLog(string message) => LogMessagePublished?.Invoke(message);
        public static void PublishJobChanged() => JobChanged?.Invoke();
    }
}