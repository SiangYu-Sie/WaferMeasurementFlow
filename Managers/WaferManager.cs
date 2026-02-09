using System.Collections.Concurrent;
using System.Collections.Generic;
using WaferMeasurementFlow.Models;
using WaferMeasurementFlow.Agents;

namespace WaferMeasurementFlow.Managers
{
    public class WaferManager
    {
        private ConcurrentDictionary<string, Substrate> _allWafers = new ConcurrentDictionary<string, Substrate>();

        public void RegisterWafer(Substrate wafer)
        {
            if (_allWafers.TryAdd(wafer.Id, wafer))
            {
                SystemEventBus.PublishLog($"WaferManager: Registered wafer '{wafer.Id}'.");
            }
        }

        public Substrate GetWafer(string id)
        {
            _allWafers.TryGetValue(id, out var wafer);
            return wafer;
        }

        public void UpdateWaferState(string id, SubstrateState newState)
        {
            if (_allWafers.TryGetValue(id, out var wafer))
            {
                wafer.State = newState;
                SystemEventBus.PublishLog($"WaferManager: Updated wafer '{id}' state to {newState}.");
            }
        }
    }
}
