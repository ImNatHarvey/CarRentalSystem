using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using FluentMigrator.Runner;
using CarRentalSystem.Forms;
using CarRentalSystem.Database; 

namespace CarRentalSystem
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 1. Setup the FluentMigrator Service
            var serviceProvider = CreateServices();

            // 2. Run the Database Migrations (Creates/Updates Tables in SSMS)
            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }

            // 3. Launch the App
            Application.Run(new MainForm());
        }

        // Configures the migration runner to look for your SQL connection and Migration classes
        private static IServiceProvider CreateServices()
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer()
                    .WithGlobalConnectionString(DatabaseConfig.ConnectionString)
                    // Scans your current project for any classes labeled with [Migration]
                    .ScanIn(typeof(Program).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        // Executes the migrations up to the latest version
        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }
}