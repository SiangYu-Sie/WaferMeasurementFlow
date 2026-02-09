using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WaferMeasurementFlow.Agents;
using WaferMeasurementFlow.Models;
using Microsoft.Extensions.DependencyInjection; // Still needed for GetRequiredService in BtnCreatePJCJ_Click

using WaferMeasurementFlow.Helpers;

namespace WaferMeasurementFlow
{
    public partial class PJCJCreationForm : Form
    {
        private readonly Equipment _equipment;

        // UI Controls are assigned by InitializeComponent()


        public PJCJCreationForm(Equipment equipment)
        {
            InitializeComponent(); // This will now set up all UI controls
            ThemeManager.ApplyTheme(this);
            _equipment = equipment;

            LoadAvailableLoadPorts();
            LoadRecipes();
        }

        private void LoadRecipes()
        {
            _recipeComboBox!.Items.Clear();
            var recipes = _equipment.RecipeManager.GetAllRecipes();
            foreach (var recipe in recipes)
            {
                _recipeComboBox.Items.Add(recipe.Id);
            }
            // Bind Event manually if not in designer
            _recipeComboBox.SelectedIndexChanged += RecipeComboBox_SelectedIndexChanged;
        }

        private void RecipeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateWafersToProcessList();
        }

        private void LoadAvailableLoadPorts()
        {
            _loadPortSelectionComboBox!.Items.Clear();
            foreach (var port in _equipment.LoadPorts.Values)
            {
                _loadPortSelectionComboBox.Items.Add(port.Id);
            }
            if (_loadPortSelectionComboBox.Items.Count > 0)
                _loadPortSelectionComboBox.SelectedIndex = 0;
        }

        private void LoadPortSelectionComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateWafersToProcessList();
        }

        private void UpdateWafersToProcessList()
        {
            _wafersToProcessCheckedListBox!.Items.Clear();
            if (_loadPortSelectionComboBox.SelectedItem is int selectedLoadPortId)
            {
                // Determine target slots from Recipe
                HashSet<int>? targetSlots = null;
                if (_recipeComboBox.SelectedItem is string recipeId)
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
                            _wafersToProcessCheckedListBox.Items.Add($"{substrate.Id} (Slot {substrate.Slot})", shouldCheck ? CheckState.Checked : CheckState.Unchecked);
                        }
                    }
                }
            }
        }

        private HashSet<int>? ParseTargetSlots(string slotsStr)
        {
            var slots = new HashSet<int>();
            if (slotsStr.Trim().ToUpper() == "ALL") return null; // Null means All

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

        private async void BtnCreateAndStartJob_Click(object? sender, EventArgs e)
        {
            _btnCreateAndStartJob!.Enabled = false;

            // Validation
            if (_loadPortSelectionComboBox!.SelectedItem == null)
            {
                MessageBox.Show("請選擇一個 Load Port。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnCreateAndStartJob.Enabled = true;
                return;
            }
            if (string.IsNullOrWhiteSpace(_controlJobIdTextBox!.Text))
            {
                MessageBox.Show("Control Job ID 不能為空。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnCreateAndStartJob.Enabled = true;
                return;
            }
            if (string.IsNullOrWhiteSpace(_processJobIdTextBox!.Text))
            {
                MessageBox.Show("Process Job ID 不能為空。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnCreateAndStartJob.Enabled = true;
                return;
            }


            if (!(_loadPortSelectionComboBox!.SelectedItem is int selectedPortId))
            {
                MessageBox.Show("請選擇一個 Load Port。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnCreateAndStartJob.Enabled = true;
                return;
            }
            var targetPort = _equipment.LoadPorts[selectedPortId];

            var selectedWafers = new List<Substrate>();
            foreach (var item in _wafersToProcessCheckedListBox!.CheckedItems)
            {
                var itemStr = item.ToString()!;
                var parts = itemStr.Split(new[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
                var slot = int.Parse(parts[1].Replace("Slot", "").Trim());

                // Retrieve the actual substrate object from the load port's carrier
                if (targetPort.Carrier != null && targetPort.Carrier.SlotMap.TryGetValue(slot, out Substrate? substrate))
                {
                    selectedWafers.Add(substrate);
                }
            }

            if (!selectedWafers.Any())
            {
                MessageBox.Show("請至少選擇一個 Wafer 進行處理。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnCreateAndStartJob.Enabled = true;
                return;
            }

            string recipeId = _recipeComboBox!.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(recipeId))
            {
                MessageBox.Show("請選擇 Recipe。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnCreateAndStartJob.Enabled = true;
                return;
            }

            // 1. Create Process Job (SEMI E40)
            var processJob = _equipment.ControlJobManager.CreateProcessJob(
                _processJobIdTextBox.Text,
                recipeId,
                selectedWafers);

            // 2. Create Control Job (SEMI E94)
            var controlJob = _equipment.ControlJobManager.CreateControlJob(
                _controlJobIdTextBox.Text,
                new List<ProcessJob> { processJob },
                targetPort.Carrier!); // We verified carrier is not null earlier

            // 3. Execute Control Job
            MessageBox.Show($"Control Job '{controlJob.Id}' Created & Queued. Starting Execution...", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);

            await _equipment.ControlJobManager.ExecuteControlJob(controlJob);

            MessageBox.Show($"Control Job '{controlJob.Id}' 已完成。", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }
    }
}