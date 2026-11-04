using AutoSync.CLI;
using AutoSync.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace AutoSync.Interfaces
{ 
    public class ILogin
    {
        //Path to the JSON with all the Users Examples (AutoSync/Data/Users.json)
        private static string usersListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../../Data", "Users.json");

        public static AutoSync.UserData NewLogin()
        {
            Console.Write("Welcome to ");
            CLISystem.ShowStylizedMessage(false, "AutoSync", ConsoleColor.Black, ConsoleColor.Blue);
            Console.WriteLine(", Who are You?");

            CLISystem.SkipRow();

            Console.Write("Insert Your User ID: ");
            string userID = Console.ReadLine() ?? "";
            Console.Write($"Walcome, {userID}! Insert your password: ");
            string password = CLISystem.ReadHiddenLine() ?? "";

            //User that stores the ID and password typed by the user
            User userLoginAtempt = new User() { userID = userID, password = password};

            //Verifies if the login informations match with some role
            bool isAdmin = Database.UsersList.Admins.Any(u => u.userID == userLoginAtempt.userID && u.password == userLoginAtempt.password);
            bool isOperator = Database.UsersList.Operators.Any(u => u.userID == userLoginAtempt.userID && u.password == userLoginAtempt.password);
            bool isDriver = Database.UsersList.Drivers.Any(u => u.userID == userLoginAtempt.userID && u.password == userLoginAtempt.password);

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
    }
}