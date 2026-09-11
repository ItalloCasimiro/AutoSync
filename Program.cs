using System;
using AutoSync.Interfaces;

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
        public static void Main()
        {
            UserData userData = ILogin.NewLogin();

            if (userData.userRole == UserRoles.None) {
                Console.WriteLine("User Not Found...");
                return;
            }

            Console.WriteLine($"Welcome again, {userData.userID}!");

            switch (userData.userRole)
            {
                case UserRoles.Admin:
                    break;
                case UserRoles.Operator:
                    break;
                case UserRoles.Driver:
                    break;
            }
        }
    }
}
