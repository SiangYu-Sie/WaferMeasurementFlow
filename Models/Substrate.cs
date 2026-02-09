namespace WaferMeasurementFlow.Models
{
    public class Substrate
    {
        public string Id { get; set; }
        public int Slot { get; set; }
        public SubstrateState State { get; set; }

        public Substrate(string id, int slot)
        {
            Id = id;
            Slot = slot;
            State = SubstrateState.PRESENT; // Assume it's present when created
        }
    }
}