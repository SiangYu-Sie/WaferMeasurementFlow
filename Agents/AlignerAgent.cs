using System.Threading.Tasks;
using WaferMeasurementFlow.Models;

namespace WaferMeasurementFlow.Agents
{
    public class AlignerAgent
    {
        public AlignerState State { get; private set; }

        public AlignerAgent()
        {
            State = AlignerState.IDLE;
        }

        public async Task AlignSubstrate(Substrate substrate)
        {
            if (State != AlignerState.IDLE)
            {
                SystemEventBus.PublishLog("Error: Aligner is busy.");
                return;
            }

            State = AlignerState.ALIGNING;
            SystemEventBus.PublishLog($"Aligner: Aligning substrate '{substrate.Id}'.");
            await Task.Delay(1000); // Simulate alignment

            State = AlignerState.COMPLETED;
            SystemEventBus.PublishLog($"Aligner: Alignment complete for '{substrate.Id}'.");
            await Task.Delay(200);

            State = AlignerState.IDLE;
        }
    }
}
