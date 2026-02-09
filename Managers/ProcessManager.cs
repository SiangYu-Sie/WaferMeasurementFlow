using System.Threading.Tasks;
using WaferMeasurementFlow.Agents;
using WaferMeasurementFlow.Models;

namespace WaferMeasurementFlow.Managers
{
    public class ProcessManager
    {
        public async Task ProcessSubstrate(Substrate substrate)
        {
            SystemEventBus.PublishLog($"ProcessManager: Starting process on '{substrate.Id}'.");
            substrate.State = SubstrateState.PROCESSING;
            SystemEventBus.Publish(substrate);

            await Task.Delay(2000); // Simulate processing

            substrate.State = SubstrateState.PROCESSED;
            SystemEventBus.PublishLog($"ProcessManager: Finished process on '{substrate.Id}'.");
            SystemEventBus.Publish(substrate);
        }
    }
}
