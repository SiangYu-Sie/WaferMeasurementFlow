namespace WaferMeasurementFlow.Models
{
    public enum CarrierState
    {
        IDLE,
        WAITING_FOR_TP1,
        TRANSFERRING,
        WAITING_FOR_TP2
    }

    public enum LoadPortState
    {
        EMPTY,
        CARRIER_PRESENT,
        CLAMPED,
        DOOR_OPEN,
        MAPPING,
        READY_TO_PROCESS, // Ready for Robot access
        READY_TO_UNLOAD   // Process finished, can remove carrier
    }

    public enum SubstrateState
    {
        UNKNOWN,
        EMPTY,
        PRESENT,
        PICKED,
        PROCESSING,
        PROCESSED
    }

    public enum RobotState
    {
        IDLE,
        MOVING_TO_SOURCE,
        PICKING,
        MOVING_TO_DEST,
        PUTTING,
        MOVING_HOME
    }

    public enum ControlJobState
    {
        QUEUED,
        SELECTED,
        WAITING_FOR_START,
        EXECUTING,
        PAUSED,
        COMPLETED,
        ABORTED
    }

    public enum AlignerState
    {
        IDLE,
        ALIGNING,
        COMPLETED,
        ERROR
    }
}