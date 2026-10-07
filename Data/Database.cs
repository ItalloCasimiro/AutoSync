using AutoSync.CLI;
using AutoSync.Interfaces;
using AutoSync.Interfaces.Menus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

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
    }
}
