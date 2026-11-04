using AutoSync.CLI;
using AutoSync.Data;
using AutoSync.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AutoSync.Interfaces.Menus
{
    class AdminMenu
    {
        public static Dictionary<string, Action> actions = new Dictionary<string, Action>()
        {
            { "1. Add a New Driver to The Company", DriverRegistrationService.AddNewDriver },
            { "2. Add a New Operator to The Company", OperatorRegistrationService.AddNewOperator }
        };

        public static void Initialize()
        {
            CLISystem.ActionsList("What are You Planning to do Today?", actions);
        }
    }
}
