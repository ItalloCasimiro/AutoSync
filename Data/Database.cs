using AutoSync.CLI;
using AutoSync.Interfaces;
using AutoSync.Interfaces.Menus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using AutoSync.Services;

namespace AutoSync.Data
{
    // JSON User types Data Structure
    public class UsersDatabase
    {
        public List<User> Admins { get; set; } = new();
        public List<User> Operators { get; set; } = new();
        public List<User> Drivers { get; set; } = new();
    }

    public static class Database
    {
        //Path to the JSON with all the Users Examples (AutoSync/Data/Users.json)
        static string usersListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../../Data", "Users.json");

        public static UsersDatabase UsersList;

        public static List<string> UsersIDs;

        public static void LoadUsers()
        {
            if (!File.Exists(usersListPath))
            {
                Console.WriteLine("Users Database Not Found...");
            }

            //Load the Users JSON file and converts to a UserData class variable (A List for each role with all the users)
            var jsonString = File.ReadAllText(usersListPath);
            UsersList = JsonSerializer.Deserialize<UsersDatabase>(jsonString) ?? new UsersDatabase();

            UsersIDs = Database.UsersList.Admins      
            .Concat(Database.UsersList.Operators)
            .Concat(Database.UsersList.Drivers)
            .Select(u => u.userID)
            .ToList();
        }

        public static void RegistryAccessLog(string userID, string dateTime)
        {
            string accessLogsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../../Data", "AccessLogs.json");

            if (!File.Exists(accessLogsPath))
            {
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, "Access Logs Database Not Found...", ConsoleColor.Black, ConsoleColor.Red);
                return;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            string jsonString = File.ReadAllText(accessLogsPath);

            List<Dictionary<string, string>> accessLogs;

            // If has no logs, create a new list
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                accessLogs = new List<Dictionary<string, string>>();
            }
            else // If already has some log, load them
            {
                accessLogs = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonString, options) ?? new List<Dictionary<string, string>>();
            }

            // New access log
            accessLogs.Add(new Dictionary<string, string>
            {
                { "userID", userID },
                { "Date", dateTime }
            });

            // Update the database
            string updatedJson = JsonSerializer.Serialize(accessLogs, options);
            File.WriteAllText(accessLogsPath, updatedJson);
        }

        public static void RegistryNewDriver(string driverFullName, CNH driversCNH)
        {
             string driversListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../../Data", "Drivers.json");

            if (!File.Exists(driversListPath))
            {
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, "Drivers Database Not Found...", ConsoleColor.Black, ConsoleColor.Red);
                return;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            string jsonString = File.ReadAllText(driversListPath);

            List<Dictionary<string, object>> driversList;

            // If has no logs, create a new list
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                driversList = new List<Dictionary<string, object>>();
            }
            else // If already has some log, load them
            {
                driversList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonString, options) ?? new List<Dictionary<string, object>>();
            }

            // New access log
            driversList.Add(new Dictionary<string, object>
            {
                { "name", driverFullName },
                { "CNH", new Dictionary<string, object>
                {
                    {"fullName", driverFullName},
                    { "bornDate", driversCNH.BornDate.ToString("dd/MM/yyyy") },
                    { "registrationNumber", driversCNH.RegistrationNumber },
                    { "category", driversCNH.Category.ToString()! },
                    { "validity", driversCNH.Validity.ToString("dd/MM/yyyy") },
                    { "securityCode", driversCNH.SecurityCode }
                } }
            });

            // Update the database
            string updatedJson = JsonSerializer.Serialize(driversList, options);
            File.WriteAllText(driversListPath, updatedJson);
        }
        
        public static void RegistryNewVehicle(string ownerName, Vehicle vehicle)
        {
             string vehiclesListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../../Data", "Vehicles.json");

            if (!File.Exists(vehiclesListPath))
            {
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, "Vehicles Database Not Found...", ConsoleColor.Black, ConsoleColor.Red);
                return;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            string jsonString = File.ReadAllText(vehiclesListPath);

            List<Dictionary<string, object>> vehiclesList;

            // If has no logs, create a new list
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                vehiclesList = new List<Dictionary<string, object>>();
            }
            else // If already has some log, load them
            {
                vehiclesList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonString, options) ?? new List<Dictionary<string, object>>();
            }

            // New access log
            vehiclesList.Add(new Dictionary<string, object>
            {
                { "VehicleInfo", new Dictionary<string, object>
                {
                    { "ownerName", ownerName },
                    { "licensePlate", vehicle.LicensePlate},
                    { "model", vehicle.Model },
                    { "year", vehicle.Year },
                    { "capacity",vehicle.Capacity },
                    { "type", vehicle.Type! }
                } }
            });

            // Update the database
            string updatedJson = JsonSerializer.Serialize(vehiclesList, options);
            File.WriteAllText(vehiclesListPath, updatedJson);
        }
    }
}
