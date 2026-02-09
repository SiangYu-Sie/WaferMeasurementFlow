using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WaferMeasurementFlow.Managers;
using WaferMeasurementFlow.Models;
using WaferMeasurementFlow.Helpers;

namespace WaferMeasurementFlow
{
    public partial class RecipeEditorForm : Form
    {
        private readonly Equipment _equipment;

        public RecipeEditorForm(Equipment equipment)
        {
            InitializeComponent();
            _equipment = equipment;

            ThemeManager.ApplyTheme(this);

            LoadRecipes();
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
                    _recipeIdTextBox.ReadOnly = true; // Cannot rename ID directly

                    _parametersGridView.Rows.Clear();
                    foreach (var param in recipe.Parameters)
                    {
                        _parametersGridView.Rows.Add(param.Key, param.Value);
                    }
                    _txtTargetSlots.Text = recipe.TargetSlots; // Load TargetSlots
                }
            }
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            _recipeListBox.SelectedIndex = -1;
            _recipeIdTextBox.Text = "";
            _recipeIdTextBox.ReadOnly = false;
            _parametersGridView.Rows.Clear();
            _txtTargetSlots.Text = ""; // Clear
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
            recipe.TargetSlots = _txtTargetSlots.Text; // Save TargetSlots

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

            // Re-select the saved item
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

                // Clear UI
                _recipeIdTextBox.Text = "";
                _parametersGridView.Rows.Clear();
            }
        }
    }
}
