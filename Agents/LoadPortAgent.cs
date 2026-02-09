using System.Threading.Tasks;
using WaferMeasurementFlow.Models;
using WaferMeasurementFlow.Managers;

namespace WaferMeasurementFlow.Agents
{
    public class LoadPortAgent
    {
        public LoadPort LoadPort { get; private set; }
        private readonly E87Manager _e87Manager; // Dependency

        // Pass-through properties
        public int Id => LoadPort.Id;
        public LoadPortState State => LoadPort.State;
        public Carrier Carrier => LoadPort.Carrier;

        public LoadPortAgent(int id, E87Manager e87Manager)
        {
            LoadPort = new LoadPort(id);
            _e87Manager = e87Manager;
        }

        public async Task PlaceCarrier(Carrier carrier)
        {
            if (LoadPort.State == LoadPortState.EMPTY)
            {
                SystemEventBus.PublishLog($"LoadPort {LoadPort.Id}: Carrier '{carrier.Id}' placed.");
                LoadPort.Carrier = carrier;
                LoadPort.State = LoadPortState.CARRIER_PRESENT;
                SystemEventBus.Publish(LoadPort);

                // E87: Verify Carrier ID
                if (!await _e87Manager.VerifyCarrierId(carrier))
                {
                    SystemEventBus.PublishLog($"Error: Carrier ID Verification Failed. Process Aborted.");
                    return; // Stop flow
                }

                await Task.Delay(500); // Simulate clamping
                await ClampCarrier();
            }
            else
            {
                SystemEventBus.PublishLog($"Error: LoadPort {LoadPort.Id} is not empty.");
            }
        }

        private async Task ClampCarrier()
        {
            if (LoadPort.State == LoadPortState.CARRIER_PRESENT && LoadPort.Carrier != null)
            {
                SystemEventBus.PublishLog($"LoadPort {LoadPort.Id}: Clamping carrier '{LoadPort.Carrier.Id}'.");
                LoadPort.State = LoadPortState.CLAMPED;
                SystemEventBus.Publish(LoadPort);

                await Task.Delay(500); // Simulate door opening
                await OpenDoor();
            }
        }

        private async Task OpenDoor()
        {
            if (LoadPort.State == LoadPortState.CLAMPED && LoadPort.Carrier != null)
            {
                SystemEventBus.PublishLog($"LoadPort {LoadPort.Id}: Opening door for carrier '{LoadPort.Carrier.Id}'.");
                LoadPort.State = LoadPortState.DOOR_OPEN;
                SystemEventBus.Publish(LoadPort);

                await Task.Delay(1000); // Simulate mapping
                await MapWafers();
            }
        }

        private async Task MapWafers()
        {
            if (LoadPort.State == LoadPortState.DOOR_OPEN && LoadPort.Carrier != null)
            {
                SystemEventBus.PublishLog($"LoadPort {LoadPort.Id}: Mapping wafers in carrier '{LoadPort.Carrier.Id}'.");
                LoadPort.State = LoadPortState.MAPPING;
                SystemEventBus.Publish(LoadPort);

                // In a real system, this would involve a sensor reading the wafers.
                // Here, we just confirm the substrates in the model.
                foreach (var substrate in LoadPort.Carrier.SlotMap.Values)
                {
                    SystemEventBus.PublishLog($"  - Found Wafer '{substrate.Id}' in Slot {substrate.Slot}.");
                    await Task.Delay(50);
                }

                // E87: Verify Slot Map
                if (!await _e87Manager.VerifySlotMap(LoadPort.Carrier))
                {
                    SystemEventBus.PublishLog($"Error: Slot Map Verification Failed. Carrier content unknown.");
                    return;
                }

                LoadPort.State = LoadPortState.READY_TO_PROCESS;
                SystemEventBus.PublishLog($"LoadPort {LoadPort.Id}: Carrier '{LoadPort.Carrier.Id}' is ready for wafer processing (E87 Verified).");
                SystemEventBus.Publish(LoadPort);
            }
        }

        public void UnloadCarrier()
        {
            if (LoadPort.State == LoadPortState.READY_TO_UNLOAD)
            {
                SystemEventBus.PublishLog($"LoadPort {LoadPort.Id}: Carrier '{LoadPort.Carrier?.Id}' unloaded.");
                LoadPort.Carrier = null;
                LoadPort.State = LoadPortState.EMPTY;
                SystemEventBus.Publish(LoadPort);
            }
            else
            {
                SystemEventBus.PublishLog($"Error: LoadPort {LoadPort.Id} is not ready for unload.");
            }
        }
    }
}