using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using WaferMeasurementFlow.Agents;
using WaferMeasurementFlow.Core;
using WaferMeasurementFlow.Forms;

namespace WaferMeasurementFlow
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            Application.Run(serviceProvider.GetRequiredService<MainForm>());
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // Register Equipment as Singleton (System Root)
            services.AddSingleton<Equipment>();

            // Register Forms
            services.AddTransient<MainForm>();
            services.AddTransient<PJCJCreationForm>();
            services.AddTransient<RecipeEditorForm>();
        }
    }
}