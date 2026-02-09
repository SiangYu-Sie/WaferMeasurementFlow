
namespace WaferMeasurementFlow.Forms
{
    partial class EtelTestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelUrl = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.groupBoxRW = new System.Windows.Forms.GroupBox();
            this.labelValue = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.labelSubIndex = new System.Windows.Forms.Label();
            this.txtSubIndex = new System.Windows.Forms.TextBox();
            this.labelIndex = new System.Windows.Forms.Label();
            this.txtIndex = new System.Windows.Forms.TextBox();
            this.labelType = new System.Windows.Forms.Label();
            this.comboType = new System.Windows.Forms.ComboBox();
            this.btnWrite = new System.Windows.Forms.Button();
            this.btnRead = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.groupBoxRW.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelUrl
            // 
            this.labelUrl.AutoSize = true;
            this.labelUrl.Location = new System.Drawing.Point(13, 13);
            this.labelUrl.Name = "labelUrl";
            this.labelUrl.Size = new System.Drawing.Size(32, 12);
            this.labelUrl.TabIndex = 0;
            this.labelUrl.Text = "URL:";
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(51, 10);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(286, 22);
            this.txtUrl.TabIndex = 1;
            this.txtUrl.Text = "etcom://ETEL1/drive1";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(343, 8);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(424, 8);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(75, 23);
            this.btnDisconnect.TabIndex = 3;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // groupBoxRW
            // 
            this.groupBoxRW.Controls.Add(this.labelValue);
            this.groupBoxRW.Controls.Add(this.txtValue);
            this.groupBoxRW.Controls.Add(this.labelSubIndex);
            this.groupBoxRW.Controls.Add(this.txtSubIndex);
            this.groupBoxRW.Controls.Add(this.labelIndex);
            this.groupBoxRW.Controls.Add(this.txtIndex);
            this.groupBoxRW.Controls.Add(this.labelType);
            this.groupBoxRW.Controls.Add(this.comboType);
            this.groupBoxRW.Controls.Add(this.btnWrite);
            this.groupBoxRW.Controls.Add(this.btnRead);
            this.groupBoxRW.Location = new System.Drawing.Point(15, 39);
            this.groupBoxRW.Name = "groupBoxRW";
            this.groupBoxRW.Size = new System.Drawing.Size(484, 100);
            this.groupBoxRW.TabIndex = 4;
            this.groupBoxRW.TabStop = false;
            this.groupBoxRW.Text = "Read / Write Parameter";
            // 
            // labelValue
            // 
            this.labelValue.AutoSize = true;
            this.labelValue.Location = new System.Drawing.Point(300, 24);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(35, 12);
            this.labelValue.TabIndex = 9;
            this.labelValue.Text = "Value:";
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(341, 21);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(100, 22);
            this.txtValue.TabIndex = 8;
            // 
            // labelSubIndex
            // 
            this.labelSubIndex.AutoSize = true;
            this.labelSubIndex.Location = new System.Drawing.Point(194, 24);
            this.labelSubIndex.Name = "labelSubIndex";
            this.labelSubIndex.Size = new System.Drawing.Size(51, 12);
            this.labelSubIndex.TabIndex = 7;
            this.labelSubIndex.Text = "SubIndex:";
            // 
            // txtSubIndex
            // 
            this.txtSubIndex.Location = new System.Drawing.Point(251, 21);
            this.txtSubIndex.Name = "txtSubIndex";
            this.txtSubIndex.Size = new System.Drawing.Size(43, 22);
            this.txtSubIndex.TabIndex = 6;
            this.txtSubIndex.Text = "0";
            // 
            // labelIndex
            // 
            this.labelIndex.AutoSize = true;
            this.labelIndex.Location = new System.Drawing.Point(107, 24);
            this.labelIndex.Name = "labelIndex";
            this.labelIndex.Size = new System.Drawing.Size(35, 12);
            this.labelIndex.TabIndex = 5;
            this.labelIndex.Text = "Index:";
            // 
            // txtIndex
            // 
            this.txtIndex.Location = new System.Drawing.Point(148, 21);
            this.txtIndex.Name = "txtIndex";
            this.txtIndex.Size = new System.Drawing.Size(40, 22);
            this.txtIndex.TabIndex = 4;
            this.txtIndex.Text = "0";
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Location = new System.Drawing.Point(7, 24);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(32, 12);
            this.labelType.TabIndex = 3;
            this.labelType.Text = "Type:";
            // 
            // comboType
            // 
            this.comboType.FormattingEnabled = true;
            this.comboType.Items.AddRange(new object[] {
            "K",
            "M",
            "X",
            "Y",
            "L"});
            this.comboType.Location = new System.Drawing.Point(45, 21);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(56, 20);
            this.comboType.TabIndex = 2;
            this.comboType.Text = "K";
            // 
            // btnWrite
            // 
            this.btnWrite.Location = new System.Drawing.Point(366, 60);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(75, 23);
            this.btnWrite.TabIndex = 1;
            this.btnWrite.Text = "Write";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(285, 60);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(75, 23);
            this.btnRead.TabIndex = 0;
            this.btnRead.Text = "Read";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(15, 145);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(484, 293);
            this.txtLog.TabIndex = 5;
            // 
            // EtelTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 450);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.groupBoxRW);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.labelUrl);
            this.Name = "EtelTestForm";
            this.Text = "ETEL Driver Test";
            this.groupBoxRW.ResumeLayout(false);
            this.groupBoxRW.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelUrl;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.GroupBox groupBoxRW;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.ComboBox comboType;
        private System.Windows.Forms.Label labelIndex;
        private System.Windows.Forms.TextBox txtIndex;
        private System.Windows.Forms.Label labelSubIndex;
        private System.Windows.Forms.TextBox txtSubIndex;
        private System.Windows.Forms.Label labelValue;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.TextBox txtLog;
    }
}
