using System.Collections.Generic;

namespace WaferMeasurementFlow.Models
{
    public class ProcessJob
    {
        public string Id { get; set; }
        public Recipe Recipe { get; set; }
        public List<Substrate> SubstratesToProcess { get; set; }

        public ProcessJob(string id, Recipe recipe)
        {
            Id = id;
            Recipe = recipe;
            SubstratesToProcess = new List<Substrate>();
        }
    }
}