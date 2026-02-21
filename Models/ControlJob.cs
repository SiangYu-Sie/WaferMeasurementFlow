using System.Collections.Generic;

namespace WaferMeasurementFlow.Models
{
    /// <summary>
    /// SEMI E94: Control Job
    /// </summary>
    public class ControlJob
    {
        public string Id { get; set; }
        public ControlJobState State { get; set; }

        /// <summary>CarrierInputSpec: 來源載具 ID (SEMI E94)</summary>
        public string CarrierInputSpec { get; set; } = "";

        /// <summary>MtrlOutSpec: 處理完成後材料送往的目的地</summary>
        public string MtrlOutSpec { get; set; } = "";

        /// <summary>ProcessOrderMgmt: PJ 啟動順序 (LIST/ARRIVAL/OPTIMIZE)</summary>
        public ProcessOrderMgmt ProcessOrderMgmt { get; set; } = ProcessOrderMgmt.LIST;

        /// <summary>TargetPortId: 預期的目標 Port (可為 0 表示不指定)</summary>
        public int TargetPortId { get; set; }

        /// <summary>ProcessingCtrlSpec: 包含的 PJ 清單</summary>
        public List<ProcessJob> ProcessJobs { get; set; }

        public ControlJob(string id)
        {
            Id = id;
            ProcessJobs = new List<ProcessJob>();
            State = ControlJobState.QUEUED;
        }
    }
}