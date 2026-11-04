using AutoSync.CLI;
using AutoSync.Data;
using AutoSync.Interfaces.Menus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AutoSync.Services
{
    public class OperatorIdentification
    {
        public string FullName { get; set; }
        [JsonConverter(typeof(DateConverter))] public DateTime BornDate { get; set; }
        public int CompanyID { get; set; }

        public OperatorIdentification() { }

        public OperatorIdentification(string fullName, DateTime bornDate, int companyID)
        {
            FullName = fullName;
            BornDate = bornDate;
            CompanyID = companyID;
        }
    }

    class OperatorRegistrationService
    {
        public static void AddNewOperator()
        {
            Database.LoadOperatorsCompanyIdentifications();

            CLISystem.SkipRow();
            Console.Write("All right, ");
            CLISystem.ShowStylizedMessage(true, "let's add a new operator!", ConsoleColor.Black, ConsoleColor.Yellow);

            OperatorIdentification newOperatorID = new OperatorIdentification();

            // Full Name
            do
            {
                CLISystem.SkipRow();

                Console.Write("Insert the Operators's Full Name: ");
                newOperatorID.FullName = Console.ReadLine() ?? "";

                if (newOperatorID.FullName == "")
                {
                    CLISystem.SkipRow();
                    CLISystem.ShowStylizedMessage(true, "Error: The typed name is empty.", ConsoleColor.Black, ConsoleColor.Yellow);
                    newOperatorID.FullName = "";
                }

                else if (newOperatorID.FullName.Split(" ").Length < 2)
                {
                    CLISystem.SkipRow();
                    CLISystem.ShowStylizedMessage(true, "Error: Insert your full name.", ConsoleColor.Black, ConsoleColor.Yellow);
                    newOperatorID.FullName = "";
                }

                foreach (string name in newOperatorID.FullName.Split(" "))
                {
                    if (name.Length < 3)
                    {
                        CLISystem.SkipRow();
                        CLISystem.ShowStylizedMessage(true, "Error: The typed name is too short.", ConsoleColor.Black, ConsoleColor.Yellow);
                        newOperatorID.FullName = "";
                        break;
                    }
                }

                foreach (OperatorIdentification ID in Database.OperatorsIDs)
                {
                    if (ID.FullName == newOperatorID.FullName)
                    {
                        CLISystem.SkipRow();
                        CLISystem.ShowStylizedMessage(true, "Error: Already existent operator.", ConsoleColor.Black, ConsoleColor.Yellow);
                        newOperatorID.FullName = "";
                        break;
                    }
                }
            } while (newOperatorID.FullName == "");

            // Born Date
            do
            {
                CLISystem.SkipRow();

                Console.Write("Insert the Operators's Born Date: ");
                newOperatorID.BornDate = CLISystem.ReadDateInput();
            } while (newOperatorID.BornDate == DateTime.MinValue);

            // Company ID
            do
            {
                CLISystem.SkipRow();

                Console.Write($"Create a Company ID for {newOperatorID.FullName}: ");

                int.TryParse(Console.ReadLine() ?? "", out int CompanyIDInput);

                if (CompanyIDInput == 0)
                {
                    CLISystem.SkipRow();
                    CLISystem.ShowStylizedMessage(true, "Error: Invalid Company ID.", ConsoleColor.Black, ConsoleColor.Yellow);
                    newOperatorID.CompanyID = 0;
                }
                else if (CompanyIDInput.ToString().Length != 4)
                {
                    CLISystem.SkipRow();
                    CLISystem.ShowStylizedMessage(true, "Error: Additional / Missing digits for the Company ID.", ConsoleColor.Black, ConsoleColor.Yellow);
                    newOperatorID.CompanyID = 0;
                }

                bool foundExistentCompanyID = false;

                foreach (OperatorIdentification ID in Database.OperatorsIDs)
                {
                    if (ID.CompanyID == CompanyIDInput)
                    {
                        CLISystem.SkipRow();
                        CLISystem.ShowStylizedMessage(true, "Error: Already existent Company ID.", ConsoleColor.Black, ConsoleColor.Yellow);
                        newOperatorID.CompanyID = 0;
                        foundExistentCompanyID = true;
                        break;
                    }
                }

                if (!foundExistentCompanyID)
                {
                    newOperatorID.CompanyID = CompanyIDInput;
                }
            } while (newOperatorID.CompanyID == 0);

            User newOperatorUser = new User();
            newOperatorUser.userID = newOperatorID.FullName.Split(" ")[0];

            do
            {
                CLISystem.SkipRow();

                Console.Write("Insert a password for {0}: ", newOperatorUser.userID);
                newOperatorUser.password = CLISystem.ReadHiddenLine() ?? "";

                if (newOperatorUser.password.Length < newOperatorID.FullName.Split(" ")[0].Length)
                {
                    CLISystem.SkipRow();
                    CLISystem.ShowStylizedMessage(true, "Error: Short password. Password length must be at least equal or greater than the ID length.", ConsoleColor.Black, ConsoleColor.Yellow);
                    newOperatorUser.password = "";
                    return;
                }

                else if (newOperatorUser.password == newOperatorID.FullName.Split(" ")[0])
                {
                    CLISystem.SkipRow();
                    CLISystem.ShowStylizedMessage(true, "Error: The password must be different of the Driver's ID.", ConsoleColor.Black, ConsoleColor.Yellow);
                    newOperatorUser.password = "";
                }
            } while (newOperatorUser.password == "");

            CLISystem.SkipRow();

            CLISystem.CreateInfoBox(
                $"Company Identification",
                new string[] {
                    $"Full Name: {newOperatorID.FullName}",
                    $"Born Date: {newOperatorID.BornDate.ToString("dd/MM/yyyy")}",
                    $"Company ID: {newOperatorID.CompanyID}"
                }
            );

            if (CLISystem.YNQuestion("Add", "new Operator to the Company"))
            {
                Database.RegistryNewOperator(newOperatorID);
                Database.RegistryNewUser(newOperatorUser, UserRoles.Operator);
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, $"Maintenance request updated successfully!", ConsoleColor.Black, ConsoleColor.Green);
            }
            else
            {
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, $"{newOperatorID} added successfully to the company's list of operators.", ConsoleColor.Black, ConsoleColor.Yellow);
            }
        }
    }
}
