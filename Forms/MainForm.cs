using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using WaferMeasurementFlow.Models;
using WaferMeasurementFlow.UI;
using WaferMeasurementFlow.Helpers;
using WaferMeasurementFlow.Agents;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaferMeasurementFlow.Core;

namespace WaferMeasurementFlow.Forms
{
    public partial class MainForm : Form
    {
        private readonly Equipment _equipment;
        private readonly IServiceProvider _serviceProvider;

        // UI Controls
        private StatusIndicator _indPort1;
        private StatusIndicator _indPort2;
        private Label _lblCarrierId;
        private ComboBox _comboPort;

        private ActionButton _btnPlace;
        private ActionButton _btnUnload;
        private ActionButton _btnPJCJ;
        private ActionButton _btnRecipes;
        private ActionButton _btnSecs;
        private ActionButton _btnEtel;

        private DataGridView _gridSlots;
        private DataGridView _gridJobs; // PJ/CJ 列表
        private RichTextBox _txtLog;

        public MainForm(Equipment equipment, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _equipment = equipment;

            _equipment.SecsManager.ShowMonitorRequested += (s, e) =>
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(() => ShowSecsMonitor()));
                    return;
                }
                ShowSecsMonitor();
            };
            _serviceProvider = serviceProvider;

            BuildUI();

            SubscribeToEvents();
            UpdateUI();
        }

        private void BuildUI()
        {
            // Theme Setup
            this.BackColor = IndTheme.BgPrimary;
            this.ForeColor = IndTheme.TextPrimary;
            this.Font = IndTheme.BodyFont;
            this.Size = new Size(1280, 800);
            this.Text = "晶圓量測流程系統 (Wafer Measurement Flow System)";

            // Main Layout (2 Columns)
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10),
                BackColor = Color.Transparent
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F)); // Left: Controls
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F)); // Right: Data
            this.Controls.Add(layout);

            // === LEFT PANEL ===
            var leftPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 10, 0) };
            layout.Controls.Add(leftPanel, 0, 0);

            var leftLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 300F)); // Ports Status
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Operations
            leftPanel.Controls.Add(leftLayout);

            // 1. Port Status Section
            var secPorts = new SectionPanel { Title = "裝載埠狀態 (Load Port Status)", Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 10) };
            var pnlPorts = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15, 45, 15, 15), BackColor = Color.Transparent };
            secPorts.Controls.Add(pnlPorts);

            var gridPorts = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
            gridPorts.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            gridPorts.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            pnlPorts.Controls.Add(gridPorts);

            _indPort1 = new StatusIndicator("Load Port 1", "未知", IndTheme.StatusGray) { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 5) };
            _indPort2 = new StatusIndicator("Load Port 2", "未知", IndTheme.StatusGray) { Dock = DockStyle.Fill, Margin = new Padding(0, 5, 0, 0) };

            gridPorts.Controls.Add(_indPort1, 0, 0);
            gridPorts.Controls.Add(_indPort2, 0, 1);

            _lblCarrierId = new Label
            {
                Text = "當前載具: -",
                AutoSize = true,
                ForeColor = IndTheme.TextSecondary,
                Location = new Point(200, 15), // Top right of header
                Font = IndTheme.BodyFont
            };
            secPorts.Controls.Add(_lblCarrierId);
            leftLayout.Controls.Add(secPorts, 0, 0);

            // 2. Operations Section
            var secOps = new SectionPanel { Title = "系統操作 (System Operations)", Dock = DockStyle.Fill };
            var pnlOps = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15, 45, 15, 15), BackColor = Color.Transparent };
            secOps.Controls.Add(pnlOps);

            var flowOps = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };
            pnlOps.Controls.Add(flowOps);

            // Active Port Selection
            var lblPortParams = new Label { Text = "目標裝載埠 (Target Port):", AutoSize = true, ForeColor = IndTheme.TextMuted, Margin = new Padding(3, 0, 3, 5) };
            flowOps.Controls.Add(lblPortParams);

            _comboPort = new ComboBox
            {
                Width = 200,
                BackColor = IndTheme.BgCardHover,
                ForeColor = IndTheme.TextPrimary,
                FlatStyle = FlatStyle.Flat,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(3, 0, 3, 15)
            };
            _comboPort.Items.AddRange(new object[] { "Port 1", "Port 2" });
            _comboPort.SelectedIndex = 0;
            _comboPort.SelectedIndexChanged += (s, e) => { UpdateUI(); };
            flowOps.Controls.Add(_comboPort);

            _btnPlace = CreateBtn("放置載具", IndTheme.StatusGreen, BtnPlaceCarrier_Click);
            flowOps.Controls.Add(_btnPlace);

            _btnUnload = CreateBtn("卸載載具", IndTheme.StatusYellow, UnloadCarrier_Click);
            flowOps.Controls.Add(_btnUnload);

            _btnPJCJ = CreateBtn("建立工單", IndTheme.StatusBlue, BtnCreatePJCJ_Click);
            _btnPJCJ.Margin = new Padding(3, 20, 3, 3); // Spacer
            flowOps.Controls.Add(_btnPJCJ);

            _btnRecipes = CreateBtn("配方管理", IndTheme.StatusBlue, BtnManageRecipes_Click);
            flowOps.Controls.Add(_btnRecipes);

            _btnSecs = CreateBtn("SECS 監控", IndTheme.StatusBlue, btnSECS_Click);
            _btnSecs.Margin = new Padding(3, 20, 3, 3);
            flowOps.Controls.Add(_btnSecs);

            _btnEtel = CreateBtn("ETEL 驅動測試", IndTheme.StatusGray, BtnEtelTest_Click);
            flowOps.Controls.Add(_btnEtel);

            leftLayout.Controls.Add(secOps, 0, 1);


            // === RIGHT PANEL ===
            var rightPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 0, 0, 0) };
            layout.Controls.Add(rightPanel, 1, 0);

            var rightLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1
            };
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F)); // Slot Map
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F)); // PJ/CJ List
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35F)); // Log
            rightPanel.Controls.Add(rightLayout);

            // 3. Slot Map Section
            var secMap = new SectionPanel { Title = "晶圓狀態圖 (Wafer Slot Map)", Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 10) };
            var pnlMap = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15, 45, 15, 15), BackColor = Color.Transparent };
            secMap.Controls.Add(pnlMap);

            _gridSlots = new DataGridView();
            SetupDataGridView(_gridSlots);
            _gridSlots.Columns.Add("Slot", "Slot");
            _gridSlots.Columns.Add("ID", "Wafer ID");
            _gridSlots.Columns.Add("State", "狀態");
            _gridSlots.Columns[0].Width = 60;
            pnlMap.Controls.Add(_gridSlots);

            rightLayout.Controls.Add(secMap, 0, 0);

            // 4. PJ/CJ List Section
            var secJobs = new SectionPanel { Title = "工單列表 (Job List)", Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 10) };
            var pnlJobs = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15, 45, 15, 15), BackColor = Color.Transparent };
            secJobs.Controls.Add(pnlJobs);

            _gridJobs = new DataGridView();
            SetupDataGridView(_gridJobs);
            _gridJobs.Columns.Add("Type", "類型");
            _gridJobs.Columns.Add("JobID", "工單 ID");
            _gridJobs.Columns.Add("Recipe", "配方");
            _gridJobs.Columns.Add("State", "狀態");
            _gridJobs.Columns.Add("Wafers", "晶圓數");
            _gridJobs.Columns[0].Width = 50;
            _gridJobs.Columns[4].Width = 60;
            pnlJobs.Controls.Add(_gridJobs);

            rightLayout.Controls.Add(secJobs, 0, 1);

            // 5. Log Section
            var secLog = new SectionPanel { Title = "系統日誌 (System Log)", Dock = DockStyle.Fill };
            var pnlLog = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 40, 10, 10), BackColor = Color.Transparent };
            secLog.Controls.Add(pnlLog);

            _txtLog = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(12, 12, 16),
                ForeColor = IndTheme.TextSecondary,
                Font = IndTheme.MonoFont,
                BorderStyle = BorderStyle.None,
                ReadOnly = true
            };
            pnlLog.Controls.Add(_txtLog);

            rightLayout.Controls.Add(secLog, 0, 2);
        }

        private ActionButton CreateBtn(string text, Color color, EventHandler handler)
        {
            var btn = new ActionButton(text, color) { Width = 180, Height = 45, Margin = new Padding(3, 3, 3, 8) };
            btn.Click += handler;
            return btn;
        }

        private void SetupDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = IndTheme.BgCard;
            dgv.DefaultCellStyle.BackColor = IndTheme.BgCard;
            dgv.DefaultCellStyle.ForeColor = IndTheme.TextPrimary;
            dgv.DefaultCellStyle.SelectionBackColor = IndTheme.BgCardHover;
            dgv.DefaultCellStyle.SelectionForeColor = IndTheme.TextPrimary;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = IndTheme.BgSecondary;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = IndTheme.TextPrimary;
            dgv.EnableHeadersVisualStyles = false;
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = IndTheme.BorderColor;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.Dock = DockStyle.Fill;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeight = 35;
            dgv.RowTemplate.Height = 30;
        }

        private void SubscribeToEvents()
        {
            SystemEventBus.LogMessagePublished += (message) =>
            {
                if (_txtLog.InvokeRequired)
                    _txtLog.Invoke(new Action(() => { _txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n"); _txtLog.ScrollToCaret(); }));
                else
                {
                    _txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
                    _txtLog.ScrollToCaret();
                }
            };

            SystemEventBus.LoadPortStateChanged += (loadPort) =>
            {
                if (InvokeRequired) Invoke(new Action(UpdateUI)); else UpdateUI();
            };

            SystemEventBus.CarrierStateChanged += (carrier) =>
            {
                if (InvokeRequired) Invoke(new Action(UpdateUI)); else UpdateUI();
            };

            SystemEventBus.SubstrateStateChanged += (substrate) =>
            {
                if (InvokeRequired) Invoke(new Action(UpdateUI)); else UpdateUI();
            };
        }

        private void UpdateUI()
        {
            int p1Id = 1, p2Id = 2;

            // Port 1 Status
            var p1 = _equipment.LoadPorts[p1Id];
            _indPort1.SetStatus(GetLoadPortStateString(p1.State), GetStateColor(p1.State));

            // Port 2 Status
            var p2 = _equipment.LoadPorts[p2Id];
            _indPort2.SetStatus(GetLoadPortStateString(p2.State), GetStateColor(p2.State));

            // Buttons Logic
            int selectedPortId = GetSelectedPortId();
            var targetPort = _equipment.LoadPorts[selectedPortId];

            bool isPortEmpty = targetPort.State == LoadPortState.EMPTY;
            _btnPlace.Enabled = isPortEmpty;

            bool hasCarrier = targetPort.Carrier != null;
            _btnUnload.Enabled = hasCarrier && (targetPort.State == LoadPortState.READY_TO_UNLOAD || targetPort.State == LoadPortState.READY_TO_PROCESS);

            // Carrier ID Display
            if (targetPort.Carrier != null)
                _lblCarrierId.Text = $"當前載具: {targetPort.Carrier.Id}";
            else
                _lblCarrierId.Text = "當前載具: 無";

            UpdateSlotMapDisplay();
            UpdateJobsDisplay();
        }

        private void UpdateSlotMapDisplay()
        {
            _gridSlots.Rows.Clear();
            if (_comboPort.SelectedIndex >= 0)
            {
                int portId = GetSelectedPortId();
                var port = _equipment.LoadPorts[portId];
                if (port.Carrier != null)
                {
                    var sortedSlots = port.Carrier.SlotMap.Values.OrderBy(s => s.Slot);
                    foreach (var substrate in sortedSlots)
                    {
                        string stateStr = GetSubstrateStateString(substrate.State);
                        _gridSlots.Rows.Add(substrate.Slot, substrate.Id, stateStr);
                    }
                }
            }
        }

        private void UpdateJobsDisplay()
        {
            _gridJobs.Rows.Clear();

            // Show Control Jobs
            foreach (var cj in _equipment.ControlJobManager.ControlJobs)
            {
                string stateStr = GetControlJobStateString(cj.State);
                int waferCount = cj.ProcessJobs.Sum(pj => pj.SubstratesToProcess.Count);
                string recipeStr = string.Join(",", cj.ProcessJobs.Select(pj => pj.Recipe.Id).Distinct());
                _gridJobs.Rows.Add("CJ", cj.Id, recipeStr, stateStr, waferCount);
            }

            // Show Process Jobs (that might not be in any CJ)
            foreach (var pj in _equipment.ControlJobManager.ProcessJobs)
            {
                // Check if already shown via CJ
                bool inCJ = _equipment.ControlJobManager.ControlJobs.Any(cj => cj.ProcessJobs.Contains(pj));
                if (!inCJ)
                {
                    _gridJobs.Rows.Add("PJ", pj.Id, pj.Recipe.Id, "POOLED", pj.SubstratesToProcess.Count);
                }
            }
        }

        private string GetControlJobStateString(ControlJobState state)
        {
            return state switch
            {
                ControlJobState.QUEUED => "排隊中",
                ControlJobState.EXECUTING => "執行中",
                ControlJobState.COMPLETED => "已完成",
                ControlJobState.PAUSED => "已暫停",
                _ => state.ToString()
            };
        }

        private int GetSelectedPortId()
        {
            return _comboPort.SelectedIndex < 0 ? 1 : _comboPort.SelectedIndex + 1;
        }

        private Color GetStateColor(LoadPortState state)
        {
            return state switch
            {
                LoadPortState.READY_TO_PROCESS => IndTheme.StatusGreen,
                LoadPortState.READY_TO_UNLOAD => IndTheme.StatusYellow,
                LoadPortState.EMPTY => IndTheme.StatusGray,
                LoadPortState.CARRIER_PRESENT => IndTheme.StatusBlue,
                _ => IndTheme.StatusRed
            };
        }

        private string GetLoadPortStateString(LoadPortState state)
        {
            return state switch
            {
                LoadPortState.EMPTY => "閒置 (Empty)",
                LoadPortState.CARRIER_PRESENT => "載具在席 (Present)",
                LoadPortState.CLAMPED => "已夾持 (Clamped)",
                LoadPortState.DOOR_OPEN => "艙門開啟 (Open)",
                LoadPortState.MAPPING => "掃描中 (Mapping)",
                LoadPortState.READY_TO_PROCESS => "準備處理 (Ready)",
                LoadPortState.READY_TO_UNLOAD => "可卸載 (Unload)",
                _ => state.ToString()
            };
        }

        private string GetSubstrateStateString(SubstrateState state)
        {
            return state switch
            {
                SubstrateState.UNKNOWN => "未知",
                SubstrateState.EMPTY => "空",
                SubstrateState.PRESENT => "在席",
                SubstrateState.PICKED => "已取走",
                SubstrateState.PROCESSING => "處理中",
                SubstrateState.PROCESSED => "已完成",
                _ => state.ToString()
            };
        }

        private async void BtnPlaceCarrier_Click(object sender, EventArgs e)
        {
            int portId = GetSelectedPortId();
            var targetPort = _equipment.LoadPorts[portId];

            if (targetPort.State != LoadPortState.EMPTY)
            {
                MessageBox.Show($"Load Port {portId} 目前不是空白狀態 (Not EMPTY)。");
                return;
            }

            var carrier = new Carrier($"C{DateTime.Now.Ticks % 1000:000}");
            carrier.SlotMap[1] = new Substrate($"Wafer-{portId}-01", 1);
            carrier.SlotMap[2] = new Substrate($"Wafer-{portId}-02", 2);

            await targetPort.PlaceCarrier(carrier);
        }

        private void BtnCreatePJCJ_Click(object sender, EventArgs e)
        {
            using (var form = _serviceProvider.GetRequiredService<PJCJCreationForm>())
            {
                form.ShowDialog();
            }
            UpdateUI();
        }

        private void UnloadCarrier_Click(object sender, EventArgs e)
        {
            int portId = GetSelectedPortId();
            var targetPort = _equipment.LoadPorts[portId];

            if (targetPort.Carrier == null)
            {
                MessageBox.Show($"Port {portId} 上沒有 Carrier。");
                return;
            }

            targetPort.UnloadCarrier();
        }

        private void BtnManageRecipes_Click(object sender, EventArgs e)
        {
            try
            {
                var form = _serviceProvider.GetRequiredService<RecipeEditorForm>();
                form.ShowDialog();
            }
            catch
            {
                var form = new RecipeEditorForm(_equipment);
                form.ShowDialog();
            }
        }

        private void btnSECS_Click(object sender, EventArgs e)
        {
            _equipment.SecsManager.RequestShowMonitor();
        }

        private Forms.SecsMonitorForm? _monitorForm;
        private void ShowSecsMonitor()
        {
            if (_monitorForm == null || _monitorForm.IsDisposed)
            {
                _monitorForm = new Forms.SecsMonitorForm(_equipment.SecsManager);
            }
            _monitorForm.Show();
            _monitorForm.BringToFront();
        }

        private void BtnEtelTest_Click(object sender, EventArgs e)
        {
            var form = new WaferMeasurementFlow.Forms.EtelTestForm();
            form.Show();
        }
    }
}