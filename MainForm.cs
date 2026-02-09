using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WaferMeasurementFlow.Agents;
using WaferMeasurementFlow.Models;
using Microsoft.Extensions.DependencyInjection; // Added for IServiceProvider

using WaferMeasurementFlow.Helpers;


namespace WaferMeasurementFlow
{
    public partial class MainForm : Form
    {
        private readonly Equipment _equipment;
        private readonly IServiceProvider _serviceProvider; // Added for opening PJCJCreationForm

        // UI Controls are assigned by InitializeComponent()


        public MainForm(Equipment equipment, IServiceProvider serviceProvider) // IServiceProvider kept
        {
            InitializeComponent(); // This will now set up all UI controls
            _equipment = equipment;
            _serviceProvider = serviceProvider; // Assign serviceProvider


            SubscribeToEvents();

            // Apply Dark/Green Theme
            ThemeManager.ApplyTheme(this);

            UpdateUI();
        }

        // InitializeCustomUI() method is removed and its content moved to MainForm.Designer.cs

        private void SubscribeToEvents()
        {
            SystemEventBus.LogMessagePublished += (message) =>
            {
                if (_logListBox.InvokeRequired) // Check if invoke is required
                    _logListBox.Invoke((MethodInvoker)delegate { _logListBox.Items.Insert(0, message); });
                else
                    _logListBox.Items.Insert(0, message);
            };

            SystemEventBus.LoadPortStateChanged += (loadPort) =>
            {
                if (InvokeRequired) Invoke((MethodInvoker)UpdateUI); else UpdateUI();
            };

            SystemEventBus.CarrierStateChanged += (carrier) =>
            {
                if (InvokeRequired) Invoke((MethodInvoker)UpdateUI); else UpdateUI();
            };

            SystemEventBus.SubstrateStateChanged += (substrate) =>
            {
                if (InvokeRequired) Invoke((MethodInvoker)UpdateUI); else UpdateUI();
            };
        }

        private void UpdateUI()
        {
            // Update Port 1 Status
            string p1State = GetLoadPortStateString(_equipment.LoadPorts[1].State);
            string p1Carrier = _equipment.LoadPorts[1].Carrier?.Id ?? "無";
            _lblLoadPortState.Text = $"Load Port 1: {p1State}";
            _lblCarrierId.Text = $"Carrier ID: {p1Carrier}"; // Simplified logic, assume tracking active port or show both?

            // Note: _lblCarrierId in Designer was single, but logic might want to show based on selected port. 
            // For now, let's keep it simple or fix it. 
            // The Designer has two LoadPort labels.
            // Let's update Port 2 label too.
            string p2State = GetLoadPortStateString(_equipment.LoadPorts[2].State);
            _lblLoadPort2State.Text = $"Load Port 2: {p2State}";

            // Buttons logic (Simplify: Enable if ANY port is suitable)
            bool anyEmpty = _equipment.LoadPorts.Values.Any(p => p.State == LoadPortState.EMPTY);
            bool anyReady = _equipment.LoadPorts.Values.Any(p => p.State == LoadPortState.READY_TO_PROCESS);
            bool anyUnload = _equipment.LoadPorts.Values.Any(p => p.State == LoadPortState.READY_TO_UNLOAD || p.State == LoadPortState.READY_TO_PROCESS);

            _btnPlaceCarrier.Enabled = anyEmpty;
            _btnExecuteJob.Enabled = anyReady;
            _btnUnloadCarrier.Enabled = anyUnload;

            // Update SlotMap for selected port
            UpdateSlotMapDisplay();
        }

        private void UpdateSlotMapDisplay()
        {
            _slotMapGridView.Rows.Clear();
            // Show map of selected port
            if (_comboPortSelect.SelectedIndex >= 0)
            {
                int portId = GetSelectedPortId();
                var port = _equipment.LoadPorts[portId];
                if (port.Carrier != null)
                {
                    var sortedSlots = port.Carrier.SlotMap.Values.OrderBy(s => s.Slot);
                    foreach (var substrate in sortedSlots)
                    {
                        string stateStr = GetSubstrateStateString(substrate.State);
                        _slotMapGridView.Rows.Add(substrate.Slot, substrate.Id, stateStr);
                    }
                }
            }
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

        private int GetSelectedPortId()
        {
            // Index 0 -> Port 1, Index 1 -> Port 2
            if (_comboPortSelect.SelectedIndex < 0) return 1;
            return _comboPortSelect.SelectedIndex + 1;
        }

        private async void BtnPlaceCarrier_Click(object? sender, EventArgs e)
        {
            int portId = GetSelectedPortId();
            var targetPort = _equipment.LoadPorts[portId];

            if (targetPort.State != LoadPortState.EMPTY)
            {
                MessageBox.Show($"Load Port {portId} 目前不是空白狀態 (Not EMPTY)。");
                return;
            }

            var carrier = new Carrier($"C{DateTime.Now.Ticks % 1000:000}"); // Random ID
            // Manually add some substrates
            carrier.SlotMap[1] = new Substrate($"Wafer-{portId}-01", 1);
            carrier.SlotMap[2] = new Substrate($"Wafer-{portId}-02", 2);

            await targetPort.PlaceCarrier(carrier);
        }

        private void BtnExecuteJob_Click(object? sender, EventArgs e)
        {
            // For Quick Test: Just run job on whatever port is Ready
            // In real scenario, we use Create PJCJ flow.
            MessageBox.Show("請使用 '建立新工單' 功能來定義多 Load Port 的作業。");
        }

        private void BtnCreatePJCJ_Click(object? sender, EventArgs e)
        {
            using (var form = _serviceProvider.GetRequiredService<PJCJCreationForm>())
            {
                form.ShowDialog();
            }
            UpdateUI();
        }

        private void UnloadCarrier_Click(object? sender, EventArgs e)
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
            var form = _serviceProvider.GetRequiredService<RecipeEditorForm>();
            form.ShowDialog();
        }

        private void BtnSecsMonitor_Click(object sender, EventArgs e)
        {
            _equipment.SecsManager.ShowMonitor();
        }
    }
}