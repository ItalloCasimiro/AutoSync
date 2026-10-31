using AutoSync.CLI;
using AutoSync.Data;
using AutoSync.Interfaces;
using AutoSync.Interfaces.Menus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutoSync.Services
{
    public record class MaintenanceRequest
    {
        public enum Types { Corrective, Preventive }
        public enum RequestStatus { Waiting, Approved, Rejected, Canceled }

        public string DriverName { get; set; }
        public string Description { get; set; }
        public Types? Type { get; set; }
        [JsonConverter(typeof(DateConverter))] public DateTime Date { get; set; }
        public double Price { get; set; }
        public RequestStatus? Status { get; set; }
        
        public MaintenanceRequest(){}
        
        public MaintenanceRequest(string description, Types type, DateTime date, double price, RequestStatus status)
        {
            Description = description;
            Type = type;
            Date = date;
            Price = price;
            Status = status;
        }
    }
    
    public class MaintenanceService
    {
        public static void RequestMaintenance()
        {
            if (!VerifyIfHasVehicle())
            {
                CLISystem.ShowStylizedMessage(true, "ERROR: No vehicle registered in your name", ConsoleColor.Black, ConsoleColor.Yellow);
                if (CLISystem.YNQuestion("Want to", "Back to menu"))
                {
                    Console.Clear();
                    DriverMenu.Initialize();
                }
            }
            
            Console.WriteLine("Did your vehicle brake? Let's request a maintenance for it!");

            MaintenanceRequest request = new MaintenanceRequest();
            
            CLISystem.SkipRow();
            
             // Description
            do
            {
                Console.WriteLine("What happened to your vehicle?");
                request.Description = Console.ReadLine() ?? "";

                if (request.Description == "")
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The description can't be empty", ConsoleColor.Black, ConsoleColor.Yellow);
                    request.Description = "";
                    CLISystem.SkipRow();
                }
                else if (request.Description.Length < 50)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: Description is too short. Try to write at least 50 characters", ConsoleColor.Black, ConsoleColor.Yellow);
                    request.Description = "";
                    CLISystem.SkipRow();
                }
            } while (request.Description == "");
            
            CLISystem.SkipRow();

            //Type
            do
            {
                var typesOptions = new Dictionary<string, object>
                {
                    { "1. Corrective", MaintenanceRequest.Types.Corrective },
                    { "2. Preventive", MaintenanceRequest.Types.Preventive }
                };

                object? typeInputResult = CLISystem.OptionsList("What type of maintenance your vehicle needs?", typesOptions);

                if (typeInputResult is MaintenanceRequest.Types type)
                {
                    request.Type = type;
                }
                else
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: Invalid type. Choose one type from the list", ConsoleColor.Black, ConsoleColor.Yellow);
                    request.Type = null;
                    CLISystem.SkipRow();
                }
            } while (request.Type == null);

            CLISystem.SkipRow();

            //Date
            do
            {
                Console.WriteLine("""When you want to receive your maintenance? (Separate the numbers with "/" or "-")""");
                request.Date = CLISystem.ReadDateInput();

                if (request.Date == DateTime.MinValue)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The typed date is invalid", ConsoleColor.Black, ConsoleColor.Yellow);
                    CLISystem.SkipRow();
                }

                else if (request.Date < DateTime.Today)
                {
                    CLISystem.ShowStylizedMessage(true, $"ERROR: Today is {DateTime.Today.ToString("dd/MM/yyyy")}, try a later date", ConsoleColor.Black, ConsoleColor.Yellow);
                    request.Date = DateTime.MinValue;
                    CLISystem.SkipRow();
                }

            } while (request.Date == DateTime.MinValue);

            CLISystem.SkipRow();

            request.Status = MaintenanceRequest.RequestStatus.Waiting;

            CLISystem.CreateInfoBox(
                $"New Maintenance Request",
                new string[] { $"Description: {request.Description}" },
                keepOpen: true
            );

            CLISystem.CreateInfoBox(
                "",
                new string[]
                {
                    $"Type: {request.Type.ToString()}",
                    $"Date: {request.Date.ToString("dd/MM/yyyy")}",
                    $"Price: {(request.Price > 0.0 ? request.Price.ToString("F2") : "To Negotiate")}",
                    $"Status: {request.Status.ToString()}"
                }, isContinuation: true
            );

            if (CLISystem.YNQuestion("Want to", "request this maintenance"))
            {
                Database.RegistryNewMaintenanceRequest(Program.loggedUser.userID, request);

                CLISystem.ShowStylizedMessage(true, $"Request sent! Wait for an Operator to review and return a budget.", ConsoleColor.Black, ConsoleColor.Green);
            }
            else
            {
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, $"Update action canceled, new maintenance discarted.", ConsoleColor.Black, ConsoleColor.Yellow);
            }

            CLISystem.SkipRow();

            if (CLISystem.YNQuestion("Want to", "Back to menu"))
            {
                Console.Clear();
                DriverMenu.Initialize();
            }
        }

        private static bool VerifyIfHasVehicle()
        {
            string driverName = Program.loggedUser.userID;
            
            string vehiclesListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../../Data", "Vehicles.json");

            if (!File.Exists(vehiclesListPath))
            {
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, "Vehicles Database Not Found...", ConsoleColor.Black, ConsoleColor.Red);
                return false;
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

            foreach (Dictionary<string, object> vehicle in vehiclesList)
            {
                if (vehicle["ownerName"].ToString() == driverName)
                {
                    return true;
                }
            }

            return false;
        }

        public static void ViewMaintenanceRequests()
        {
            Database.LoadMaintenanceRequests();

            int maintenanceIDX = 0;

            for (maintenanceIDX = 0; maintenanceIDX < Database.MaintenanceRequests.Count; maintenanceIDX++)
            {
                MaintenanceRequest request = Database.MaintenanceRequests[maintenanceIDX];

                if (request.DriverName == Program.loggedUser.userID)
                {
                    CLISystem.CreateInfoBox(
                        $"Maintenance Request #{maintenanceIDX + 1}", 
                        new string[] { $"Description: {request.Description}" }, 
                        keepOpen: true
                    );

                    CLISystem.CreateInfoBox(
                        "",
                        new string[]
                        {
                            $"Type: {request.Type.ToString()}",
                            $"Date: {request.Date.ToString("dd/MM/yyyy")}",
                            $"Price: {(request.Price > 0.0 ? request.Price.ToString("F2") : "To Negotiate")}",
                            $"Status: {request.Status.ToString()}"
                        }, isContinuation: true
                    );
                }
            }

            if (maintenanceIDX == 0)
            {
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, "You have no maintenance requests...", ConsoleColor.Black, ConsoleColor.Yellow);
            }
        }
    }
}