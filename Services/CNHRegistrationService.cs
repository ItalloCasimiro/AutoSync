using AutoSync.CLI;
using AutoSync.Interfaces.Menus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AutoSync.Services
{
    public class CNH
    {
        public enum Categories { A, B, C, D, E, ACC }

        public string FullName;
        public DateTime BornDate;
        public int RegistrationNumber;
        public Categories? Category;
        public DateTime Validity;
        public int SecurityCode;

        public CNH(string fullName, DateTime bornDate, int registrationNumber, Categories category, DateTime validity, int securityCode)
        {
            FullName = fullName;
            BornDate = bornDate;
            RegistrationNumber = registrationNumber;
            Category = category;
            Validity = validity;
            SecurityCode = securityCode;
        }

        public CNH() { }
    }

    class CNHRegistrationService
    {
        public static CNH RegisterCNH(string driver)
        {
            CNH newDriversCNH = new CNH();

            Console.WriteLine($"Let's Register a new CNH for {driver}!");
            CLISystem.SkipRow();

            //Full Name
            do
            {
                Console.WriteLine($"What's the driver's full name?");
                Console.Write(driver + " ");
                newDriversCNH.FullName = driver + " " + Console.ReadLine() ?? "";

                if (newDriversCNH.FullName == "")
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The name can't be empty", ConsoleColor.Black, ConsoleColor.Yellow);
                    CLISystem.SkipRow();
                }

                else if (newDriversCNH.FullName.Length < 4)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: Last name is too short", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversCNH.FullName = "";
                    CLISystem.SkipRow();
                }
            } while (newDriversCNH.FullName == "");

            CLISystem.SkipRow();

            //Born Date
            do
            {
                Console.WriteLine("""What's the driver's born date? (Separate the numbers with "/" or "-")""");
                newDriversCNH.BornDate = CLISystem.ReadDateInput();

                if (newDriversCNH.BornDate == DateTime.MinValue)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The typed date is invalid", ConsoleColor.Black, ConsoleColor.Yellow);
                    CLISystem.SkipRow();
                }

                else if (newDriversCNH.BornDate.Year > 2026 || newDriversCNH.BornDate.Year < 1900)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: Invalid year", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversCNH.BornDate = DateTime.MinValue;
                    CLISystem.SkipRow();
                }

                int age = DateTime.Today.Year - newDriversCNH.BornDate.Year;

                if (newDriversCNH.BornDate.Month > DateTime.Today.Month || (newDriversCNH.BornDate.Month == DateTime.Today.Month && newDriversCNH.BornDate.Day > DateTime.Today.Day))
                {
                    age--;
                }   

                else if (age < 18)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: You must be at least 18 years old", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversCNH.BornDate = DateTime.MinValue;
                    CLISystem.SkipRow();
                }
            } while (newDriversCNH.BornDate == DateTime.MinValue);

            CLISystem.SkipRow();

            //Registration Number
            do
            {
                Console.WriteLine($"What's the driver's CNH Registration Number?");
                string inputRegistrationNumber = Console.ReadLine() ?? "";

                if (!int.TryParse(inputRegistrationNumber, out newDriversCNH.RegistrationNumber))
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The typed Registration Number isn't a number", ConsoleColor.Black, ConsoleColor.Yellow);
                    CLISystem.SkipRow();
                }

                else if (newDriversCNH.RegistrationNumber < 0)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The Registration Number can't be negative", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversCNH.RegistrationNumber = 0;
                    CLISystem.SkipRow();
                }

                else if (newDriversCNH.RegistrationNumber.ToString().Length != 9)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: Missing / Addictional digits in the Registration Number (must be 9)", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversCNH.RegistrationNumber = 0;
                    CLISystem.SkipRow();
                }

            } while (newDriversCNH.RegistrationNumber == 0);

            CLISystem.SkipRow();

            //Category
            do
            {
                Console.WriteLine($"What's the driver's CNH Category?");

                Console.WriteLine("1.A");
                Console.WriteLine("2.B");
                Console.WriteLine("3.C");
                Console.WriteLine("4.D");
                Console.WriteLine("5.E");
                Console.WriteLine("6.ACC");

                string inputCategory = Console.ReadLine() ?? "";

                if (!int.TryParse(inputCategory, out int categoryIDX))
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The typed choice isn't in the list", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversCNH.Category = null;
                }

                if (Enum.IsDefined(typeof(CNH.Categories), categoryIDX - 1))
                {
                    newDriversCNH.Category = (CNH.Categories)categoryIDX - 1;
                }
                else
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The typed choice isn't in the list", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversCNH.Category = null;
                }

            } while (newDriversCNH.Category == null);

            CLISystem.SkipRow();

            //Vality
            do
            {
                Console.WriteLine("""When the driver's CNH expires? (Separate the numbers with "/" or "-")""");
                newDriversCNH.Validity = CLISystem.ReadDateInput();

                if (newDriversCNH.Validity == DateTime.MinValue)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The typed date is invalid", ConsoleColor.Black, ConsoleColor.Yellow);
                    CLISystem.SkipRow();
                }

                else if (newDriversCNH.Validity < DateTime.Today)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The CNH is out of vality, the driver must renew the document", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversCNH.Validity = DateTime.MinValue;
                    CLISystem.SkipRow();
                }

            } while (newDriversCNH.Validity == DateTime.MinValue);

            CLISystem.SkipRow();

            //Security Code
            do
            {
                Console.WriteLine($"What's the driver's CNH Security Code?");
                string inputCode = Console.ReadLine() ?? "";

                if (!int.TryParse(inputCode, out newDriversCNH.SecurityCode))
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The typed code isn't a number", ConsoleColor.Black, ConsoleColor.Yellow);
                    CLISystem.SkipRow();
                }

                else if (newDriversCNH.SecurityCode < 0)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The Security Code can't be negative", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversCNH.SecurityCode = 0;
                    CLISystem.SkipRow();
                }

                else if (newDriversCNH.SecurityCode.ToString().Length != 3)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: Missing / Addictional digits in the Security Code (must be 3)", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversCNH.SecurityCode = 0;
                    CLISystem.SkipRow();
                }

            } while (newDriversCNH.SecurityCode == 0);

            CLISystem.SkipRow();

            CLISystem.CreateInfoBox(
                $"{driver} CNH", 
                new string[] { 
                    $"Full Name: {newDriversCNH.FullName}", 
                    $"Born Date: {newDriversCNH.BornDate.ToString("dd/MM/yyyy")}", 
                    $"Registration N° {newDriversCNH.RegistrationNumber.ToString()}", 
                    $"Category: {newDriversCNH.Category.ToString()!}", 
                    $"Security Code: {newDriversCNH.SecurityCode.ToString()}" 
                }, keepOpen: true);
            CLISystem.CreateInfoBox("", new string[] { $"Vality: {newDriversCNH.Validity.ToString("dd/MM/yyyy")}" }, isContinuation: true);

            CLISystem.SkipRow();
            CLISystem.ShowStylizedMessage(true, $"New CNH for {driver} registered successfully!", ConsoleColor.Black, ConsoleColor.Green);

            return newDriversCNH;
        }
    }
}
