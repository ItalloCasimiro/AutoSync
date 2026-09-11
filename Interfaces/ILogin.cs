using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AutoSync.Interfaces
{
    // Data for each User
    public class User
    {
        public string userID { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
    }

    // JSON User types Data Structure
    public class UserData
    {
        public List<User> Admins { get; set; } = new();
        public List<User> Operators { get; set; } = new();
        public List<User> Drivers { get; set; } = new();
    }

    public class ILogin
    {
        private static string usersListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../../Data", "Users.json");

        public static AutoSync.UserData NewLogin()
        {
            Console.WriteLine("Welcome to AutoSync, Who are You?");

            Console.Write("Insert Your User ID: ");
            string userID = Console.ReadLine() ?? "";
            Console.Write($"Walcome, {userID}! Insert your password: ");
            string password = Console.ReadLine() ?? "";

            User userLoginAtempt = new User() { userID = userID, password = password};

            UserData usersList = LoadUsers();

            bool isAdmin = usersList.Admins.Any(u => u.userID == userLoginAtempt.userID && u.password == userLoginAtempt.password);
            bool isOperator = usersList.Operators.Any(u => u.userID == userLoginAtempt.userID && u.password == userLoginAtempt.password);
            bool isDriver = usersList.Drivers.Any(u => u.userID == userLoginAtempt.userID && u.password == userLoginAtempt.password);

            if (isAdmin)
            {
                return new AutoSync.UserData() { userID = userLoginAtempt.userID, userRole = UserRoles.Admin };
            }
            else if (isOperator) {
                return new AutoSync.UserData() { userID = userLoginAtempt.userID, userRole = UserRoles.Operator };
            }
            else if (isDriver)
            {
                return new AutoSync.UserData() { userID = userLoginAtempt.userID, userRole = UserRoles.Driver };
            }

            return new AutoSync.UserData() { userID = "", userRole = UserRoles.None };
        }

        public static UserData LoadUsers()
        {
            if (!File.Exists(usersListPath))
            {
                Console.WriteLine("Users Database Not Found...");
                return new UserData();
            }

            var jsonString = File.ReadAllText(usersListPath);
            return JsonSerializer.Deserialize<UserData>(jsonString) ?? new UserData();
        }
    }
}