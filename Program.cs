using System;
using AutoSync.Interfaces;
using AutoSync.Interfaces.Menus;
using AutoSync.CLI;
using AutoSync.Data;
using AutoSync.Services;

namespace AutoSync
{ 
    public enum UserRoles { None, Admin, Operator, Driver };

    public class UserData
    {
        public string userID { get; set; } = "";
        public UserRoles userRole { get; set; } = UserRoles.None;
    }

    public class Program
    {
        public static UserData loggedUser;

        public static async Task Main()
        {
            Database.LoadUsers();

            int loginAttempts = 3;

            //This function makes all the login process
            loginAttempt:
                loggedUser = ILogin.NewLogin();

                if (loggedUser.userRole == UserRoles.None) {
                    CLISystem.SkipRow();
                    CLISystem.ShowStylizedMessage(true, "User Not Found...", ConsoleColor.Black, ConsoleColor.Red);
                    loginAttempts -= 1;

                    if (loginAttempts == 0)
                    {
                        CLISystem.SkipRow();
                        CLISystem.ShowStylizedMessage(true, "Max login attempts reached, try again later.", ConsoleColor.Black, ConsoleColor.Red);
                        return;
                    }

                    if (CLISystem.YNQuestion("Want to", "try again"))
                    {
                        Console.Clear();
                        CLISystem.ShowStylizedMessage(true, $"Remaining login attempts: {loginAttempts}", ConsoleColor.Black, ConsoleColor.Red);
                        CLISystem.SkipRow();

                        goto loginAttempt;
                    }
                }

            Database.RegistryAccessLog(loggedUser.userID, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

            CLISystem.SkipRow();
            Console.Write($"Welcome again, ");
            CLISystem.ShowStylizedMessage(false, $"{loggedUser.userID}", ConsoleColor.Black, ConsoleColor.Yellow);
            Console.WriteLine("!");

            switch (loggedUser.userRole)
            {
                case UserRoles.Admin:
                    CLISystem.SkipRow();
                    AdminMenu.Initialize();
                    break;
                case UserRoles.Operator:
                    CLISystem.SkipRow();
                    OperatorMenu.Initialize();
                    break;
                case UserRoles.Driver:
                    CLISystem.SkipRow();
                    DriverMenu.Initialize();
                    break;
            }
        }
    }
}
