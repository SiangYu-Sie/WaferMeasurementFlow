namespace WaferMeasurementFlow.Models
{
    public class LoadPort
    {
        public int Id { get; set; }
        public LoadPortState State { get; set; }
        public Carrier? Carrier { get; set; } // A load port can have one carrier

        public LoadPort(int id)
        {
            Id = id;
            State = LoadPortState.EMPTY;
            Carrier = null;
        }
    }
}