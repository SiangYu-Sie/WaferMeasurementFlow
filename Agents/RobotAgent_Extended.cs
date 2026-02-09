using System.Threading.Tasks;
using WaferMeasurementFlow.Models;

namespace WaferMeasurementFlow.Agents
{
    public partial class RobotAgent
    {
        public async Task Transfer(string fromLocation, string toLocation, Substrate substrate)
        {
             if (State != RobotState.IDLE)
            {
                SystemEventBus.PublishLog("Error: Robot is busy.");
                return;
            }

            // Pick
            State = RobotState.MOVING_TO_SOURCE;
            SystemEventBus.PublishLog($"Robot: Moving to {fromLocation}.");
            await Task.Delay(500);

            State = RobotState.PICKING;
            SystemEventBus.PublishLog($"Robot: Picking '{substrate.Id}' from {fromLocation}.");
            substrate.State = SubstrateState.PICKED;
            SystemEventBus.Publish(substrate);
            await Task.Delay(500);

            // Place
            State = RobotState.MOVING_TO_DEST;
            SystemEventBus.PublishLog($"Robot: Moving to {toLocation}.");
            await Task.Delay(500);

            State = RobotState.PUTTING;
            SystemEventBus.PublishLog($"Robot: Placing '{substrate.Id}' at {toLocation}.");
            // State update depends on destination, generic here
            SystemEventBus.Publish(substrate);
            await Task.Delay(500);

            // Home
            State = RobotState.MOVING_HOME;
             SystemEventBus.PublishLog("Robot: Moving Home.");
            await Task.Delay(500);
            
            State = RobotState.IDLE;
        }
    }
}
