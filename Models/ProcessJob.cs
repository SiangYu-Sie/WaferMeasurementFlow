using System.Collections.Generic;

namespace WaferMeasurementFlow.Models
{
    /// <summary>
    /// SEMI E40: Process Job
    /// </summary>
    public class ProcessJob
    {
        public string Id { get; set; }
        public Recipe Recipe { get; set; }

        /// <summary>CarrierID: PJ 綁定的 Carrier ID</summary>
        public string CarrierID { get; set; } = "";

        /// <summary>PRProcessStart: TRUE=自動啟動, FALSE=手動啟動</summary>
        public bool PRProcessStart { get; set; } = true;

        /// <summary>PRMtlNameList: 待處理材料清單 (Wafer ID)</summary>
        public List<string> PRMtlNameList { get; set; } = new List<string>();

        /// <summary>E40 PJ 狀態</summary>
        public ProcessJobState State { get; set; } = ProcessJobState.POOLED;

        /// <summary>實際要處理的 Substrate 物件 (Carrier 到位後解析填入)</summary>
        public List<Substrate> SubstratesToProcess { get; set; }

        public ProcessJob(string id, Recipe recipe)
        {
            Id = id;
            Recipe = recipe;
            SubstratesToProcess = new List<Substrate>();
        }
    }
}