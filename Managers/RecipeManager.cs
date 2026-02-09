using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WaferMeasurementFlow.Models;
using WaferMeasurementFlow.Agents; // Added for SystemEventBus

namespace WaferMeasurementFlow.Managers
{
    public class RecipeManager
    {
        private ConcurrentDictionary<string, Recipe> _recipes = new ConcurrentDictionary<string, Recipe>();

        public RecipeManager()
        {
            // Add some default recipes for testing
            var defaultRecipe = new Recipe("MeasureWafer");
            defaultRecipe.Parameters["Temp"] = "25C";
            defaultRecipe.Parameters["Duration"] = "60s";
            SaveRecipe(defaultRecipe);
        }

        public void SaveRecipe(Recipe recipe)
        {
            _recipes[recipe.Id] = recipe;
            SystemEventBus.PublishLog($"RecipeManager: Saved recipe '{recipe.Id}'.");
        }

        public Recipe? GetRecipe(string id)
        {
            _recipes.TryGetValue(id, out var recipe);
            return recipe;
        }

        public List<Recipe> GetAllRecipes()
        {
            return _recipes.Values.ToList();
        }

        public void DeleteRecipe(string id)
        {
            if (_recipes.TryRemove(id, out _))
            {
                SystemEventBus.PublishLog($"RecipeManager: Deleted recipe '{id}'.");
            }
        }
    }
}
