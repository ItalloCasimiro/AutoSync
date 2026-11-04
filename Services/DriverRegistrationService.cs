using AutoSync.CLI;
using AutoSync.Data;
using AutoSync.Interfaces;
using AutoSync.Interfaces.Menus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AutoSync.Services
{
    class DriverRegistrationService
    {
        public static void AddNewDriver()
        {
            CLISystem.SkipRow();
            Console.Write("All right, ");
            CLISystem.ShowStylizedMessage(true, "let's add a new driver!", ConsoleColor.Black, ConsoleColor.Yellow);
            CLISystem.SkipRow();

            Console.Write("Insert the Driver's ID: ");
            string newDriverID = Console.ReadLine() ?? "";

            if (newDriverID == "" || newDriverID.Length < 3)
            {
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, "Error: Invalid user name. It must have at least 3 characters.", ConsoleColor.Black, ConsoleColor.Yellow);
                AddNewDriver();
                return;
            }

            else if (Database.UsersIDs.Contains(newDriverID))
            {
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, "Error: Already existent driver.", ConsoleColor.Black, ConsoleColor.Yellow);
                AddNewDriver();
                return;
            }

            Console.Write("Insert a password for {0}: ", newDriverID);
            string newDriverPassword = CLISystem.ReadHiddenLine() ?? "";

            if (newDriverPassword.Length < newDriverID.Length)
            {
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, "Error: Short password. Password length must be at least equal or greater than the ID length.", ConsoleColor.Black, ConsoleColor.Yellow);
                AddNewDriver();
                return;
            }

            else if (newDriverPassword == newDriverID)
            {
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, "Error: The password must be different of the Driver's ID.", ConsoleColor.Black, ConsoleColor.Yellow);
                AddNewDriver();
                return;
            }

            CLISystem.SkipRow();
            CNH newDriversCNH = CNHRegistrationService.RegisterCNH(newDriverID);

            if (CLISystem.YNQuestion("Are you sure you want to", "add a new Driver"))
            {
                User newDriver = new User() { userID = newDriverID, password = newDriverPassword };

                Database.RegistryNewDriver(newDriversCNH.FullName, newDriversCNH);

                Database.RegistryNewUser(newDriver, UserRoles.Driver);

                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, $"{newDriverID} added successfully to the company's list of drivers.", ConsoleColor.Black, ConsoleColor.Green);
            }
            else
            {
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, $"Registration action canceled.", ConsoleColor.Black, ConsoleColor.Yellow);
            }

            if (CLISystem.YNQuestion("Want to", "Back to menu"))
            {
                Console.Clear();
                AdminMenu.Initialize();
            }
        }
    }
}
