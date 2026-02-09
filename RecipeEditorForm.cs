using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WaferMeasurementFlow.Managers;
using WaferMeasurementFlow.Models;
using WaferMeasurementFlow.UI;

namespace WaferMeasurementFlow
{
    public partial class RecipeEditorForm : Form
    {
        private readonly Equipment _equipment;

        // UI Controls
        private ListBox _recipeListBox;
        private TextBox _recipeIdTextBox;
        private DataGridView _parametersGridView;
        private TextBox _txtTargetSlots;

        private ActionButton _btnNew;
        private ActionButton _btnSave;
        private ActionButton _btnDelete;

        public RecipeEditorForm(Equipment equipment)
        {
            InitializeComponent();
            _equipment = equipment;

            BuildUI();
            LoadRecipes();
        }

        private void BuildUI()
        {
            this.BackColor = IndTheme.BgPrimary;
            this.ForeColor = IndTheme.TextPrimary;
            this.Font = IndTheme.BodyFont;
            this.Size = new Size(800, 600);
            this.Text = "Recipe Manager";

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10),
                BackColor = Color.Transparent
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            this.Controls.Add(layout);

            // === Left: List ===
            var secList = new SectionPanel { Title = "Recipe List", Dock = DockStyle.Fill, Margin = new Padding(0, 0, 10, 0) };
            var pnlList = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 40, 10, 10), BackColor = Color.Transparent };
            secList.Controls.Add(pnlList);

            _recipeListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = IndTheme.BgCardHover,
                ForeColor = IndTheme.TextPrimary,
                BorderStyle = BorderStyle.None,
                Font = IndTheme.BodyFont
            };
            _recipeListBox.SelectedIndexChanged += RecipeListBox_SelectedIndexChanged;
            pnlList.Controls.Add(_recipeListBox);

            layout.Controls.Add(secList, 0, 0);

            // === Right: Details ===
            var rightPanel = new Panel { Dock = DockStyle.Fill };
            layout.Controls.Add(rightPanel, 1, 0);

            var secDetails = new SectionPanel { Title = "Recipe Details", Dock = DockStyle.Fill };
            var pnlDetails = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15, 45, 15, 15), BackColor = Color.Transparent };
            secDetails.Controls.Add(pnlDetails);
            rightPanel.Controls.Add(secDetails);

            // ID
            pnlDetails.Controls.Add(CreateLabel("Recipe ID:", 15, 50));
            _recipeIdTextBox = CreateTextBox();
            _recipeIdTextBox.Location = new Point(100, 47);
            _recipeIdTextBox.Width = 200;
            pnlDetails.Controls.Add(_recipeIdTextBox);

            // Slots
            pnlDetails.Controls.Add(CreateLabel("Target Slots:", 15, 85));
            _txtTargetSlots = CreateTextBox();
            _txtTargetSlots.Location = new Point(100, 82);
            _txtTargetSlots.Width = 200;
            pnlDetails.Controls.Add(_txtTargetSlots);

            pnlDetails.Controls.Add(CreateLabel("(e.g., '1,2,5' or '1-5')", 310, 85));

            // Grid
            pnlDetails.Controls.Add(CreateLabel("Parameters:", 15, 120));
            _parametersGridView = new DataGridView();
            SetupDataGridView(_parametersGridView);
            _parametersGridView.Location = new Point(15, 145);
            _parametersGridView.Size = new Size(450, 300);
            _parametersGridView.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            _parametersGridView.AllowUserToAddRows = true; // Allow adding params
            _parametersGridView.AllowUserToResizeRows = false;
            _parametersGridView.ReadOnly = false;

            _parametersGridView.Columns.Add("Key", "Parameter Name");
            _parametersGridView.Columns.Add("Value", "Value");
            pnlDetails.Controls.Add(_parametersGridView);

            // Buttons
            _btnNew = new ActionButton("New", IndTheme.StatusGreen) { Location = new Point(15, 460), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            _btnNew.Click += BtnNew_Click;
            pnlDetails.Controls.Add(_btnNew);

            _btnSave = new ActionButton("Save", IndTheme.StatusBlue) { Location = new Point(125, 460), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            _btnSave.Click += BtnSave_Click;
            pnlDetails.Controls.Add(_btnSave);

            _btnDelete = new ActionButton("Delete", IndTheme.StatusRed) { Location = new Point(235, 460), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            _btnDelete.Click += BtnDelete_Click;
            pnlDetails.Controls.Add(_btnDelete);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label { Text = text, AutoSize = true, Location = new Point(x, y), ForeColor = IndTheme.TextSecondary };
        }

        private TextBox CreateTextBox()
        {
            return new TextBox { BackColor = IndTheme.BgCardHover, ForeColor = IndTheme.TextPrimary, BorderStyle = BorderStyle.FixedSingle };
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
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeight = 30;
        }

        private void LoadRecipes()
        {
            _recipeListBox.Items.Clear();
            var recipes = _equipment.RecipeManager.GetAllRecipes();
            foreach (var recipe in recipes)
            {
                _recipeListBox.Items.Add(recipe.Id);
            }
        }

        private void RecipeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_recipeListBox.SelectedItem is string recipeId)
            {
                var recipe = _equipment.RecipeManager.GetRecipe(recipeId);
                if (recipe != null)
                {
                    _recipeIdTextBox.Text = recipe.Id;
                    _recipeIdTextBox.ReadOnly = true;

                    _parametersGridView.Rows.Clear();
                    foreach (var param in recipe.Parameters)
                    {
                        _parametersGridView.Rows.Add(param.Key, param.Value);
                    }
                    _txtTargetSlots.Text = recipe.TargetSlots;
                }
            }
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            _recipeListBox.SelectedIndex = -1;
            _recipeIdTextBox.Text = "";
            _recipeIdTextBox.ReadOnly = false;
            _parametersGridView.Rows.Clear();
            _txtTargetSlots.Text = "";
            _recipeIdTextBox.Focus();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var id = _recipeIdTextBox.Text.Trim();
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Recipe ID 不能為空。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var recipe = new Recipe(id);
            recipe.TargetSlots = _txtTargetSlots.Text;

            foreach (DataGridViewRow row in _parametersGridView.Rows)
            {
                if (row.IsNewRow) continue;
                var key = row.Cells[0].Value?.ToString();
                var value = row.Cells[1].Value?.ToString();

                if (!string.IsNullOrWhiteSpace(key))
                {
                    recipe.Parameters[key!] = value ?? "";
                }
            }

            _equipment.RecipeManager.SaveRecipe(recipe);
            MessageBox.Show($"Recipe '{id}' 已儲存。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadRecipes();
            _recipeListBox.SelectedItem = id;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            var id = _recipeIdTextBox.Text.Trim();
            if (string.IsNullOrEmpty(id)) return;

            if (MessageBox.Show($"確定要刪除 Recipe '{id}'?", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _equipment.RecipeManager.DeleteRecipe(id);
                LoadRecipes();
                _recipeIdTextBox.Text = "";
                _parametersGridView.Rows.Clear();
            }
        }
    }
}
