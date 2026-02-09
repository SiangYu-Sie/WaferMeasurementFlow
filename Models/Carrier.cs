using System.Collections.Generic;
using System.Linq;

namespace WaferMeasurementFlow.Models
{
    public class Carrier
    {
        public string Id { get; set; }
        public CarrierState State { get; set; }
        public Dictionary<int, Substrate> SlotMap { get; private set; }

        public Carrier(string id, int capacity = 25)
        {
            Id = id;
            State = CarrierState.IDLE;
            SlotMap = new Dictionary<int, Substrate>();
            for (int i = 1; i <= capacity; i++)
            {
                // Initially, all slots are empty. Substrates will be added manually for this simulation.
            }
        }
    }
}