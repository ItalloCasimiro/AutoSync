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
            //Path to the JSON with all the Users Examples (AutoSync/Data/Users.json)
            string usersListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../../Data", "Users.json");

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
            string newDriverPassword = Console.ReadLine() ?? "";

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

            if (!File.Exists(usersListPath))
            {
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, "Users Database Not Found...", ConsoleColor.Black, ConsoleColor.Red);
                return;
            }

            CLISystem.SkipRow();
            CNH newDriversCNH = CNHRegistrationService.RegisterCNH(newDriverID);

            if (!CLISystem.YNQuestion("Are you sure you want to", "add a new Driver"))
            {
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, $"Registration action canceled.", ConsoleColor.Black, ConsoleColor.Yellow);

                if (CLISystem.YNQuestion("Want to", "Back to menu"))
                {
                    Console.Clear();
                    AdminMenu.Initialize();
                }
                return;
            }

            //Load the Users JSON file and converts to a UserData class variable (A List for each role with all the users)
            var jsonString = File.ReadAllText(usersListPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            var usersDatabase = JsonSerializer.Deserialize<UsersDatabase>(jsonString) ?? new UsersDatabase();

            User newDriver = new User() { userID = newDriverID, password = newDriverPassword };

            usersDatabase.Drivers.Add(newDriver);

            string updatedJson = JsonSerializer.Serialize(usersDatabase, options);

            File.WriteAllText(usersListPath, updatedJson);

            Database.RegistryNewDriver(newDriversCNH.FullName, newDriversCNH);

            CLISystem.SkipRow();
            CLISystem.ShowStylizedMessage(true, $"{newDriverID} added successfully to the company's list of drivers.", ConsoleColor.Black, ConsoleColor.Green);
        }
    }
}
