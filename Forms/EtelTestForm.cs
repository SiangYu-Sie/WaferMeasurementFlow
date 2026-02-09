using System;
using System.Drawing;
using System.Windows.Forms;
using WaferMeasurementFlow.Managers;
using WaferMeasurementFlow.UI;

namespace WaferMeasurementFlow.Forms
{
    public partial class EtelTestForm : Form
    {
        private EtelManager _etelManager;

        // UI Controls
        private TextBox txtUrl;
        private ActionButton btnConnect;
        private ActionButton btnDisconnect;

        private ComboBox comboType;
        private TextBox txtIndex;
        private TextBox txtSubIndex;
        private TextBox txtValue;
        private ActionButton btnRead;
        private ActionButton btnWrite;

        private RichTextBox txtLog;

        public EtelTestForm()
        {
            InitializeComponent();
            BuildUI();

            _etelManager = new EtelManager();
            _etelManager.OnLog += Log;
            _etelManager.OnError += LogError;

            UpdateUI(false);
        }

        private void BuildUI()
        {
            // Form Style
            this.BackColor = IndTheme.BgPrimary;
            this.ForeColor = IndTheme.TextPrimary;
            this.Font = IndTheme.BodyFont;
            this.Size = new Size(700, 750);
            this.Text = "ETEL 驅動測試程式";
            this.StartPosition = FormStartPosition.CenterScreen;

            // Container
            var mainContainer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            this.Controls.Add(mainContainer);

            // Layout
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                Padding = new Padding(0)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 140F)); // Connection
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F)); // RW
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Log
            mainContainer.Controls.Add(layout);

            // ========================================
            // 1. Connection Section
            // ========================================
            var secConn = new SectionPanel { Title = "連線設定", Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 10) };
            var pnlConn = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15, 45, 15, 15), BackColor = Color.Transparent };
            secConn.Controls.Add(pnlConn);

            // URL Row
            var lblUrl = CreateLabel("連線 URL:");
            lblUrl.Location = new Point(15, 55);
            pnlConn.Controls.Add(lblUrl);

            txtUrl = CreateTextBox("etcom://ETEL1/drive1");
            txtUrl.Location = new Point(90, 52);
            txtUrl.Width = 300;
            pnlConn.Controls.Add(txtUrl);

            // Buttons Row
            btnConnect = new ActionButton("連線", IndTheme.StatusBlue) { Width = 120, Height = 35 };
            btnConnect.Location = new Point(410, 48);
            btnConnect.Click += btnConnect_Click;
            pnlConn.Controls.Add(btnConnect);

            btnDisconnect = new ActionButton("斷線", IndTheme.StatusRed) { Width = 120, Height = 35 };
            btnDisconnect.Location = new Point(540, 48);
            btnDisconnect.Click += btnDisconnect_Click;
            pnlConn.Controls.Add(btnDisconnect);

            layout.Controls.Add(secConn, 0, 0);

            // ========================================
            // 2. Read/Write Section
            // ========================================
            var secCmd = new SectionPanel { Title = "參數讀寫", Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 10) };
            var pnlCmd = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15, 45, 15, 15), BackColor = Color.Transparent };
            secCmd.Controls.Add(pnlCmd);

            // Row 1: Type, Index, SubIndex
            int row1Y = 55;

            var lblType = CreateLabel("類型:");
            lblType.Location = new Point(15, row1Y);
            pnlCmd.Controls.Add(lblType);

            comboType = new ComboBox
            {
                BackColor = IndTheme.BgCard,
                ForeColor = IndTheme.TextPrimary,
                FlatStyle = FlatStyle.Flat,
                Width = 70,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = IndTheme.BodyFont
            };
            comboType.Items.AddRange(new object[] { "K", "M", "X", "Y", "L" });
            comboType.SelectedIndex = 0;
            comboType.Location = new Point(60, row1Y - 3);
            pnlCmd.Controls.Add(comboType);

            var lblIndex = CreateLabel("索引:");
            lblIndex.Location = new Point(150, row1Y);
            pnlCmd.Controls.Add(lblIndex);

            txtIndex = CreateTextBox("0");
            txtIndex.Location = new Point(195, row1Y - 3);
            txtIndex.Width = 80;
            pnlCmd.Controls.Add(txtIndex);

            var lblSubIndex = CreateLabel("子索引:");
            lblSubIndex.Location = new Point(295, row1Y);
            pnlCmd.Controls.Add(lblSubIndex);

            txtSubIndex = CreateTextBox("0");
            txtSubIndex.Location = new Point(355, row1Y - 3);
            txtSubIndex.Width = 80;
            pnlCmd.Controls.Add(txtSubIndex);

            // Row 2: Value
            int row2Y = 95;

            var lblValue = CreateLabel("數值:");
            lblValue.Location = new Point(15, row2Y);
            pnlCmd.Controls.Add(lblValue);

            txtValue = CreateTextBox("");
            txtValue.Location = new Point(60, row2Y - 3);
            txtValue.Width = 200;
            pnlCmd.Controls.Add(txtValue);

            // Row 3: Buttons
            int row3Y = 135;

            btnRead = new ActionButton("讀取參數", IndTheme.StatusGreen) { Width = 140, Height = 38 };
            btnRead.Location = new Point(15, row3Y);
            btnRead.Click += btnRead_Click;
            pnlCmd.Controls.Add(btnRead);

            btnWrite = new ActionButton("寫入參數", IndTheme.StatusYellow) { Width = 140, Height = 38 };
            btnWrite.Location = new Point(170, row3Y);
            btnWrite.Click += btnWrite_Click;
            pnlCmd.Controls.Add(btnWrite);

            layout.Controls.Add(secCmd, 0, 1);

            // ========================================
            // 3. Log Section
            // ========================================
            var secLog = new SectionPanel { Title = "系統日誌", Dock = DockStyle.Fill };
            var pnlLog = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 40, 10, 10), BackColor = Color.Transparent };
            secLog.Controls.Add(pnlLog);

            txtLog = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(12, 12, 16),
                ForeColor = IndTheme.TextSecondary,
                Font = IndTheme.MonoFont,
                BorderStyle = BorderStyle.None,
                ReadOnly = true
            };
            pnlLog.Controls.Add(txtLog);

            layout.Controls.Add(secLog, 0, 2);
        }

        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                ForeColor = IndTheme.TextSecondary,
                Font = IndTheme.BodyFont
            };
        }

        private TextBox CreateTextBox(string text)
        {
            return new TextBox
            {
                Text = text,
                BackColor = IndTheme.BgCardHover,
                ForeColor = IndTheme.TextPrimary,
                BorderStyle = BorderStyle.FixedSingle,
                Font = IndTheme.BodyFont
            };
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                _etelManager.Connect(txtUrl.Text);
                UpdateUI(true);
            }
            catch (Exception ex)
            {
                Log($"連線異常: {ex.Message}");
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            _etelManager.Disconnect();
            UpdateUI(false);
        }

        private void UpdateUI(bool connected)
        {
            btnConnect.Enabled = !connected;
            btnDisconnect.Enabled = connected;
            btnRead.Enabled = connected;
            btnWrite.Enabled = connected;
            txtUrl.Enabled = !connected;
        }

        private void Log(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(Log), message);
                return;
            }
            txtLog.AppendText($"[INFO] {DateTime.Now:HH:mm:ss}: {message}{Environment.NewLine}");
            txtLog.ScrollToCaret();
        }

        private void LogError(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(LogError), message);
                return;
            }
            if (!txtLog.IsDisposed)
            {
                txtLog.SelectionColor = IndTheme.StatusRed;
                txtLog.AppendText($"[ERROR] {DateTime.Now:HH:mm:ss}: {message}{Environment.NewLine}");
                txtLog.SelectionColor = IndTheme.TextSecondary;
                txtLog.ScrollToCaret();
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            try
            {
                string type = comboType.Text;
                if (string.IsNullOrEmpty(type)) type = "K";

                if (!int.TryParse(txtIndex.Text, out int index))
                {
                    LogError("無效索引");
                    return;
                }

                if (!int.TryParse(txtSubIndex.Text, out int subIndex))
                    subIndex = 0;

                long value = _etelManager.ReadParameter(type, index, subIndex);
                txtValue.Text = value.ToString();
                Log($"讀取結果: {value}");
            }
            catch (Exception ex)
            {
                LogError($"讀取失敗: {ex.Message}");
            }
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            try
            {
                string type = comboType.Text;
                if (!int.TryParse(txtIndex.Text, out int index))
                {
                    LogError("無效索引");
                    return;
                }
                if (!int.TryParse(txtSubIndex.Text, out int subIndex)) subIndex = 0;

                if (!long.TryParse(txtValue.Text, out long value))
                {
                    LogError("無效數值");
                    return;
                }

                _etelManager.WriteParameter(type, index, subIndex, value);
                Log("寫入指令已發送");
            }
            catch (Exception ex)
            {
                LogError($"寫入失敗: {ex.Message}");
            }
        }
    }
}
