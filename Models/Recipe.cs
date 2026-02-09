using System.Collections.Generic;

namespace WaferMeasurementFlow.Models
{
    public class Recipe
    {
        public string Id { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public string TargetSlots { get; set; } = ""; // e.g., "1,2,3" or "1-5" or "ALL"

        public Recipe(string id)
        {
            Id = id;
            Parameters = new Dictionary<string, string>();
        }
    }
}