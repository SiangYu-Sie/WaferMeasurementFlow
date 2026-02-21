using System;
using System.Collections.Generic;
using Delta.DIAAuto.DIASECSGEM;
using Delta.DIAAuto.DIASECSGEM.Equipment;
using Delta.DIAAuto.DIASECSGEM.GEMDataModel;
using WaferMeasurementFlow.Managers;

namespace WaferMeasurementFlow.Services
{
    /// <summary>
    /// 專司 GEM300 (ProcessJob / ControlJob) 各項屬性編譯與 SDK Object 管理
    /// </summary>
    public class Gem300Service
    {
        private readonly BaseSecsManager _secsManager;
        private readonly Action<string> _logger;
        private readonly List<string> _pjList = new List<string>();
        // private readonly List<string> _cjList = new List<string>();
        private const int CarrierCapacity = 25;

        public Gem300Service(BaseSecsManager secsManager, Action<string> logger)
        {
            _secsManager = secsManager;
            _logger = logger;
        }

        public int CreateProcessJob(string objID, string recID, bool bStart, string carrierID, byte[] slot, out string err, string objSpec = "")
        {
            var controller = _secsManager.Controller;
            int result = controller.CreateObject(ObjectTypeKey.PROCESSJOB, objID, out err, objSpec);
            if (result != 0) return result;

            var entity = new ObjectInstance();
            entity.ObjType = ObjectTypeKey.PROCESSJOB;
            entity.ObjID = objID;
            entity.ListObjectAttributes.Add(new ObjectAttribute(141, ObjectAttributeKey.OBJID, objID));
            entity.ListObjectAttributes.Add(new ObjectAttribute(143, ObjectAttributeKey.ProcessJob.PRJOBSTATE, (byte)0)); // QUEUED
            entity.ListObjectAttributes.Add(new ObjectAttribute(146, ObjectAttributeKey.ProcessJob.PRPROCESSSTART, new bool[] { bStart }));
            entity.ListObjectAttributes.Add(new ObjectAttribute(148, ObjectAttributeKey.ProcessJob.RECID, recID));

            controller.UpdateObject(entity, out err);
            _pjList.Add(objID);
            _logger($"[GEM300] SDK PJ '{objID}' created.");
            return result;
        }

        public int GetProcessJobAttr(string objID, out string pauseEvent, out byte PJState, out string carrierID,
            out byte[] slot, out byte PRType, out bool bStart, out byte recMethod, out string recID, out string recVarList, out string err)
        {
            pauseEvent = ""; PJState = 0; carrierID = ""; slot = new byte[CarrierCapacity];
            PRType = 0x0D; bStart = false; recMethod = 0; recID = ""; recVarList = ""; err = "";

            ObjectInstance entity = null;
            var controller = _secsManager.Controller;
            int result = controller.GetObject(ObjectTypeKey.PROCESSJOB, objID, out entity, out err);
            if (result != 0 || entity == null) return result;

            // [解析屬性邏輯...]
            return result;
        }

        public int ChangeProcessJobState(string objID, object stateObj)
        {
            byte state = Convert.ToByte(stateObj);
            var entity = new ObjectInstance();
            entity.ObjID = objID;
            entity.ObjType = ObjectTypeKey.PROCESSJOB;
            entity.ListObjectAttributes.Add(new ObjectAttribute(143, ObjectAttributeKey.ProcessJob.PRJOBSTATE, state));
            
            string err;
            int result = _secsManager.Controller.UpdateObject(entity, out err);
            if (result == 0) _logger($"[GEM300] PJ '{objID}' state -> {state}");
            return result;
        }

        public int CreateControlJobInSDK(string cjId, List<string> carrierList, string[] pjIds, byte prOrder, bool autoStart, out string err)
        {
            err = "Not Implemented";
            _logger($"[GEM300] CreateControlJobInSDK called for CJ '{cjId}' (Not Implemented)");
            return -1;
        }

        public int ChangeControlJobState(string cjId, byte state, int mtrlOutState)
        {
            _logger($"[GEM300] ChangeControlJobState called for CJ '{cjId}' -> {state} (Not Implemented)");
            return -1;
        }
    }
}
