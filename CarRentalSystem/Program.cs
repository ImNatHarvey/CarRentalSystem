using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using FluentMigrator.Runner;
using CarRentalSystem.Forms;
using CarRentalSystem.Database;
using CarRentalSystem.Services; // Added to access UserService

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

            // 3. Seed the Default Admin Account
            SeedDefaultOwner();

            // 4. Launch the App (We will change this to LoginForm later!)
            Application.Run(new MainForm());
        }

        private static IServiceProvider CreateServices()
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer()
                    .WithGlobalConnectionString(DatabaseConfig.ConnectionString)
                    .ScanIn(typeof(Program).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        // Creates the initial owner account if it doesn't already exist
        private static void SeedDefaultOwner()
        {
            var userService = new UserService();
            
            // Check if the account already exists so we don't crash on multiple runs
            var existingOwner = userService.GetUserByUsername("NatarakiCar");
            
            if (existingOwner == null)
            {
                // RegisterUser automatically hashes "Nataraki2026" before saving it to SSMS
                userService.RegisterUser(
                    username: "NatarakiCar", 
                    rawPassword: "Nataraki2026", 
                    role: "Owner", 
                    firstName: "System", 
                    lastName: "Owner"
                );
            }
        }
    }
}