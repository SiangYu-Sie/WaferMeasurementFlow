namespace WaferMeasurementFlow
{
    partial class MainForm
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
            this.subtitleLabel = new System.Windows.Forms.Label();
            this.mainLayoutTable = new System.Windows.Forms.TableLayoutPanel();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.actionSection = new System.Windows.Forms.Panel();
            this._comboPortSelect = new System.Windows.Forms.ComboBox();
            this.lblPortSelect = new System.Windows.Forms.Label();
            this._btnPlaceCarrier = new System.Windows.Forms.Button();
            this._btnUnloadCarrier = new System.Windows.Forms.Button();
            this._btnCreatePJCJ = new System.Windows.Forms.Button();
            this._btnManageRecipes = new System.Windows.Forms.Button();
            this._btnExecuteJob = new System.Windows.Forms.Button();
            this._btnSecsMonitor = new System.Windows.Forms.Button();
            this.rightPanelTable = new System.Windows.Forms.TableLayoutPanel();
            this.statusSection = new System.Windows.Forms.Panel();
            this._lblLoadPortState = new System.Windows.Forms.Label();
            this._lblLoadPort2State = new System.Windows.Forms.Label();
            this._lblCarrierId = new System.Windows.Forms.Label();
            this.slotMapSection = new System.Windows.Forms.Panel();
            this._slotMapGridView = new System.Windows.Forms.DataGridView();
            this.logSection = new System.Windows.Forms.Panel();
            this._logListBox = new System.Windows.Forms.ListBox();
            this.headerPanel.SuspendLayout();
            this.mainLayoutTable.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.actionSection.SuspendLayout();
            this.rightPanelTable.SuspendLayout();
            this.statusSection.SuspendLayout();
            this.slotMapSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._slotMapGridView)).BeginInit();
            this.logSection.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(28, 28, 35);
            this.headerPanel.Controls.Add(this.titleLabel);
            this.headerPanel.Controls.Add(this.subtitleLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Height = 80;
            this.headerPanel.Padding = new System.Windows.Forms.Padding(25, 15, 25, 15);
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 18F);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(240, 242, 245);
            this.titleLabel.Location = new System.Drawing.Point(25, 15);
            this.titleLabel.Text = "晶圓量測系統";
            // 
            // subtitleLabel
            // 
            this.subtitleLabel.AutoSize = true;
            this.subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(140, 145, 160);
            this.subtitleLabel.Location = new System.Drawing.Point(25, 50);
            this.subtitleLabel.Text = "Wafer Measurement Control System v2.0";
            // 
            // mainLayoutTable
            // 
            this.mainLayoutTable.ColumnCount = 2;
            this.mainLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 280F));
            this.mainLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayoutTable.Controls.Add(this.leftPanel, 0, 0);
            this.mainLayoutTable.Controls.Add(this.rightPanelTable, 1, 0);
            this.mainLayoutTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayoutTable.Padding = new System.Windows.Forms.Padding(15);
            this.mainLayoutTable.RowCount = 1;
            this.mainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(this.actionSection);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftPanel.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            // 
            // actionSection
            // 
            this.actionSection.BackColor = System.Drawing.Color.FromArgb(38, 40, 48);
            this.actionSection.Controls.Add(this.lblPortSelect);
            this.actionSection.Controls.Add(this._comboPortSelect);
            this.actionSection.Controls.Add(this._btnPlaceCarrier);
            this.actionSection.Controls.Add(this._btnUnloadCarrier);
            this.actionSection.Controls.Add(this._btnCreatePJCJ);
            this.actionSection.Controls.Add(this._btnManageRecipes);
            this.actionSection.Controls.Add(this._btnExecuteJob);
            this.actionSection.Controls.Add(this._btnSecsMonitor);
            this.actionSection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionSection.Padding = new System.Windows.Forms.Padding(20, 50, 20, 20);
            // 
            // lblPortSelect
            // 
            this.lblPortSelect.AutoSize = true;
            this.lblPortSelect.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.lblPortSelect.ForeColor = System.Drawing.Color.FromArgb(240, 242, 245);
            this.lblPortSelect.Location = new System.Drawing.Point(20, 15);
            this.lblPortSelect.Text = "操作面板";
            // 
            // _comboPortSelect
            // 
            this._comboPortSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._comboPortSelect.Items.AddRange(new object[] { "Load Port 1", "Load Port 2" });
            this._comboPortSelect.Location = new System.Drawing.Point(20, 55);
            this._comboPortSelect.Size = new System.Drawing.Size(220, 30);
            // 
            // _btnPlaceCarrier
            // 
            this._btnPlaceCarrier.Location = new System.Drawing.Point(20, 100);
            this._btnPlaceCarrier.Size = new System.Drawing.Size(220, 40);
            this._btnPlaceCarrier.Text = "放置 Carrier";
            this._btnPlaceCarrier.Click += new System.EventHandler(this.BtnPlaceCarrier_Click);
            // 
            // _btnUnloadCarrier
            // 
            this._btnUnloadCarrier.Location = new System.Drawing.Point(20, 150);
            this._btnUnloadCarrier.Size = new System.Drawing.Size(220, 40);
            this._btnUnloadCarrier.Text = "卸載 Carrier";
            this._btnUnloadCarrier.Click += new System.EventHandler(this.UnloadCarrier_Click);
            // 
            // _btnCreatePJCJ
            // 
            this._btnCreatePJCJ.Location = new System.Drawing.Point(20, 220);
            this._btnCreatePJCJ.Size = new System.Drawing.Size(220, 40);
            this._btnCreatePJCJ.Text = "建立新工單 (Job)...";
            this._btnCreatePJCJ.Click += new System.EventHandler(this.BtnCreatePJCJ_Click);
            // 
            // _btnManageRecipes
            // 
            this._btnManageRecipes.Location = new System.Drawing.Point(20, 270);
            this._btnManageRecipes.Size = new System.Drawing.Size(220, 40);
            this._btnManageRecipes.Text = "管理 Recipe...";
            this._btnManageRecipes.Click += new System.EventHandler(this.BtnManageRecipes_Click);
            // 
            // _btnExecuteJob
            // 
            this._btnExecuteJob.Location = new System.Drawing.Point(20, 340);
            this._btnExecuteJob.Size = new System.Drawing.Size(220, 40);
            this._btnExecuteJob.Text = "執行工單";
            this._btnExecuteJob.Click += new System.EventHandler(this.BtnExecuteJob_Click);
            // 
            // _btnSecsMonitor
            // 
            this._btnSecsMonitor.Location = new System.Drawing.Point(20, 410);
            this._btnSecsMonitor.Size = new System.Drawing.Size(220, 40);
            this._btnSecsMonitor.Text = "SECS 監控";
            this._btnSecsMonitor.Click += new System.EventHandler(this.BtnSecsMonitor_Click);
            // 
            // rightPanelTable
            // 
            this.rightPanelTable.ColumnCount = 1;
            this.rightPanelTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightPanelTable.Controls.Add(this.statusSection, 0, 0);
            this.rightPanelTable.Controls.Add(this.slotMapSection, 0, 1);
            this.rightPanelTable.Controls.Add(this.logSection, 0, 2);
            this.rightPanelTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanelTable.RowCount = 3;
            this.rightPanelTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.rightPanelTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.rightPanelTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            // 
            // statusSection
            // 
            this.statusSection.BackColor = System.Drawing.Color.FromArgb(38, 40, 48);
            this.statusSection.Controls.Add(this._lblLoadPortState);
            this.statusSection.Controls.Add(this._lblLoadPort2State);
            this.statusSection.Controls.Add(this._lblCarrierId);
            this.statusSection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusSection.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.statusSection.Padding = new System.Windows.Forms.Padding(20, 50, 20, 20);
            // 
            // _lblLoadPortState
            // 
            this._lblLoadPortState.AutoSize = true;
            this._lblLoadPortState.Font = new System.Drawing.Font("Segoe UI Semibold", 11F);
            this._lblLoadPortState.ForeColor = System.Drawing.Color.FromArgb(240, 242, 245);
            this._lblLoadPortState.Location = new System.Drawing.Point(20, 55);
            this._lblLoadPortState.Text = "Load Port 1: 未知";
            // 
            // _lblLoadPort2State
            // 
            this._lblLoadPort2State.AutoSize = true;
            this._lblLoadPort2State.Font = new System.Drawing.Font("Segoe UI Semibold", 11F);
            this._lblLoadPort2State.ForeColor = System.Drawing.Color.FromArgb(240, 242, 245);
            this._lblLoadPort2State.Location = new System.Drawing.Point(20, 85);
            this._lblLoadPort2State.Text = "Load Port 2: 未知";
            // 
            // _lblCarrierId
            // 
            this._lblCarrierId.AutoSize = true;
            this._lblCarrierId.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this._lblCarrierId.ForeColor = System.Drawing.Color.FromArgb(59, 130, 246);
            this._lblCarrierId.Location = new System.Drawing.Point(350, 55);
            this._lblCarrierId.Text = "Carrier ID: --";
            // 
            // slotMapSection
            // 
            this.slotMapSection.BackColor = System.Drawing.Color.FromArgb(38, 40, 48);
            this.slotMapSection.Controls.Add(this._slotMapGridView);
            this.slotMapSection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slotMapSection.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.slotMapSection.Padding = new System.Windows.Forms.Padding(15, 45, 15, 15);
            // 
            // _slotMapGridView
            // 
            this._slotMapGridView.AllowUserToAddRows = false;
            this._slotMapGridView.AllowUserToDeleteRows = false;
            this._slotMapGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._slotMapGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                new System.Windows.Forms.DataGridViewTextBoxColumn() { HeaderText = "Slot", Name = "Slot", Width = 60 },
                new System.Windows.Forms.DataGridViewTextBoxColumn() { HeaderText = "Wafer ID", Name = "WaferID", Width = 180 },
                new System.Windows.Forms.DataGridViewTextBoxColumn() { HeaderText = "狀態", Name = "Status", AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill }
            });
            this._slotMapGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._slotMapGridView.ReadOnly = true;
            // 
            // logSection
            // 
            this.logSection.BackColor = System.Drawing.Color.FromArgb(38, 40, 48);
            this.logSection.Controls.Add(this._logListBox);
            this.logSection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logSection.Padding = new System.Windows.Forms.Padding(15, 45, 15, 15);
            // 
            // _logListBox
            // 
            this._logListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._logListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._logListBox.Font = new System.Drawing.Font("Consolas", 9.5F);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(18, 18, 22);
            this.ClientSize = new System.Drawing.Size(1280, 850);
            this.Controls.Add(this.mainLayoutTable);
            this.Controls.Add(this.headerPanel);
            this.MinimumSize = new System.Drawing.Size(1000, 700);
            this.Text = "晶圓量測系統 - 控制中心";
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.mainLayoutTable.ResumeLayout(false);
            this.leftPanel.ResumeLayout(false);
            this.actionSection.ResumeLayout(false);
            this.actionSection.PerformLayout();
            this.rightPanelTable.ResumeLayout(false);
            this.statusSection.ResumeLayout(false);
            this.statusSection.PerformLayout();
            this.slotMapSection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._slotMapGridView)).EndInit();
            this.logSection.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        // Header
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label subtitleLabel;

        // Main Layout
        private System.Windows.Forms.TableLayoutPanel mainLayoutTable;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.TableLayoutPanel rightPanelTable;

        // Sections
        private System.Windows.Forms.Panel actionSection;
        private System.Windows.Forms.Panel statusSection;
        private System.Windows.Forms.Panel slotMapSection;
        private System.Windows.Forms.Panel logSection;

        // Controls
        private System.Windows.Forms.Label lblPortSelect;
        private System.Windows.Forms.ComboBox _comboPortSelect;
        private System.Windows.Forms.Button _btnPlaceCarrier;
        private System.Windows.Forms.Button _btnUnloadCarrier;
        private System.Windows.Forms.Button _btnCreatePJCJ;
        private System.Windows.Forms.Button _btnExecuteJob;
        private System.Windows.Forms.Button _btnSecsMonitor;
        private System.Windows.Forms.Button _btnManageRecipes;
        private System.Windows.Forms.Label _lblLoadPortState;
        private System.Windows.Forms.Label _lblLoadPort2State;
        private System.Windows.Forms.Label _lblCarrierId;
        private System.Windows.Forms.DataGridView _slotMapGridView;
        private System.Windows.Forms.ListBox _logListBox;
    }
}