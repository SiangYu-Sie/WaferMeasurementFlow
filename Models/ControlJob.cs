using System.Collections.Generic;

namespace WaferMeasurementFlow.Models
{
    public class ControlJob
    {
        public string Id { get; set; }
        public ControlJobState State { get; set; }
        public List<ProcessJob> ProcessJobs { get; set; }

        public ControlJob(string id)
        {
            Id = id;
            State = ControlJobState.QUEUED;
            ProcessJobs = new List<ProcessJob>();
        }
    }
}