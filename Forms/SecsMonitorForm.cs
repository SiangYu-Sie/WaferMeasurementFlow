using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WaferMeasurementFlow.Managers;
using WaferMeasurementFlow.Core;

namespace WaferMeasurementFlow.Forms
{
    public partial class SecsMonitorForm : Form
    {
        private readonly SecsManager _secsManager;
        private System.Windows.Forms.Timer _uiTimer;

        // Theme Colors - Industrial Professional Dark
        private static readonly Color BgPrimary = Color.FromArgb(18, 18, 22);
        private static readonly Color BgSecondary = Color.FromArgb(28, 28, 35);
        private static readonly Color BgCard = Color.FromArgb(38, 40, 48);
        private static readonly Color BgCardHover = Color.FromArgb(48, 50, 60);
        private static readonly Color BorderColor = Color.FromArgb(55, 60, 70);
        private static readonly Color TextPrimary = Color.FromArgb(240, 242, 245);
        private static readonly Color TextSecondary = Color.FromArgb(140, 145, 160);
        private static readonly Color TextMuted = Color.FromArgb(90, 95, 110);

        private static readonly Color StatusGreen = Color.FromArgb(34, 197, 94);
        private static readonly Color StatusYellow = Color.FromArgb(250, 204, 21);
        private static readonly Color StatusRed = Color.FromArgb(239, 68, 68);
        private static readonly Color StatusBlue = Color.FromArgb(59, 130, 246);
        private static readonly Color StatusGray = Color.FromArgb(100, 105, 120);

        // UI Components
        private Panel headerPanel = null!;
        private Panel contentPanel = null!;
        private RichTextBox logBox = null!;

        private StatusIndicator indInit = null!;
        private StatusIndicator indDriver = null!;
        private StatusIndicator indComm = null!;
        private StatusIndicator indControl = null!;

        // HSMS 連線設定 UI
        private ComboBox cmbConnectMode = null!;
        private TextBox txtLocalIP = null!;
        private TextBox txtLocalPort = null!;
        private TextBox txtRemoteIP = null!;
        private TextBox txtRemotePort = null!;
        private TextBox txtDeviceId = null!;
        private TextBox txtT3 = null!;
        private TextBox txtT5 = null!;

        private ActionButton btnInitialize = null!;
        private ActionButton btnStartDriver = null!;
        private ActionButton btnStopDriver = null!;
        private ActionButton btnEnableComm = null!;
        private ActionButton btnDisableComm = null!;
        private ActionButton btnOffline = null!;
        private ActionButton btnOnlineLocal = null!;
        private ActionButton btnOnlineRemote = null!;
        private ActionButton btnDriverConfig = null!;
        private ActionButton btnTabMonitor = null!;
        private ActionButton btnTabConfig = null!;

        public SecsMonitorForm(SecsManager secsManager)
        {
            _secsManager = secsManager;
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            InitializeComponent();

            _uiTimer = new System.Windows.Forms.Timer { Interval = 300 };
            _uiTimer.Tick += (s, e) => RefreshStatus();
            _uiTimer.Start();

            _secsManager.LogReceived += OnLogReceived;
        }

        private void InitializeComponent()
        {
            this.Text = "SECS/GEM Equipment Monitor";
            this.Size = new Size(1200, 850);
            this.BackColor = BgPrimary;
            this.ForeColor = TextPrimary;
            this.Font = new Font("Segoe UI", 9.5F);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(1000, 750);

            BuildHeader();
            BuildContent();

            this.FormClosing += (s, e) => { if (e.CloseReason == CloseReason.UserClosing) { e.Cancel = true; this.Hide(); } };
        }

        private void BuildHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = BgSecondary,
                Padding = new Padding(25, 0, 25, 0)
            };

            var titleLabel = new Label
            {
                Text = "SECS/GEM Monitor (DIASECSGEM300)",
                Font = new Font("Segoe UI Semibold", 16F),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(25, 20)
            };

