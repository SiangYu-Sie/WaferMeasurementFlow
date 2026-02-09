namespace WaferMeasurementFlow
{
    partial class RecipeEditorForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.headerPanel = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.leftSection = new System.Windows.Forms.Panel();
            this.leftSectionTitle = new System.Windows.Forms.Label();
            this._recipeListBox = new System.Windows.Forms.ListBox();
            this.leftButtonPanel = new System.Windows.Forms.Panel();
            this._btnNew = new System.Windows.Forms.Button();
            this._btnDelete = new System.Windows.Forms.Button();
            this.rightSection = new System.Windows.Forms.Panel();
            this.rightSectionTitle = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._recipeIdTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this._txtTargetSlots = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._parametersGridView = new System.Windows.Forms.DataGridView();
            this.ParamName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParamValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._btnSave = new System.Windows.Forms.Button();
            this.headerPanel.SuspendLayout();
            this.mainLayout.SuspendLayout();
            this.leftSection.SuspendLayout();
            this.leftButtonPanel.SuspendLayout();
            this.rightSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._parametersGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(28, 28, 35);
            this.headerPanel.Controls.Add(this.titleLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Height = 70;
            this.headerPanel.Padding = new System.Windows.Forms.Padding(25, 20, 25, 20);
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 14F);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(240, 242, 245);
            this.titleLabel.Location = new System.Drawing.Point(25, 22);
            this.titleLabel.Text = "Recipe 編輯器";
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 2;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 280F));
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.leftSection, 0, 0);
            this.mainLayout.Controls.Add(this.rightSection, 1, 0);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Padding = new System.Windows.Forms.Padding(15);
            this.mainLayout.RowCount = 1;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            // 
            // leftSection
            // 
            this.leftSection.BackColor = System.Drawing.Color.FromArgb(38, 40, 48);
            this.leftSection.Controls.Add(this.leftSectionTitle);
            this.leftSection.Controls.Add(this._recipeListBox);
            this.leftSection.Controls.Add(this.leftButtonPanel);
            this.leftSection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftSection.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.leftSection.Padding = new System.Windows.Forms.Padding(15, 45, 15, 15);
            // 
            // leftSectionTitle
            // 
            this.leftSectionTitle.AutoSize = true;
            this.leftSectionTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.leftSectionTitle.ForeColor = System.Drawing.Color.FromArgb(240, 242, 245);
            this.leftSectionTitle.Location = new System.Drawing.Point(15, 15);
            this.leftSectionTitle.Text = "Recipe 清單";
            // 
            // _recipeListBox
            // 
            this._recipeListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this._recipeListBox.Location = new System.Drawing.Point(15, 50);
            this._recipeListBox.Size = new System.Drawing.Size(230, 320);
            this._recipeListBox.SelectedIndexChanged += RecipeListBox_SelectedIndexChanged;
            // 
            // leftButtonPanel
            // 
            this.leftButtonPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.leftButtonPanel.Controls.Add(this._btnNew);
            this.leftButtonPanel.Controls.Add(this._btnDelete);
            this.leftButtonPanel.Location = new System.Drawing.Point(15, 380);
            this.leftButtonPanel.Size = new System.Drawing.Size(230, 45);
            // 
            // _btnNew
            // 
            this._btnNew.Location = new System.Drawing.Point(0, 0);
            this._btnNew.Size = new System.Drawing.Size(105, 40);
            this._btnNew.Text = "新增";
            this._btnNew.Click += BtnNew_Click;
            // 
            // _btnDelete
            // 
            this._btnDelete.Location = new System.Drawing.Point(115, 0);
            this._btnDelete.Size = new System.Drawing.Size(105, 40);
            this._btnDelete.Text = "刪除";
            this._btnDelete.Click += BtnDelete_Click;
            // 
            // rightSection
            // 
            this.rightSection.BackColor = System.Drawing.Color.FromArgb(38, 40, 48);
            this.rightSection.Controls.Add(this.rightSectionTitle);
            this.rightSection.Controls.Add(this.label2);
            this.rightSection.Controls.Add(this._recipeIdTextBox);
            this.rightSection.Controls.Add(this.label4);
            this.rightSection.Controls.Add(this._txtTargetSlots);
            this.rightSection.Controls.Add(this.label3);
            this.rightSection.Controls.Add(this._parametersGridView);
            this.rightSection.Controls.Add(this._btnSave);
            this.rightSection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightSection.Padding = new System.Windows.Forms.Padding(15, 45, 15, 15);
            // 
            // rightSectionTitle
            // 
            this.rightSectionTitle.AutoSize = true;
            this.rightSectionTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.rightSectionTitle.ForeColor = System.Drawing.Color.FromArgb(240, 242, 245);
            this.rightSectionTitle.Location = new System.Drawing.Point(15, 15);
            this.rightSectionTitle.Text = "Recipe 詳情";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(140, 145, 160);
            this.label2.Location = new System.Drawing.Point(15, 55);
            this.label2.Text = "Recipe ID";
            // 
            // _recipeIdTextBox
            // 
            this._recipeIdTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this._recipeIdTextBox.Location = new System.Drawing.Point(120, 52);
            this._recipeIdTextBox.Size = new System.Drawing.Size(350, 28);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(140, 145, 160);
            this.label4.Location = new System.Drawing.Point(15, 95);
            this.label4.Text = "Target Slots";
            // 
            // _txtTargetSlots
            // 
            this._txtTargetSlots.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this._txtTargetSlots.Location = new System.Drawing.Point(120, 92);
            this._txtTargetSlots.Size = new System.Drawing.Size(350, 28);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(140, 145, 160);
            this.label3.Location = new System.Drawing.Point(15, 135);
            this.label3.Text = "Parameters";
            // 
            // _parametersGridView
            // 
            this._parametersGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this._parametersGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._parametersGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { this.ParamName, this.ParamValue });
            this._parametersGridView.Location = new System.Drawing.Point(15, 160);
            this._parametersGridView.Size = new System.Drawing.Size(455, 210);
            // 
            // ParamName
            // 
            this.ParamName.HeaderText = "參數名稱";
            this.ParamName.Width = 180;
            // 
            // ParamValue
            // 
            this.ParamValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ParamValue.HeaderText = "值";
            // 
            // _btnSave
            // 
            this._btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnSave.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this._btnSave.Location = new System.Drawing.Point(350, 385);
            this._btnSave.Size = new System.Drawing.Size(120, 40);
            this._btnSave.Text = "儲存";
            this._btnSave.Click += BtnSave_Click;
            // 
            // RecipeEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(18, 18, 22);
            this.ClientSize = new System.Drawing.Size(850, 550);
            this.Controls.Add(this.mainLayout);
            this.Controls.Add(this.headerPanel);
            this.MinimumSize = new System.Drawing.Size(750, 500);
            this.Text = "Recipe 編輯器";
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.mainLayout.ResumeLayout(false);
            this.leftSection.ResumeLayout(false);
            this.leftSection.PerformLayout();
            this.leftButtonPanel.ResumeLayout(false);
            this.rightSection.ResumeLayout(false);
            this.rightSection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._parametersGridView)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        // Header
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label titleLabel;

        // Layout
        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.Panel leftSection;
        private System.Windows.Forms.Panel rightSection;
        private System.Windows.Forms.Label leftSectionTitle;
        private System.Windows.Forms.Label rightSectionTitle;

        // Left Panel
        private System.Windows.Forms.ListBox _recipeListBox;
        private System.Windows.Forms.Panel leftButtonPanel;
        private System.Windows.Forms.Button _btnNew;
        private System.Windows.Forms.Button _btnDelete;

        // Right Panel
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox _recipeIdTextBox;
        private System.Windows.Forms.TextBox _txtTargetSlots;
        private System.Windows.Forms.DataGridView _parametersGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParamName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParamValue;
        private System.Windows.Forms.Button _btnSave;
    }
}
