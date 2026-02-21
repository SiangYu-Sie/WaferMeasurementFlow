using WaferMeasurementFlow.Agents;
using WaferMeasurementFlow.Managers;
using WaferMeasurementFlow.Services;
using System.Collections.Generic;

namespace WaferMeasurementFlow.Core
{
    public class Equipment
    {
        // Hardware Agents
        public RobotAgent Robot { get; private set; }
        public Dictionary<int, LoadPortAgent> LoadPorts { get; private set; } = new Dictionary<int, LoadPortAgent>();
        // Helper to keep code compatible for now/easy access
        public LoadPortAgent LoadPort1 => LoadPorts[1];
        public LoadPortAgent LoadPort2 => LoadPorts[2];

        public AlignerAgent Aligner { get; private set; }

        // Managers
        public ControlJobAgent ControlJobManager { get; private set; } // Renamed to clarify role, acts as CJ Manager
        public ProcessManager ProcessManager { get; private set; }
        public E87Manager E87Manager { get; private set; }
        public WaferManager WaferManager { get; private set; }
        public RecipeManager RecipeManager { get; private set; } // New Manager
        
        private SecsManager _secsManager;
        public SecsManager SecsManager 
        { 
            get
            {
                if (_secsManager == null)
                {
                    try
                    {
                        _secsManager = new SecsManager();
                    }
                    catch (System.TypeInitializationException ex)
                    {
                        string errorMsg = $"無法初始化 SECS Manager: {ex.Message}";
                        
                        if (ex.InnerException != null)
                        {
                            errorMsg += $"\n內部異常: {ex.InnerException.Message}";
                            
                            // 檢查是否為 DNGuard 相關錯誤
                            if (ex.InnerException.Message.Contains("DNGuard"))
                            {
                                errorMsg += "\n\n解決方法:";
                                errorMsg += "\n1. 檢查應用程式目錄是否包含 DNGuard Runtime 檔案 (DNGuard.Runtime.dll, DNGuard.HVM.dll)";
                                errorMsg += "\n2. 確認檔案未被防毒軟體封鎖或隔離";
                                errorMsg += "\n3. 聯繫 DIASECSGEM 供應商獲取完整安裝套件";
                                errorMsg += "\n4. 檢查檔案是否具有執行權限 (右鍵 > 內容 > 解除封鎖)";
                                
                                // 列出應用程式目錄資訊
                                var appPath = System.AppDomain.CurrentDomain.BaseDirectory;
                                errorMsg += $"\n\n應用程式目錄: {appPath}";
                                errorMsg += "\n缺少必要的 DNGuard Runtime 檔案";
                                
                                try
                                {
                                    var dllFiles = System.IO.Directory.GetFiles(appPath, "*.dll");
                                    errorMsg += $"\n目前目錄中的 DLL 數量: {dllFiles.Length}";
                                }
                                catch { }
                            }
                        }
                        
                        LogSystem?.LogError(errorMsg);
                        throw new System.Exception(errorMsg, ex);
                    }
                }
                return _secsManager;
            }
            private set => _secsManager = value;
        }

        // System Services
        public LogSystem LogSystem { get; private set; }

        public Equipment()
        {
            InitializeSystem();
        }

        private void InitializeSystem()
        {
            // Initialize Services
            LogSystem = new LogSystem();

            // Initialize Logic Managers
            ProcessManager = new ProcessManager();
            E87Manager = new E87Manager();
            WaferManager = new WaferManager();
            RecipeManager = new RecipeManager(); // Initialize RecipeManager
            // SecsManager 改為延遲初始化,在首次存取時才建立

            // Initialize Hardware Agents
            Robot = new RobotAgent();

            // Initialize 2 Load Ports
            LoadPorts[1] = new LoadPortAgent(1, E87Manager);
            LoadPorts[2] = new LoadPortAgent(2, E87Manager);

            Aligner = new AlignerAgent();

            // ControlJobAgent orchestrates the others
            // Note: ControlJobAgent needs update to handle multiple ports or look them up from Equipment
            ControlJobManager = new ControlJobAgent(Robot, LoadPorts, Aligner, ProcessManager, WaferManager, RecipeManager);
        }
    }
}
