namespace WaferMeasurementFlow
{
    partial class PJCJCreationForm
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
            this.mainPanel = new System.Windows.Forms.Panel();
            this.formSection = new System.Windows.Forms.Panel();
            this.waferSection = new System.Windows.Forms.Panel();
            this.waferSectionTitle = new System.Windows.Forms.Label();
            this._loadPortSelectionComboBox = new System.Windows.Forms.ComboBox();
            this._controlJobIdTextBox = new System.Windows.Forms.TextBox();
            this._processJobIdTextBox = new System.Windows.Forms.TextBox();
            this._recipeComboBox = new System.Windows.Forms.ComboBox();
            this._wafersToProcessCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this._btnCreateAndStartJob = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.headerPanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.formSection.SuspendLayout();
            this.waferSection.SuspendLayout();
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
            this.titleLabel.Text = "建立工單 (Process Job / Control Job)";
            // 
            // mainPanel
            // 
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Padding = new System.Windows.Forms.Padding(25);
            this.mainPanel.Controls.Add(this.formSection);
            this.mainPanel.Controls.Add(this.waferSection);
            // 
            // formSection
            // 
            this.formSection.BackColor = System.Drawing.Color.FromArgb(38, 40, 48);
            this.formSection.Dock = System.Windows.Forms.DockStyle.Top;
            this.formSection.Height = 220;
            this.formSection.Padding = new System.Windows.Forms.Padding(20);
            this.formSection.Controls.Add(this.label1);
            this.formSection.Controls.Add(this._loadPortSelectionComboBox);
            this.formSection.Controls.Add(this.label2);
            this.formSection.Controls.Add(this._controlJobIdTextBox);
            this.formSection.Controls.Add(this.label3);
            this.formSection.Controls.Add(this._processJobIdTextBox);
            this.formSection.Controls.Add(this.label4);
            this.formSection.Controls.Add(this._recipeComboBox);
            // 
            // waferSection
            // 
            this.waferSection.BackColor = System.Drawing.Color.FromArgb(38, 40, 48);
            this.waferSection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.waferSection.Padding = new System.Windows.Forms.Padding(20, 50, 20, 20);
            this.waferSection.Controls.Add(this.waferSectionTitle);
            this.waferSection.Controls.Add(this._wafersToProcessCheckedListBox);
            this.waferSection.Controls.Add(this._btnCreateAndStartJob);
            // 
            // waferSectionTitle
            // 
            this.waferSectionTitle.AutoSize = true;
            this.waferSectionTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.waferSectionTitle.ForeColor = System.Drawing.Color.FromArgb(240, 242, 245);
            this.waferSectionTitle.Location = new System.Drawing.Point(20, 15);
            this.waferSectionTitle.Text = "選擇晶圓";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(140, 145, 160);
            this.label1.Location = new System.Drawing.Point(20, 25);
            this.label1.Text = "選擇 Load Port";
            // 
            // _loadPortSelectionComboBox
            // 
            this._loadPortSelectionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._loadPortSelectionComboBox.Location = new System.Drawing.Point(160, 22);
            this._loadPortSelectionComboBox.Size = new System.Drawing.Size(280, 30);
            this._loadPortSelectionComboBox.SelectedIndexChanged += new System.EventHandler(this.LoadPortSelectionComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(140, 145, 160);
            this.label2.Location = new System.Drawing.Point(20, 65);
            this.label2.Text = "Control Job ID";
            // 
            // _controlJobIdTextBox
            // 
            this._controlJobIdTextBox.Location = new System.Drawing.Point(160, 62);
            this._controlJobIdTextBox.Size = new System.Drawing.Size(280, 30);
            this._controlJobIdTextBox.Text = "CJ-";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label3.ForeColor = System.Drawing.Color.FromArgb(140, 145, 160);
            this.label3.Location = new System.Drawing.Point(20, 105);
            this.label3.Text = "Process Job ID";
            // 
            // _processJobIdTextBox
            // 
            this._processJobIdTextBox.Location = new System.Drawing.Point(160, 102);
            this._processJobIdTextBox.Size = new System.Drawing.Size(280, 30);
            this._processJobIdTextBox.Text = "PJ-";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(140, 145, 160);
            this.label4.Location = new System.Drawing.Point(20, 145);
            this.label4.Text = "Recipe";
            // 
            // _recipeComboBox
            // 
            this._recipeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._recipeComboBox.Location = new System.Drawing.Point(160, 142);
            this._recipeComboBox.Size = new System.Drawing.Size(280, 30);
            // 
            // _wafersToProcessCheckedListBox
            // 
            this._wafersToProcessCheckedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this._wafersToProcessCheckedListBox.Location = new System.Drawing.Point(20, 50);
            this._wafersToProcessCheckedListBox.Size = new System.Drawing.Size(420, 250);
            // 
            // _btnCreateAndStartJob
            // 
            this._btnCreateAndStartJob.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnCreateAndStartJob.Font = new System.Drawing.Font("Segoe UI Semibold", 11F);
            this._btnCreateAndStartJob.Location = new System.Drawing.Point(260, 320);
            this._btnCreateAndStartJob.Size = new System.Drawing.Size(180, 45);
            this._btnCreateAndStartJob.Text = "建立並開始 Job";
            this._btnCreateAndStartJob.Click += new System.EventHandler(this.BtnCreateAndStartJob_Click);
            // 
            // PJCJCreationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(18, 18, 22);
            this.ClientSize = new System.Drawing.Size(520, 680);
            this.Controls.Add(this.waferSection);
            this.Controls.Add(this.formSection);
            this.Controls.Add(this.headerPanel);
            this.MinimumSize = new System.Drawing.Size(480, 600);
            this.Text = "建立工單";
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.formSection.ResumeLayout(false);
            this.formSection.PerformLayout();
            this.waferSection.ResumeLayout(false);
            this.waferSection.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        // Header
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label titleLabel;

        // Main
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel formSection;
        private System.Windows.Forms.Panel waferSection;
        private System.Windows.Forms.Label waferSectionTitle;

        // Controls
        private System.Windows.Forms.ComboBox _loadPortSelectionComboBox;
        private System.Windows.Forms.TextBox _controlJobIdTextBox;
        private System.Windows.Forms.TextBox _processJobIdTextBox;
        private System.Windows.Forms.ComboBox _recipeComboBox;
        private System.Windows.Forms.CheckedListBox _wafersToProcessCheckedListBox;
        private System.Windows.Forms.Button _btnCreateAndStartJob;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}