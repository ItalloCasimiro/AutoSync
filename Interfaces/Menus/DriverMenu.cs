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
            { "1. Register a new Vehicle", VehicleRegistrationService.RegisterVehicle }
        };

        public static void Initialize()
        {
            CLISystem.OptionsList("What are You Planning to do Today?", actions);
        }
    }
}
