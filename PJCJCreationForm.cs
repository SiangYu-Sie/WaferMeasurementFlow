using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaferMeasurementFlow.Managers;
using WaferMeasurementFlow.Models;
using WaferMeasurementFlow.UI;

namespace WaferMeasurementFlow
{
    public partial class PJCJCreationForm : Form
    {
        private readonly Equipment _equipment;

        // UI Controls
        private ComboBox _comboRecipes;
        private ComboBox _comboPorts;
        private DataGridView _gridWafers;
        private TextBox _txtPJCJId;
        private TextBox _txtCJCJId;
        private ActionButton _btnCreate;

        public PJCJCreationForm(Equipment equipment)
        {
            InitializeComponent();
            _equipment = equipment;

            BuildUI();

            LoadAvailableLoadPorts();
            LoadRecipes();
        }

        private void BuildUI()
        {
            // Theme Style
            this.BackColor = IndTheme.BgPrimary;
            this.ForeColor = IndTheme.TextPrimary;
            this.Font = IndTheme.BodyFont;
            this.Size = new Size(900, 600);
            this.Text = "建立控制工單 (Create Control Job)";

            // Layout
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10),
                BackColor = Color.Transparent
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F)); // Left: Inputs
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F)); // Right: Wafers
            this.Controls.Add(layout);

            // === Left Panel: Settings ===
            var secSettings = new SectionPanel { Title = "工單設定", Dock = DockStyle.Fill, Margin = new Padding(0, 0, 10, 0) };
            var pnlSettings = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15, 45, 15, 15), BackColor = Color.Transparent };
            secSettings.Controls.Add(pnlSettings);

            // Recipe
            pnlSettings.Controls.Add(CreateLabel("選擇配方 (Recipe):", 15, 50));
            _comboRecipes = CreateComboBox();
            _comboRecipes.Location = new Point(15, 75);
            _comboRecipes.Width = 300;
            _comboRecipes.SelectedIndexChanged += RecipeComboBox_SelectedIndexChanged;
            pnlSettings.Controls.Add(_comboRecipes);

            // Load Port
            pnlSettings.Controls.Add(CreateLabel("選擇裝載埠 (Load Port):", 15, 115));
            _comboPorts = CreateComboBox();
            _comboPorts.Location = new Point(15, 140);
            _comboPorts.Width = 300;
            _comboPorts.SelectedIndexChanged += LoadPortSelectionComboBox_SelectedIndexChanged;
            pnlSettings.Controls.Add(_comboPorts);

            // Job IDs
            pnlSettings.Controls.Add(CreateLabel("Process Job ID:", 15, 180));
            _txtPJCJId = CreateTextBox($"PJ_{DateTime.Now:MMddHHmm}");
            _txtPJCJId.Location = new Point(15, 205);
            _txtPJCJId.Width = 300;
            pnlSettings.Controls.Add(_txtPJCJId);

            pnlSettings.Controls.Add(CreateLabel("Control Job ID:", 15, 245));
            _txtCJCJId = CreateTextBox($"CJ_{DateTime.Now:MMddHHmm}");
            _txtCJCJId.Location = new Point(15, 270);
            _txtCJCJId.Width = 300;
            pnlSettings.Controls.Add(_txtCJCJId);

            // Create Button
            _btnCreate = new ActionButton("建立並執行工單", IndTheme.StatusBlue) { Location = new Point(15, 330), Width = 300, Height = 45 };
            _btnCreate.Click += BtnCreateAndStartJob_Click;
            pnlSettings.Controls.Add(_btnCreate);

            layout.Controls.Add(secSettings, 0, 0);

            // === Right Panel: Wafers ===
            var secWafers = new SectionPanel { Title = "晶圓選取", Dock = DockStyle.Fill };
            var pnlWafers = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 40, 10, 10), BackColor = Color.Transparent };
            secWafers.Controls.Add(pnlWafers);

            _gridWafers = new DataGridView();
            SetupDataGridView(_gridWafers);

            // Add CheckBox Column
            var chkCol = new DataGridViewCheckBoxColumn { Name = "Select", HeaderText = "選取", Width = 40 };
            _gridWafers.Columns.Add(chkCol);
            _gridWafers.Columns.Add("Slot", "Slot");
            _gridWafers.Columns.Add("ID", "Wafer ID");
            _gridWafers.Columns["Slot"].ReadOnly = true;
            _gridWafers.Columns["ID"].ReadOnly = true;

            pnlWafers.Controls.Add(_gridWafers);
            layout.Controls.Add(secWafers, 1, 0);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label { Text = text, AutoSize = true, Location = new Point(x, y), ForeColor = IndTheme.TextSecondary };
        }

        private TextBox CreateTextBox(string text)
        {
            return new TextBox { Text = text, BackColor = IndTheme.BgCardHover, ForeColor = IndTheme.TextPrimary, BorderStyle = BorderStyle.FixedSingle };
        }

        private ComboBox CreateComboBox()
        {
            return new ComboBox { BackColor = IndTheme.BgCardHover, ForeColor = IndTheme.TextPrimary, FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList };
        }

        private void SetupDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = IndTheme.BgCardHover;
            dgv.DefaultCellStyle.BackColor = IndTheme.BgCardHover;
            dgv.DefaultCellStyle.ForeColor = IndTheme.TextPrimary;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = IndTheme.BgSecondary;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = IndTheme.TextPrimary;
            dgv.EnableHeadersVisualStyles = false;
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = IndTheme.BorderColor;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = false; // Allow checking boxes
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.Dock = DockStyle.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeight = 30;
        }

        private void LoadRecipes()
        {
            _comboRecipes.Items.Clear();
            var recipes = _equipment.RecipeManager.GetAllRecipes();
            foreach (var recipe in recipes)
            {
                _comboRecipes.Items.Add(recipe.Id);
            }
        }

        private void RecipeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWafersToProcessList();
        }

        private void LoadAvailableLoadPorts()
        {
            _comboPorts.Items.Clear();
            foreach (var port in _equipment.LoadPorts.Values)
            {
                _comboPorts.Items.Add(port.Id);
            }
            if (_comboPorts.Items.Count > 0)
                _comboPorts.SelectedIndex = 0;
        }

        private void LoadPortSelectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWafersToProcessList();
        }

        private void UpdateWafersToProcessList()
        {
            _gridWafers.Rows.Clear();
            if (_comboPorts.SelectedItem is int selectedLoadPortId)
            {
                HashSet<int> targetSlots = null;
                if (_comboRecipes.SelectedItem is string recipeId)
                {
                    var recipe = _equipment.RecipeManager.GetRecipe(recipeId);
                    if (recipe != null && !string.IsNullOrWhiteSpace(recipe.TargetSlots))
                    {
                        targetSlots = ParseTargetSlots(recipe.TargetSlots);
                    }
                }

                if (_equipment.LoadPorts.ContainsKey(selectedLoadPortId))
                {
                    var port = _equipment.LoadPorts[selectedLoadPortId];
                    if (port.Carrier != null)
                    {
                        foreach (var substrate in port.Carrier.SlotMap.Values.OrderBy(s => s.Slot))
                        {
                            bool shouldCheck = targetSlots == null || targetSlots.Contains(substrate.Slot);
                            int rowIdx = _gridWafers.Rows.Add(shouldCheck, substrate.Slot, substrate.Id);
                        }
                    }
                }
            }
        }

        private HashSet<int> ParseTargetSlots(string slotsStr)
        {
            var slots = new HashSet<int>();
            if (slotsStr.Trim().ToUpper() == "ALL") return null;

            var parts = slotsStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (int.TryParse(part.Trim(), out int slot))
                {
                    slots.Add(slot);
                }
                else if (part.Contains("-"))
                {
                    var range = part.Split('-');
                    if (range.Length == 2 && int.TryParse(range[0], out int start) && int.TryParse(range[1], out int end))
                    {
                        for (int i = start; i <= end; i++) slots.Add(i);
                    }
                }
            }
            return slots;
        }

        private async void BtnCreateAndStartJob_Click(object sender, EventArgs e)
        {
            _btnCreate.Enabled = false;

            // Validation
            if (_comboPorts.SelectedItem == null)
            {
                MessageBox.Show("請選擇一個裝載埠 (Load Port)。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnCreate.Enabled = true;
                return;
            }
            if (string.IsNullOrWhiteSpace(_txtCJCJId.Text))
            {
                MessageBox.Show("Control Job ID 不能為空。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnCreate.Enabled = true;
                return;
            }
            if (string.IsNullOrWhiteSpace(_txtPJCJId.Text))
            {
                MessageBox.Show("Process Job ID 不能為空。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnCreate.Enabled = true;
                return;
            }

            if (!(_comboPorts.SelectedItem is int selectedPortId))
            {
                MessageBox.Show("請選擇一個裝載埠 (Load Port)。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnCreate.Enabled = true;
                return;
            }
            var targetPort = _equipment.LoadPorts[selectedPortId];

            var selectedWafers = new List<Substrate>();
            foreach (DataGridViewRow row in _gridWafers.Rows)
            {
                if (Convert.ToBoolean(row.Cells["Select"].Value))
                {
                    int slot = Convert.ToInt32(row.Cells["Slot"].Value);
                    if (targetPort.Carrier != null && targetPort.Carrier.SlotMap.TryGetValue(slot, out Substrate substrate))
                    {
                        selectedWafers.Add(substrate);
                    }
                }
            }

            if (!selectedWafers.Any())
            {
                MessageBox.Show("請至少選擇一個晶圓 (Wafer) 進行處理。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnCreate.Enabled = true;
                return;
            }

            string recipeId = _comboRecipes.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(recipeId))
            {
                MessageBox.Show("請選擇配方 (Recipe)。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnCreate.Enabled = true;
                return;
            }

            // Execute
            var processJob = _equipment.ControlJobManager.CreateProcessJob(
                _txtPJCJId.Text,
                recipeId,
                selectedWafers);

            var controlJob = _equipment.ControlJobManager.CreateControlJob(
                _txtCJCJId.Text,
                new List<ProcessJob> { processJob },
                targetPort.Carrier!);

            MessageBox.Show($"工單 '{controlJob.Id}' 已建立並排程。開始執行...", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);

            await _equipment.ControlJobManager.ExecuteControlJob(controlJob);

            MessageBox.Show($"工單 '{controlJob.Id}' 已完成。", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }
    }
}