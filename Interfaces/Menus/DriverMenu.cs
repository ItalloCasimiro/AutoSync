using AutoSync.CLI;
using System;
using System.Collections.Generic;
using System.Text;
using AutoSync.Services;

namespace AutoSync.Interfaces.Menus
{
    class DriverMenu
    {
        public static Dictionary<string, Action> actions = new Dictionary<string, Action>()
        {
            { "1. Register a new Vehicle", VehicleRegistrationService.RegisterVehicle },
            { "2. Request maintenance", MaintenanceService.RequestMaintenance },
            { "3. View maintenance requests", MaintenanceService.ViewMaintenanceRequests },
        };

        public static void Initialize()
        {
            CLISystem.ActionsList("What are You Planning to do Today?", actions);
        }
    }
}
