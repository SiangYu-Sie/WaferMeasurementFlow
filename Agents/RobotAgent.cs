using System.Threading.Tasks;
using WaferMeasurementFlow.Models;

namespace WaferMeasurementFlow.Agents
{
    public partial class RobotAgent
    {
        public RobotState State { get; private set; }

        public RobotAgent()
        {
            State = RobotState.IDLE;
        }

        public async Task MoveSubstrate(LoadPortAgent fromPortAgent, int fromSlot, string toLocation)
        {
            var fromPort = fromPortAgent.LoadPort;
            if (State != RobotState.IDLE)
            {
                SystemEventBus.PublishLog("Error: Robot is busy.");
                return;
            }

            if (fromPort.Carrier == null || !fromPort.Carrier.SlotMap.ContainsKey(fromSlot))
            {
                SystemEventBus.PublishLog($"Error: No substrate found at LoadPort {fromPort.Id}, Slot {fromSlot}.");
                return;
            }

            var substrate = fromPort.Carrier.SlotMap[fromSlot];

            // Pick sequence
            State = RobotState.MOVING_TO_SOURCE;
            SystemEventBus.PublishLog($"Robot: Moving to LoadPort {fromPort.Id}.");
            await Task.Delay(500);

            State = RobotState.PICKING;
            SystemEventBus.PublishLog($"Robot: Picking substrate '{substrate.Id}' from Slot {fromSlot}.");
            substrate.State = SubstrateState.PICKED;
            SystemEventBus.Publish(substrate);
            await Task.Delay(500);

            fromPort.Carrier.SlotMap.Remove(fromSlot); // Wafer is no longer in the carrier
            SystemEventBus.Publish(fromPort.Carrier);

            // Place sequence
            State = RobotState.MOVING_TO_DEST;
            SystemEventBus.PublishLog($"Robot: Moving substrate '{substrate.Id}' to {toLocation}.");
            await Task.Delay(500);

            State = RobotState.PUTTING;
            SystemEventBus.PublishLog($"Robot: Placing substrate '{substrate.Id}' at {toLocation}.");
            substrate.State = SubstrateState.PROCESSING; // Assume destination is a processing chamber
            SystemEventBus.Publish(substrate);
            await Task.Delay(500);
            
            // Return to home
            State = RobotState.MOVING_HOME;
            SystemEventBus.PublishLog("Robot: Moving to home position.");
            await Task.Delay(500);

            State = RobotState.IDLE;
            SystemEventBus.PublishLog("Robot: Idle.");
        }
    }
}