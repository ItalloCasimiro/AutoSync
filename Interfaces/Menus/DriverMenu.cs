using AutoSync.CLI;
using AutoSync.Data;
using AutoSync.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoSync.Interfaces.Menus
{
    class DriverMenu
    {
        public static Dictionary<string, Action> actions = new Dictionary<string, Action>()
        {
            { "1. Register a new Vehicle", VehicleRegistrationService.RegisterVehicle },
            { "2. View registered Vehicles", ViewRegisteredVehicles },
            { "3. Request maintenance", MaintenanceService.RequestMaintenance },
            { "4. View maintenance requests", MaintenanceService.ViewMaintenanceRequests },
        };

        public static void Initialize()
        {
            CLISystem.ActionsList("What are You Planning to do Today?", actions);
        }

        private static void ViewRegisteredVehicles()
        {
            Database.LoadVehicles();

            int maintenanceIDX = 0;

            for (maintenanceIDX = 0; maintenanceIDX < Database.vehicles.Count; maintenanceIDX++)
            {
                Vehicle vehicle = Database.vehicles[maintenanceIDX];

                if (vehicle.Owner == Program.loggedUser.userID)
                {
                    CLISystem.CreateInfoBox(
                        $"{Program.loggedUser.userID} {vehicle.Model}",
                        new string[] {
                            $"Model: {vehicle.Model}",
                            $"Year: {vehicle.Year}",
                            $"License Plate: {vehicle.LicensePlate}",
                            $"Max Velocity: {vehicle.MaxVelocity}km/h",
                            $"Capacity: {vehicle.Capacity}kg",
                            $"Vehicle Type: {vehicle.Type}"
                        }
                    );
                }
            }

            if (maintenanceIDX == 0)
            {
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, "You have no maintenance requests...", ConsoleColor.Black, ConsoleColor.Yellow);
            }

            if (CLISystem.YNQuestion("Want to", "Back to menu"))
            {
                Console.Clear();
                DriverMenu.Initialize();
            }
        }
    }
}
