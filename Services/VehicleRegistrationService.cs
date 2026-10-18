using AutoSync.CLI;
using AutoSync.Interfaces;
using AutoSync.Interfaces.Menus;
using System;
using System.Collections.Generic;
using System.Text;
using AutoSync.Data;

namespace AutoSync.Services
{
    public class Vehicle
    {
        public enum Types { Passengers, Cargo, Race } //Like: Cars, Trucks and Formula 1

        public UserData Owner;
        public string LicensePlate;
        public string Model;
        public int Year;
        public double Capacity;
        public Types? Type;

        public Vehicle() { }

        public Vehicle(UserData owner, string licensePlate, string model, int year, double capacity, Types type)
        {
            Owner = owner;
            LicensePlate = licensePlate;
            Model = model;
            Year = year;
            Capacity = capacity;
            Type = type;
        }
    }

    class VehicleRegistrationService
    {
        public static void RegisterVehicle()
        {
            Vehicle newDriversVehicle = new Vehicle();

            Console.WriteLine("Let's Register a vehicle for you!");
            CLISystem.SkipRow();

            //Type
            do
            {
                Console.WriteLine($"What's the vehicle's type?");

                Console.WriteLine("1.Passengers");
                Console.WriteLine("2.Cargo");
                Console.WriteLine("3.Race");

                string inputType = Console.ReadLine() ?? "";

                if (!int.TryParse(inputType, out int typeIDX))
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The typed choice isn't in the list", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversVehicle.Type = null;
                }

                if (Enum.IsDefined(typeof(CNH.Categories), typeIDX - 1))
                {
                    newDriversVehicle.Type = (Vehicle.Types)typeIDX - 1;
                }
                else
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The typed choice isn't in the list", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversVehicle.Type = null;
                }

            } while (newDriversVehicle.Type == null);

            CLISystem.SkipRow();

            //License Plate
            do
            {
                Console.WriteLine("What's the vehicle's license plate?");

                newDriversVehicle.LicensePlate = Console.ReadLine() ?? "";

                if (newDriversVehicle.LicensePlate.Length != 7)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The informed License Plate has missing / addictional digits (must be 7)", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversVehicle.LicensePlate = "";
                    CLISystem.SkipRow();
                }
                else if (newDriversVehicle.LicensePlate.Count(char.IsDigit) != 3){
                    CLISystem.ShowStylizedMessage(true, "ERROR: The informed License Plate can't have more / less than 3 numbers", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversVehicle.LicensePlate = "";
                    CLISystem.SkipRow();
                }
                else if (newDriversVehicle.LicensePlate.Count(char.IsLetter) != 4)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The informed License Plate can't have more / less than 4 letters", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversVehicle.LicensePlate = "";
                    CLISystem.SkipRow();
                }

            } while (newDriversVehicle.LicensePlate == "");

            newDriversVehicle.LicensePlate = newDriversVehicle.LicensePlate.ToUpper();

            //Model
            do
            {
                CLISystem.SkipRow();

                Console.WriteLine("What's the vehicle's model?");

                newDriversVehicle.Model = Console.ReadLine() ?? "";

                if (newDriversVehicle.Model.Length < 3)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The informed model is too short", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversVehicle.Model = "";
                    CLISystem.SkipRow();
                }
                if (newDriversVehicle.Model.Length > 12)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The informed model is too long", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversVehicle.Model = "";
                    CLISystem.SkipRow();
                }

                if (newDriversVehicle.Model != "")
                {
                    if (!CLISystem.YNQuestion("Is your", $"car's model: {newDriversVehicle.Model}"))
                    {
                        newDriversVehicle.Model = "";
                    }
                }
            } while (newDriversVehicle.Model == "");

            CLISystem.SkipRow();

            //Year
            do
            {
                Console.WriteLine("What's the vehicle's year?");

                int.TryParse(Console.ReadLine(), out newDriversVehicle.Year);

                if (newDriversVehicle.Year == 0)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The informed year is not a valid number", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversVehicle.Year = 0;
                    CLISystem.SkipRow();
                }
                else if (newDriversVehicle.Year.ToString().Length != 4)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The informed year is not valid", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversVehicle.Year = 0;
                    CLISystem.SkipRow();
                }
                else if (newDriversVehicle.Year < 1886)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: Cars were invented in 1886, driver", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversVehicle.Year = 0;
                    CLISystem.SkipRow();
                }
                else if (newDriversVehicle.Year > DateTime.Now.Year)
                {
                    CLISystem.ShowStylizedMessage(true, $"ERROR: We're currently in {DateTime.Now.Year}, driver", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversVehicle.Year = 0;
                    CLISystem.SkipRow();
                }
            } while (newDriversVehicle.Year == 0);

            CLISystem.SkipRow();

            //Capacity
            do
            {
                Console.WriteLine("What's the vehicle's capacity?");

                newDriversVehicle.Capacity = CLISystem.FormatedDoubleValue(Console.ReadLine()!);

                if (newDriversVehicle.Capacity == 0.0)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The typed capacity is not a number", ConsoleColor.Black, ConsoleColor.Yellow);
                    newDriversVehicle.Capacity = 0.0;
                    CLISystem.SkipRow();
                }
                else if (newDriversVehicle.Type != Vehicle.Types.Cargo && newDriversVehicle.Capacity >= 500.0)
                {
                    CLISystem.ShowStylizedMessage(true, "ERROR: The chosen vehicle type does not match with the capacity (Must be a \"Cargo\" vehicle)", ConsoleColor.Black, ConsoleColor.Yellow);
                    CLISystem.SkipRow();

                    if (CLISystem.YNQuestion("Want to", "Change the vehicle type to \"Cargo\""))
                    {
                        newDriversVehicle.Type = Vehicle.Types.Cargo;
                    }
                    else
                    {
                        newDriversVehicle.Capacity = 0.0;
                    }
                    CLISystem.SkipRow();
                }
            } while (newDriversVehicle.Capacity == 0.0);

            newDriversVehicle.Owner = Program.loggedUser;

            CLISystem.SkipRow();

            CLISystem.CreateInfoBox(
                $"{Program.loggedUser.userID} {newDriversVehicle.Model}", 
                new string[] {
                    $"Model {newDriversVehicle.Model}",
                    $"Year: {newDriversVehicle.Year}",
                    $"License Plate: {newDriversVehicle.LicensePlate}", 
                    $"Capacity: {newDriversVehicle.Capacity}",
                    $"Vehicle Type: {newDriversVehicle.Type}"
                }, keepOpen: true);
            CLISystem.CreateInfoBox("", new string[] { $"Owner: {newDriversVehicle.Owner.userID}", }, isContinuation: true);

            CLISystem.SkipRow();
            
            Database.RegistryNewVehicle(Program.loggedUser.userID, newDriversVehicle);
            
            CLISystem.ShowStylizedMessage(true, $"New vehicle for {Program.loggedUser.userID} registered successfully!", ConsoleColor.Black, ConsoleColor.Green);

            if (CLISystem.YNQuestion("Want to", "Back to menu"))
            {
                Console.Clear();
                DriverMenu.Initialize();
            }
        }
    }
}
