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
            this.Size = new Size(600, 700);
            this.Text = "ETEL 驅動測試程式 (ETEL Driver Test)";

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
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 130F)); // Connection
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F)); // RW
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Log
            mainContainer.Controls.Add(layout);

            // 1. Connection Section
            var secConn = new SectionPanel { Title = "連線設定 (Connection)", Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 10) };
            var pnlConn = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15, 45, 15, 15), BackColor = Color.Transparent };
            secConn.Controls.Add(pnlConn);

            pnlConn.Controls.Add(SetLoc(CreateLabel("URL:"), 15, 55));

            txtUrl = CreateTextBox("etcom://ETEL1/drive1");
            txtUrl.Location = new Point(55, 52);
            txtUrl.Width = 250;
            pnlConn.Controls.Add(txtUrl);

            btnConnect = new ActionButton("連線 (Connect)", IndTheme.StatusBlue);
            btnConnect.Location = new Point(320, 45);
            btnConnect.Click += btnConnect_Click;
            pnlConn.Controls.Add(btnConnect);

            btnDisconnect = new ActionButton("斷線 (Disconnect)", IndTheme.StatusRed);
            btnDisconnect.Location = new Point(430, 45);
            btnDisconnect.Click += btnDisconnect_Click;
            pnlConn.Controls.Add(btnDisconnect);

            layout.Controls.Add(secConn, 0, 0);

            // 2. Read/Write Section
            var secCmd = new SectionPanel { Title = "參數讀寫 (Read/Write)", Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 10) };
            var pnlCmd = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15, 45, 15, 15), BackColor = Color.Transparent };
            secCmd.Controls.Add(pnlCmd);

            int row1Y = 50;
            pnlCmd.Controls.Add(SetLoc(CreateLabel("類型 (Type):"), 15, row1Y + 3));

            comboType = new ComboBox
            {
                BackColor = IndTheme.BgCard,
                ForeColor = IndTheme.TextPrimary,
                FlatStyle = FlatStyle.Flat,
                Width = 50,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboType.Items.AddRange(new object[] { "K", "M", "X", "Y", "L" });
            comboType.Text = "K";
            comboType.Location = new Point(90, row1Y);
            pnlCmd.Controls.Add(comboType);

            pnlCmd.Controls.Add(SetLoc(CreateLabel("索引 (Idx):"), 150, row1Y + 3));
            txtIndex = CreateTextBox("0");
            txtIndex.Location = new Point(220, row1Y);
            txtIndex.Width = 50;
            pnlCmd.Controls.Add(txtIndex);

            pnlCmd.Controls.Add(SetLoc(CreateLabel("子索引 (Sub):"), 280, row1Y + 3));
            txtSubIndex = CreateTextBox("0");
            txtSubIndex.Location = new Point(360, row1Y);
            txtSubIndex.Width = 50;
            pnlCmd.Controls.Add(txtSubIndex);

            pnlCmd.Controls.Add(SetLoc(CreateLabel("數值 (Val):"), 420, row1Y + 3));
            txtValue = CreateTextBox("");
            txtValue.Location = new Point(490, row1Y);
            txtValue.Width = 80;
            pnlCmd.Controls.Add(txtValue);

            btnRead = new ActionButton("讀取 (Read)", IndTheme.StatusGreen);
            btnRead.Location = new Point(15, 95);
            btnRead.Click += btnRead_Click;
            pnlCmd.Controls.Add(btnRead);

            btnWrite = new ActionButton("寫入 (Write)", IndTheme.StatusYellow);
            btnWrite.Location = new Point(125, 95);
            btnWrite.Click += btnWrite_Click;
            pnlCmd.Controls.Add(btnWrite);

            layout.Controls.Add(secCmd, 0, 1);

            // 3. Log Section
            var secLog = new SectionPanel { Title = "系統日誌 (System Log)", Dock = DockStyle.Fill };
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

        private Control SetLoc(Control c, int x, int y) { c.Location = new Point(x, y); return c; }

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
                    LogError("無效索引 (Invalid Index)");
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
                    LogError("無效索引 (Invalid Index)");
                    return;
                }
                if (!int.TryParse(txtSubIndex.Text, out int subIndex)) subIndex = 0;

                if (!long.TryParse(txtValue.Text, out long value))
                {
                    LogError("無效數值 (Invalid Value)");
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
