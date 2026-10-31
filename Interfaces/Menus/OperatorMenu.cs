using AutoSync.CLI;
using AutoSync.Data;
using AutoSync.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoSync.Interfaces.Menus
{
    class OperatorMenu
    {
        public static Dictionary<string, Action> actions = new Dictionary<string, Action>()
        {
            { "1. Add a New Driver to The Company", DriverRegistrationService.AddNewDriver },
            { "2. Review maintenances", ReviewMaintenanceRequests },
            //{ "3. Check Driver(s) CNH(s)",  },
        };

        public static void Initialize()
        {
            CLISystem.ActionsList("What are You Planning to do Today?", actions);
        }

        public static void ReviewMaintenanceRequests()
        {
            Database.LoadMaintenanceRequests();

            string maintenanceRequestDriverName = "";
            List<MaintenanceRequest> driverRequests = [];

            do
            {
                CLISystem.SkipRow();

                Console.WriteLine("Which Driver you want to check requests?");

                maintenanceRequestDriverName = Console.ReadLine() ?? "";

                if (maintenanceRequestDriverName == "")
                {
                    CLISystem.SkipRow();
                    CLISystem.ShowStylizedMessage(true, "Error: The driver name can't be empty", ConsoleColor.Black, ConsoleColor.Yellow);
                    maintenanceRequestDriverName = "";
                }

                bool driverExists = false;

                foreach (User driver in Database.UsersList.Drivers)
                {
                    if (driver.userID == maintenanceRequestDriverName)
                    {
                        driverExists = true;
                        break;
                    }
                }

                if (!driverExists)
                {
                    CLISystem.SkipRow();
                    CLISystem.ShowStylizedMessage(true, "Error: Driver not found...", ConsoleColor.Black, ConsoleColor.Yellow);
                    maintenanceRequestDriverName = "";
                }

            } while (maintenanceRequestDriverName == "");

            int maintenanceIDX = 0;

            bool hasMaintenanceRequest = false;

            for (maintenanceIDX = 0; maintenanceIDX < Database.MaintenanceRequests.Count; maintenanceIDX++)
            {
                MaintenanceRequest request = Database.MaintenanceRequests[maintenanceIDX];

                if (request.DriverName == maintenanceRequestDriverName)
                {
                    driverRequests.Add(request);
                    hasMaintenanceRequest = true;

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
                CLISystem.ShowStylizedMessage(true, "No maintenance requests found on the database...", ConsoleColor.Black, ConsoleColor.Yellow);
                return;
            }

            else if (!hasMaintenanceRequest)
            {
                CLISystem.SkipRow();
                CLISystem.ShowStylizedMessage(true, "Error: No maintenance requests found for this driver...", ConsoleColor.Black, ConsoleColor.Yellow);
                return;
            }

            CLISystem.SkipRow();

            Dictionary<string, object> requestsOptions = new Dictionary<string, object>();

            for (maintenanceIDX = 0; maintenanceIDX < driverRequests.Count; maintenanceIDX++)
            {
                requestsOptions.Add((maintenanceIDX + 1) + ". "  + driverRequests[maintenanceIDX].Description, driverRequests[maintenanceIDX]);
            }

            MaintenanceRequest toReviewRequest =  (MaintenanceRequest)CLISystem.OptionsList($"Which request of {maintenanceRequestDriverName} do you want to review?", requestsOptions);

            MaintenanceRequest toReviewRequestOG = new MaintenanceRequest(
                toReviewRequest.Description,
                (MaintenanceRequest.Types)toReviewRequest.Type!,
                toReviewRequest.Date,
                toReviewRequest.Price,
                (MaintenanceRequest.RequestStatus)toReviewRequest.Status!
            );
            toReviewRequestOG.DriverName = toReviewRequest.DriverName;

            CLISystem.CreateInfoBox(
                $"Maintenance Request #{maintenanceIDX + 1}",
                new string[] { $"Description: {toReviewRequest.Description}" },
                keepOpen: true
            );

            CLISystem.CreateInfoBox(
                "",
                new string[]
                {
                    $"Type: {toReviewRequest.Type.ToString()}",
                    $"Date: {toReviewRequest.Date.ToString("dd/MM/yyyy")}",
                    $"Price: {(toReviewRequest.Price > 0.0 ? toReviewRequest.Price.ToString("F2") : "To Negotiate")}",
                    $"Status: {toReviewRequest.Status.ToString()}"
                }, isContinuation: true
            );

            CLISystem.SkipRow();

            if (CLISystem.YNQuestion("Is this request", "valid for maintenance"))
            {
                toReviewRequest.Status = MaintenanceRequest.RequestStatus.Approved;

                CLISystem.SkipRow();

                Console.WriteLine("Maintenance request approved!");

                CLISystem.SkipRow();

                do
                {
                    Console.Write("Assign a budget for this maintenance: ");
                    toReviewRequest.Price = CLISystem.FormatedDoubleValue(Console.ReadLine() ?? "");

                    if (toReviewRequest.Price == 0.0)
                    {
                        CLISystem.SkipRow();
                        CLISystem.ShowStylizedMessage(true, "Error: The typed budget is invalid.");
                        toReviewRequest.Price = 0.0;
                    }

                } while (toReviewRequest.Price == 0.0);

                CLISystem.CreateInfoBox(
                    $"Maintenance Request #{maintenanceIDX + 1}",
                    new string[] { $"Description: {toReviewRequest.Description}" },
                    keepOpen: true
                );

                CLISystem.CreateInfoBox(
                    "",
                    new string[]
                    {
                    $"Type: {toReviewRequest.Type.ToString()}",
                    $"Date: {toReviewRequest.Date.ToString("dd/MM/yyyy")}",
                    $"Price: {(toReviewRequest.Price > 0.0 ? toReviewRequest.Price.ToString("F2") : "To Negotiate")}",
                    $"Status: {toReviewRequest.Status.ToString()}"
                    }, isContinuation: true
                );

                if (CLISystem.YNQuestion("Update", "maintenance request status"))
                {
                    Database.UpdateMaintenanceRequest(toReviewRequestOG, toReviewRequest);
                    CLISystem.SkipRow();
                    CLISystem.ShowStylizedMessage(true, $"Maintenance request updated successfully!", ConsoleColor.Black, ConsoleColor.Green);
                }
                else
                {
                    CLISystem.SkipRow();
                    CLISystem.ShowStylizedMessage(true, $"Update action canceled, nothing changed.", ConsoleColor.Black, ConsoleColor.Yellow);
                }
            }
            else
            {
                toReviewRequest.Status = MaintenanceRequest.RequestStatus.Rejected;
                CLISystem.SkipRow();
                Console.WriteLine("Maintenance request rejected.");
            }
        }
    }
}