            var versionLabel = new Label
            {
                Text = "v3.0",
                Font = new Font("Segoe UI", 9F),
                ForeColor = TextMuted,
                AutoSize = true,
                Location = new Point(355, 28)
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(versionLabel);

            // Tab Buttons in Header
            btnTabMonitor = new ActionButton("監控操作", StatusBlue) { Width = 140, Height = 40, Location = new Point(500, 15) };
            btnTabConfig = new ActionButton("連線設定", StatusGray) { Width = 140, Height = 40, Location = new Point(650, 15) };
            
            headerPanel.Controls.Add(btnTabMonitor);
            headerPanel.Controls.Add(btnTabConfig);

            this.Controls.Add(headerPanel);
        }

        private void BuildContent()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BgPrimary,
                Padding = new Padding(20)
            };
            this.Controls.Add(contentPanel);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            contentPanel.Controls.Add(mainLayout);

            // Left Panel used to hold Tabs (now Buttons + Content)
            var leftPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 10, 0) };
            mainLayout.Controls.Add(leftPanel, 0, 0);



            var tabContentPanel = new Panel 
            { 
                Dock = DockStyle.Fill, 
                BackColor = BgPrimary, 
                Padding = new Padding(1),
                AutoScroll = true
            };
            leftPanel.Controls.Add(tabContentPanel);
            
            // Define Layouts first so we can use them in button clicks
            var monitorLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                Padding = new Padding(5, 30, 5, 5), // Added Top Padding
                Visible = true
            };
            var configLayout = new TableLayoutPanel 
            { 
                Dock = DockStyle.Fill, 
                Padding = new Padding(20, 60, 20, 20), // Added Top Padding to avoid Header
                ColumnCount = 1,
                Visible = false
            };

            // Tab Switch Logic - Buttons in Header
            void SwitchTab(bool isMonitor)
            {
                monitorLayout.Visible = isMonitor;
                configLayout.Visible = !isMonitor;
                
                // Visual Feedback
                btnTabMonitor.BackColor = isMonitor ? StatusBlue : StatusGray;
                btnTabConfig.BackColor = !isMonitor ? StatusBlue : StatusGray;
            }
            
            btnTabMonitor.Click += (s, e) => SwitchTab(true);
            btnTabConfig.Click += (s, e) => SwitchTab(false);

            headerPanel.BringToFront(); // CRITICAL: Fix Z-Order so Header doesn't overlap Content

            // Improve Button Layout - Move Left as requested
            if (btnTabMonitor != null)
            {
                 btnTabMonitor.Location = new Point(470, 15); // Move closer to Title
                 btnTabMonitor.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            }
            if (btnTabConfig != null) 
            {
                btnTabConfig.Location = new Point(620, 15);
                btnTabConfig.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            }

            // === View 1: Monitor Layout ===
            tabContentPanel.Controls.Add(monitorLayout);
            monitorLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F)); // Status
            monitorLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 180F)); // Control Panel
            monitorLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Detail Info

            // === View 2: Config Layout ===
            tabContentPanel.Controls.Add(configLayout);

            // 1. Status Section (Add to monitorLayout)
            var statusSection = CreateSection("系統狀態總覽", 190);
            var statusGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 2,
                Padding = new Padding(10, 45, 10, 10)
            };
            statusGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            statusGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            statusGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            statusGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            indInit = new StatusIndicator("初始化", "等待中", StatusGray) { Dock = DockStyle.Fill, Margin = new Padding(5) };
            indDriver = new StatusIndicator("驅動連線", "未連線", StatusGray) { Dock = DockStyle.Fill, Margin = new Padding(5) };
            indComm = new StatusIndicator("通訊狀態", "已禁用", StatusRed) { Dock = DockStyle.Fill, Margin = new Padding(5) };
            indControl = new StatusIndicator("控制模式", "離線", StatusRed) { Dock = DockStyle.Fill, Margin = new Padding(5) };

            statusGrid.Controls.Add(indInit, 0, 0);
            statusGrid.Controls.Add(indDriver, 1, 0);
            statusGrid.Controls.Add(indComm, 0, 1);
            statusGrid.Controls.Add(indControl, 1, 1);
            statusSection.Controls.Add(statusGrid);
            monitorLayout.Controls.Add(statusSection, 0, 0);

            // 2. Controls Section (Add to monitorLayout)
            var ctrlSection = CreateSection("操作面板", 170);
            var ctrlFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(10, 50, 10, 10),
                AutoScroll = false
            };

            btnStartDriver = new ActionButton("啟動驅動", StatusBlue) { Margin = new Padding(5) };
            btnStartDriver.Click += BtnStartDriver_Click;
            btnStopDriver = new ActionButton("停止驅動", StatusGray) { Margin = new Padding(5) };
            btnStopDriver.Click += BtnStopDriver_Click;
            btnEnableComm = new ActionButton("啟用通訊", StatusGreen) { Margin = new Padding(5) };
            btnEnableComm.Click += BtnEnableComm_Click;
            btnDisableComm = new ActionButton("禁用通訊", StatusRed) { Margin = new Padding(5) };
            btnDisableComm.Click += BtnDisableComm_Click;
            btnOffline = new ActionButton("OFFLINE", StatusRed) { Margin = new Padding(5) };
            btnOffline.Click += (s, e) => { _secsManager.GoOffline(); RefreshStatus(); };
            btnOnlineLocal = new ActionButton("LOCAL", StatusYellow) { Margin = new Padding(5) };
            btnOnlineLocal.Click += (s, e) => { _secsManager.GoOnlineLocal(); RefreshStatus(); };
            btnOnlineRemote = new ActionButton("REMOTE", StatusGreen) { Margin = new Padding(5) };
            btnOnlineRemote.Click += (s, e) => { _secsManager.GoOnlineRemote(); RefreshStatus(); };

            ctrlFlow.Controls.AddRange(new Control[] { btnStartDriver, btnStopDriver, btnEnableComm, btnDisableComm, btnOffline, btnOnlineLocal, btnOnlineRemote });
            ctrlSection.Controls.Add(ctrlFlow);
            monitorLayout.Controls.Add(ctrlSection, 0, 1);

            // 3. Instruction Detail Section (Add to monitorLayout)
            var detailSection = CreateSection("操作說明", 0);
            var detailContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12, 45, 12, 12), BackColor = Color.Transparent };
            var detailText = new RichTextBox
            {
                Text = "【操作流程 - 參考 DIASECSGEM300】\n" +
                       "1. 設定 HSMS 連線參數 (IP/Port/Mode)\n" +
                       "2. 按「初始化」→ 執行 Initialize()\n" +
                       "3. 按「啟動驅動」→ 執行 DriverStart()\n" +
                       "4. 等待 InitialCompleted 事件觸發\n" +
                       "   → 自動執行 EnableComm + OnlineRemote\n\n" +
                       "【手動操作】\n" +
                       "• 啟用通訊：發送 S1F13 通訊握手\n" +
                       "• 禁用通訊：中斷 GEM 通訊層\n" +
                       "• OFFLINE / LOCAL / REMOTE：控制狀態切換\n\n" +
                       "【連線模式】\n" +
                       "• Passive：設備等待 Host 連入 (預設)\n" +
                       "• Active：設備主動連接 Host",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = TextSecondary,
                BackColor = BgCard,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };
            detailContent.Controls.Add(detailText);
            detailSection.Controls.Add(detailContent);
            monitorLayout.Controls.Add(detailSection, 0, 2);

            // === View 2: Config Layout Content ===
            // (configLayout created above, now adding children)


            var configSection = CreateSection("HSMS 連線設定", 350);
            var configPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 50, 20, 20), BackColor = Color.Transparent };
            configSection.Controls.Add(configPanel);

            // Revised coordinates with EXTRA large spacing
            int labelX = 20, valueX = 140, row2LabelX = 320, row2ValueX = 430;
            int rowH = 45; 
            int y = 60; // Push down to avoid Title overlap

            // Row 1: Connect Mode + Device ID
            configPanel.Controls.Add(CreateConfigLabel("連線模式:", labelX, y));
            cmbConnectMode = new ComboBox
            {
                BackColor = BgCardHover, ForeColor = TextPrimary, FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList, Width = 140, Font = new Font("Segoe UI", 10F),
                Location = new Point(valueX, y - 2)
            };
            cmbConnectMode.Items.AddRange(new object[] { "Passive", "Active" });
            cmbConnectMode.SelectedIndex = _secsManager.Config.ConnectMode == "Active" ? 1 : 0;
            cmbConnectMode.SelectedIndexChanged += (s, e) => UpdateConfigUI();
            configPanel.Controls.Add(cmbConnectMode);

            configPanel.Controls.Add(CreateConfigLabel("Device ID:", row2LabelX, y));
            txtDeviceId = CreateConfigTextBox(_secsManager.Config.DeviceId.ToString(), row2ValueX, y, 70);
            configPanel.Controls.Add(txtDeviceId);

            y += rowH;
            // Row 2: Local IP + Port
            configPanel.Controls.Add(CreateConfigLabel("Local IP:", labelX, y));
            txtLocalIP = CreateConfigTextBox("127.0.0.1", valueX, y, 140);
            configPanel.Controls.Add(txtLocalIP);

            configPanel.Controls.Add(CreateConfigLabel("Local Port:", row2LabelX, y));
            txtLocalPort = CreateConfigTextBox(_secsManager.Config.LocalPort.ToString(), row2ValueX, y, 70);
            configPanel.Controls.Add(txtLocalPort);

            y += rowH;
            // Row 3: Remote IP + Port (Active mode)
            configPanel.Controls.Add(CreateConfigLabel("Remote IP:", labelX, y));
            txtRemoteIP = CreateConfigTextBox(_secsManager.Config.ValidIpAddress, valueX, y, 140);
            configPanel.Controls.Add(txtRemoteIP);

            configPanel.Controls.Add(CreateConfigLabel("Remote Port:", row2LabelX, y));
            txtRemotePort = CreateConfigTextBox(_secsManager.Config.RemotePort.ToString(), row2ValueX, y, 70);
            configPanel.Controls.Add(txtRemotePort);

            y += rowH;
            // Row 4: T3 + T5
            configPanel.Controls.Add(CreateConfigLabel("T3 (sec):", labelX, y));
            txtT3 = CreateConfigTextBox(_secsManager.Config.T3Timeout.ToString(), valueX, y, 70);
            configPanel.Controls.Add(txtT3);

            configPanel.Controls.Add(CreateConfigLabel("T5 (sec):", row2LabelX, y));
            txtT5 = CreateConfigTextBox(_secsManager.Config.T5Timeout.ToString(), row2ValueX, y, 70);
            configPanel.Controls.Add(txtT5);

            y += rowH + 20;
            // Row 5: Initialize + DriverConfig buttons
            btnInitialize = new ActionButton("初始化", StatusBlue) { Width = 120, Height = 38, Margin = new Padding(0) };
            btnInitialize.Location = new Point(valueX, y); // Align with input boxes
            btnInitialize.Click += BtnInitialize_Click;
            configPanel.Controls.Add(btnInitialize);

            btnDriverConfig = new ActionButton("檢視設定", StatusGray) { Width = 120, Height = 38, Margin = new Padding(0) };
            btnDriverConfig.Location = new Point(row2LabelX, y); // Align with second column labels
            btnDriverConfig.Click += (s, e) =>
            {
                var cfg = _secsManager.Config;
                var info = $"HSMS Mode: {cfg.ConnectMode}\n" +
                           $"Local: {txtLocalIP.Text}:{cfg.LocalPort}\n" +
                           $"Remote: {cfg.ValidIpAddress}:{cfg.RemotePort}\n" +
                           $"DeviceId: {cfg.DeviceId}\n" +
                           $"T3={cfg.T3Timeout}, T5={cfg.T5Timeout}";
                MessageBox.Show(info, "HSMS Driver 設定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            configPanel.Controls.Add(btnDriverConfig);

            configLayout.Controls.Add(configSection);
            
            // Ensure Config is updated after controls are created
            UpdateConfigUI();

            // Right: Log Section
            var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 0, 0, 0) };
            mainLayout.Controls.Add(rightPanel, 1, 0);

            var logSection = CreateSection("系統日誌", 0);
            logSection.Dock = DockStyle.Fill;
            logBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(12, 12, 16),
                ForeColor = Color.FromArgb(180, 190, 200),
                Font = new Font("Consolas", 9F),
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };
            var logInner = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12, 45, 12, 12) };
            logInner.Controls.Add(logBox);
            logSection.Controls.Add(logInner);
            rightPanel.Controls.Add(logSection);
        }

        private Label CreateConfigLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                ForeColor = TextSecondary,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(x, y + 3)
            };
        }

        private TextBox CreateConfigTextBox(string text, int x, int y, int width)
        {
            return new TextBox
            {
                Text = text,
                BackColor = BgCardHover,
                ForeColor = TextPrimary,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9F),
                Width = width,
                Location = new Point(x, y)
            };
        }

        private void UpdateConfigUI()
        {
            bool isActive = cmbConnectMode.SelectedIndex == 1;
            txtRemoteIP.Enabled = isActive;
            txtRemotePort.Enabled = isActive;
        }

        /// <summary>
        /// 將 UI 上的設定值寫入 SecsManager.Config，參考 DIASECSGEM300 DriverConfigInfo
        /// </summary>
        private void ApplyConfigFromUI()
        {
            var cfg = _secsManager.Config;
            cfg.ConnectMode = cmbConnectMode.SelectedIndex == 1 ? "Active" : "Passive";
            cfg.ValidIpAddress = txtRemoteIP.Text.Trim();

            if (int.TryParse(txtLocalPort.Text, out int lp)) cfg.LocalPort = lp;
            if (int.TryParse(txtRemotePort.Text, out int rp)) cfg.RemotePort = rp;
            if (int.TryParse(txtDeviceId.Text, out int did)) cfg.DeviceId = did;
            if (int.TryParse(txtT3.Text, out int t3)) cfg.T3Timeout = t3;
            if (int.TryParse(txtT5.Text, out int t5)) cfg.T5Timeout = t5;
        }

        // ===================================================================
        // Button Handlers - 參考 DIASECSGEM300 MainForm 的連線流程
        // ===================================================================

        /// <summary>
        /// 初始化：對應 DIASECSGEM300 的 InitialDIASecsGem()
        /// 必須在 DriverStart 之前呼叫
        /// </summary>
        private void BtnInitialize_Click(object sender, EventArgs e)
        {
            ApplyConfigFromUI();
            _secsManager.InitializeController();
            RefreshStatus();
        }

        /// <summary>
        /// 啟動驅動：對應 DIASECSGEM300 的 btnStart_Click
        /// 僅執行 DriverStart，不自動 EnableComm/OnlineRemote
        /// InitialCompleted 事件會自動觸發後續動作
        /// </summary>
        private void BtnStartDriver_Click(object sender, EventArgs e)
        {
            _secsManager.StartDriver();
            RefreshStatus();
        }

        /// <summary>
        /// 停止驅動：對應 DIASECSGEM300 的 btnStop_Click
        /// </summary>
        private void BtnStopDriver_Click(object sender, EventArgs e)
        {
            _secsManager.StopDriver();
            RefreshStatus();
        }

        /// <summary>
        /// 啟用通訊：對應 DIASECSGEM300 的 btnEnableComm_Click
        /// </summary>
        private void BtnEnableComm_Click(object sender, EventArgs e)
        {
            _secsManager.EnableComm();
            RefreshStatus();
        }

        /// <summary>
        /// 禁用通訊：對應 DIASECSGEM300 的 btnDisableComm_Click
        /// </summary>
        private void BtnDisableComm_Click(object sender, EventArgs e)
        {
            _secsManager.DisableComm();
            RefreshStatus();
        }

        private Panel CreateSection(string title, int height)
        {
            var panel = new Panel
            {
                BackColor = BgCard,
                Margin = new Padding(0, 0, 0, 10),
                Dock = height == 0 ? DockStyle.Fill : DockStyle.Top,
                Height = height == 0 ? 100 : height
            };
            panel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var pen = new Pen(BorderColor, 1))
                    g.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                using (var brush = new SolidBrush(TextPrimary))
                    g.DrawString(title, new Font("Segoe UI Semibold", 10F), brush, 15, 15);
            };
            return panel;
        }

        private void RefreshStatus()
        {
            if (_secsManager == null || this.IsDisposed) return;
            if (this.InvokeRequired) { this.BeginInvoke(new Action(RefreshStatus)); return; }

            // Guard: UI controls may not be fully initialized yet
            if (btnInitialize == null) return;

            // Init
            int initRes = _secsManager.LastInitResult;
            indInit.SetStatus(initRes == 0 ? "成功" : (initRes == -1 ? "等待中" : $"失敗({initRes})"),
                              initRes == 0 ? StatusGreen : (initRes == -1 ? StatusGray : StatusRed));

            // Driver
            var drv = _secsManager.DriverStatus;
            string drvText = drv == SecsDriverState.Connected ? "已連線" : (drv == SecsDriverState.Connecting ? "連線中..." : (drv == SecsDriverState.Listening ? "等待連入..." : "未連線"));
            indDriver.SetStatus(drvText, drv == SecsDriverState.Connected ? StatusGreen : (drv == SecsDriverState.Disconnected ? StatusRed : StatusYellow));

            // Comm
            var comm = _secsManager.CommState;
            string commText = comm == SecsCommState.Communicating ? "通訊中" : (comm == SecsCommState.Disabled ? "已禁用" : "未通訊");
            indComm.SetStatus(commText, comm == SecsCommState.Communicating ? StatusGreen : StatusRed);

            // Control
            var ctl = _secsManager.ControlMode;
            string ctlText = ctl == SecsControlMode.OnlineRemote ? "遠端控制" : (ctl == SecsControlMode.OnlineLocal ? "本地控制" : "離線");
            indControl.SetStatus(ctlText, ctl == SecsControlMode.OnlineRemote ? StatusGreen : (ctl == SecsControlMode.OnlineLocal ? StatusYellow : StatusRed));

            // Buttons state management following DIASECSGEM300 flow
            bool isInitialized = initRes == 0;
            bool isDisconnected = drv == SecsDriverState.Disconnected;

            // Initialize 按鈕：只有在未初始化或 Driver 停止時才能按
            btnInitialize.Enabled = !isInitialized || isDisconnected;

            // 連線設定：只有在 Driver 未啟動時才能改
            bool canEditConfig = isDisconnected;
            cmbConnectMode.Enabled = canEditConfig;
            txtLocalIP.Enabled = canEditConfig;
            txtLocalPort.Enabled = canEditConfig;
            txtDeviceId.Enabled = canEditConfig;
            txtT3.Enabled = canEditConfig;
            txtT5.Enabled = canEditConfig;
            if (canEditConfig) UpdateConfigUI(); // 只在可編輯時更新 Active/Passive 邏輯

            // DriverStart/Stop：參考 DIASECSGEM300 switch(SECSDriverStatus)
            // Disconnection → btnStart=true, btnStop=false
            // Listening/Connecting → btnStart=false, btnStop=true
            // Connection → btnStart=false, btnStop=true
            btnStartDriver.Enabled = isInitialized && isDisconnected;
            btnStopDriver.Enabled = !isDisconnected;

            // EnableComm/DisableComm：參考 DIASECSGEM300 switch(CommunicationState)
            // Disabled → EnableComm=true, DisableComm=false
            // NotCommunicating → EnableComm=false, DisableComm=true
            // Communicating → EnableComm=false, DisableComm=true
            btnEnableComm.Enabled = comm == SecsCommState.Disabled && isInitialized;
            btnDisableComm.Enabled = comm != SecsCommState.Disabled;

            // Control State buttons：需要通訊已建立
            bool canCtl = comm == SecsCommState.Communicating;
            btnOffline.Enabled = canCtl && ctl != SecsControlMode.EquipmentOffline;
            btnOnlineLocal.Enabled = canCtl && ctl != SecsControlMode.OnlineLocal;
            btnOnlineRemote.Enabled = canCtl && ctl != SecsControlMode.OnlineRemote;
        }

        private void OnLogReceived(object sender, string log)
        {
            if (this.InvokeRequired) { this.BeginInvoke(new Action<object, string>(OnLogReceived), sender, log); return; }
            if (logBox.TextLength > 80000) logBox.Clear();
            logBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {log}{Environment.NewLine}");
            logBox.ScrollToCaret();
        }

        // --- Custom Controls ---

        private class StatusIndicator : Control
        {
            private string _title;
            private string _value = "";
            private Color _color = StatusGray;

            public StatusIndicator(string title, string value, Color color)
            {
                _title = title;
                _value = value;
                _color = color;
                DoubleBuffered = true;
                SetStyle(ControlStyles.SupportsTransparentBackColor, true);
                BackColor = Color.Transparent;
            }

            public void SetStatus(string value, Color color)
            {
                _value = value;
                _color = color;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Background
                using (var brush = new SolidBrush(BgCardHover))
                {
                    var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                    g.FillRectangle(brush, rect);
                }

                // Left accent
                using (var brush = new SolidBrush(_color))
                    g.FillRectangle(brush, 0, 0, 4, Height);

                // Lamp (right side)
                int lampSize = 16;
                int lampX = Width - lampSize - 15;
                int lampY = (Height - lampSize) / 2;
                using (var brush = new SolidBrush(Color.FromArgb(50, _color)))
                    g.FillEllipse(brush, lampX - 4, lampY - 4, lampSize + 8, lampSize + 8);
                using (var brush = new SolidBrush(_color))
                    g.FillEllipse(brush, lampX, lampY, lampSize, lampSize);

                // Title
                using (var brush = new SolidBrush(TextSecondary))
                    g.DrawString(_title, new Font("Segoe UI", 9F), brush, 15, 12);

                // Value
                using (var brush = new SolidBrush(TextPrimary))
                    g.DrawString(_value, new Font("Segoe UI Semibold", 13F), brush, 15, 35);
            }
        }

        private class ActionButton : Control
        {
            private Color _accentColor;
            private bool _hovering = false;

            public ActionButton(string text, Color accent)
            {
                Text = text;
                _accentColor = accent;
                Size = new Size(100, 38);
                DoubleBuffered = true;
                Cursor = Cursors.Hand;
            }

            protected override void OnMouseEnter(EventArgs e) { _hovering = true; Invalidate(); base.OnMouseEnter(e); }
            protected override void OnMouseLeave(EventArgs e) { _hovering = false; Invalidate(); base.OnMouseLeave(e); }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                Color bg = Enabled ? (_hovering ? Color.FromArgb(60, _accentColor) : Color.FromArgb(35, _accentColor)) : BgCard;
                Color border = Enabled ? _accentColor : BorderColor;
                Color fg = Enabled ? TextPrimary : TextMuted;

                using (var brush = new SolidBrush(bg))
                    g.FillRectangle(brush, 0, 0, Width, Height);
                using (var pen = new Pen(border, 1.5f))
                    g.DrawRectangle(pen, 1, 1, Width - 3, Height - 3);

                var textSize = TextRenderer.MeasureText(Text, Font);
                TextRenderer.DrawText(g, Text, new Font("Segoe UI", 9F, FontStyle.Bold),
                    new Point((Width - textSize.Width) / 2, (Height - textSize.Height) / 2), fg);
            }
        }
    }
}
