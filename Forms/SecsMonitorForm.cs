using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WaferMeasurementFlow.Managers;

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

        private ActionButton btnStartDriver = null!;
        private ActionButton btnStopDriver = null!;
        private ActionButton btnEnableComm = null!;
        private ActionButton btnDisableComm = null!;
        private ActionButton btnOffline = null!;
        private ActionButton btnOnlineLocal = null!;
        private ActionButton btnOnlineRemote = null!;

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
            this.Size = new Size(1100, 780);
            this.BackColor = BgPrimary;
            this.ForeColor = TextPrimary;
            this.Font = new Font("Segoe UI", 9.5F);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(900, 650);

            BuildHeader();
            BuildContent();

            this.FormClosing += (s, e) => { if (e.CloseReason == CloseReason.UserClosing) { e.Cancel = true; this.Hide(); } };
            RefreshStatus();
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
                Text = "SECS/GEM Monitor",
                Font = new Font("Segoe UI Semibold", 16F),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(25, 20)
            };

            var versionLabel = new Label
            {
                Text = "v2.0",
                Font = new Font("Segoe UI", 9F),
                ForeColor = TextMuted,
                AutoSize = true,
                Location = new Point(210, 28)
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(versionLabel);
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

            // Left: Status + Controls
            var leftPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 10, 0) };
            mainLayout.Controls.Add(leftPanel, 0, 0);

            var leftLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1
            };
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 260F)); // Status
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 220F)); // Controls
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Log
            leftPanel.Controls.Add(leftLayout);

            // Status Section
            var statusSection = CreateSection("系統狀態總覽", 260);
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
            leftLayout.Controls.Add(statusSection, 0, 0);

            // Controls Section
            var ctrlSection = CreateSection("操作面板", 220);
            var ctrlFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(10, 50, 10, 10),
                AutoScroll = false
            };

            btnStartDriver = new ActionButton("啟動驅動", StatusBlue) { Margin = new Padding(5) };
            btnStartDriver.Click += (s, e) => { _secsManager.StartDriver(); RefreshStatus(); };
            btnStopDriver = new ActionButton("停止驅動", StatusGray) { Margin = new Padding(5) };
            btnStopDriver.Click += (s, e) => { _secsManager.StopDriver(); RefreshStatus(); };
            btnEnableComm = new ActionButton("啟用通訊", StatusGreen) { Margin = new Padding(5) };
            btnEnableComm.Click += (s, e) => { _secsManager.EnableComm(); RefreshStatus(); };
            btnDisableComm = new ActionButton("禁用通訊", StatusRed) { Margin = new Padding(5) };
            btnDisableComm.Click += (s, e) => { _secsManager.DisableComm(); RefreshStatus(); };
            btnOffline = new ActionButton("OFFLINE", StatusRed) { Margin = new Padding(5) };
            btnOffline.Click += (s, e) => { _secsManager.GoOffline(); RefreshStatus(); };
            btnOnlineLocal = new ActionButton("LOCAL", StatusYellow) { Margin = new Padding(5) };
            btnOnlineLocal.Click += (s, e) => { _secsManager.GoOnlineLocal(); RefreshStatus(); };
            btnOnlineRemote = new ActionButton("REMOTE", StatusGreen) { Margin = new Padding(5) };
            btnOnlineRemote.Click += (s, e) => { _secsManager.GoOnlineRemote(); RefreshStatus(); };

            ctrlFlow.Controls.AddRange(new Control[] { btnStartDriver, btnStopDriver, btnEnableComm, btnDisableComm, btnOffline, btnOnlineLocal, btnOnlineRemote });
            ctrlSection.Controls.Add(ctrlFlow);
            leftLayout.Controls.Add(ctrlSection, 0, 1);

            // Detail Info Section (Left Bottom)
            var detailSection = CreateSection("操作說明", 0);
            var detailContent = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12, 45, 12, 12), BackColor = Color.Transparent };
            var detailText = new RichTextBox
            {
                Text = "【狀態說明】\n" +
                       "• 初始化：DIASECSGEM 控制器初始化結果\n" +
                       "• 驅動連線：TCP/IP 通訊層連線狀態\n" +
                       "• 通訊狀態：S1F13 通訊握手\n" +
                       "• 控制模式：設備控制狀態模型\n\n" +
                       "【控制模式】\n" +
                       "• OFFLINE：設備離線，不接受 Host 指令\n" +
                       "• LOCAL：本地控制模式\n" +
                       "• REMOTE：遠端控制模式，Host 完全控制\n\n" +
                       "【操作流程】\n" +
                       "1. 啟動驅動 → 建立 TCP 連線\n" +
                       "2. 啟用通訊 → 發送 S1F13 握手\n" +
                       "3. 切換控制模式 → 進入 Online",
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
            leftLayout.Controls.Add(detailSection, 0, 2);

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

            // Init
            int initRes = _secsManager.LastInitResult;
            indInit.SetStatus(initRes == 0 ? "成功" : (initRes == -1 ? "等待中" : "失敗"),
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

            // Buttons
            btnStartDriver.Enabled = drv == SecsDriverState.Disconnected;
            btnStopDriver.Enabled = drv != SecsDriverState.Disconnected;
            btnEnableComm.Enabled = comm != SecsCommState.Communicating && drv == SecsDriverState.Connected;
            btnDisableComm.Enabled = comm != SecsCommState.Disabled;

            bool canCtl = comm == SecsCommState.Communicating;
            btnOffline.Enabled = canCtl && ctl != SecsControlMode.EquipmentOffline;
            btnOnlineLocal.Enabled = canCtl && ctl != SecsControlMode.OnlineLocal;
            btnOnlineRemote.Enabled = canCtl && ctl != SecsControlMode.OnlineRemote;
        }

        private void OnLogReceived(object? sender, string log)
        {
            if (this.InvokeRequired) { this.BeginInvoke(new Action<object?, string>(OnLogReceived), sender, log); return; }
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
