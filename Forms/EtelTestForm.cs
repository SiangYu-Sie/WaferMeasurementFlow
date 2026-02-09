
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaferMeasurementFlow.Managers;

namespace WaferMeasurementFlow.Forms
{
    public partial class EtelTestForm : Form
    {
        private EtelManager _etelManager;

        public EtelTestForm()
        {
            InitializeComponent();
            _etelManager = new EtelManager();
            _etelManager.OnLog += Log;
            _etelManager.OnError += LogError;
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
                Log($"Connection Exception: {ex.Message}");
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
        }

        private void LogError(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(LogError), message);
                return;
            }
            txtLog.AppendText($"[ERROR] {DateTime.Now:HH:mm:ss}: {message}{Environment.NewLine}");
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            try
            {
                string type = comboType.Text;
                if (string.IsNullOrEmpty(type)) type = "K"; // Default

                if (!int.TryParse(txtIndex.Text, out int index))
                {
                    LogError("Invalid Index");
                    return;
                }

                if (!int.TryParse(txtSubIndex.Text, out int subIndex))
                {
                    subIndex = 0; // Default
                }

                long value = _etelManager.ReadParameter(type, index, subIndex);
                txtValue.Text = value.ToString();
                Log($"Read Result: {value}");
            }
            catch (Exception ex)
            {
                LogError($"Read Failed: {ex.Message}");
            }
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            try
            {
                string type = comboType.Text;
                if (!int.TryParse(txtIndex.Text, out int index))
                {
                    LogError("Invalid Index");
                    return;
                }
                if (!int.TryParse(txtSubIndex.Text, out int subIndex)) subIndex = 0;

                if (!long.TryParse(txtValue.Text, out long value))
                {
                    LogError("Invalid Value (must be integer for now)");
                    return;
                }

                _etelManager.WriteParameter(type, index, subIndex, value);
                Log("Write Command Sent");
            }
            catch (Exception ex)
            {
                LogError($"Write Failed: {ex.Message}");
            }
        }
    }
}
