using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace AutoSync.CLI
{
    class CLISystem
    {
        public static void SkipRow()
        {
            Console.WriteLine("");
        }

        public static void ShowStylizedMessage(bool breakLine, string message, ConsoleColor backgroundColor = ConsoleColor.Black, ConsoleColor textColor = ConsoleColor.White)
        {
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = textColor;

            if (!breakLine) {
                Console.Write(message);
            }
            else
            {
                Console.WriteLine(message);
            }

            Console.ResetColor();
        }

        public static bool YNQuestion(string confirmationMessage, string action)
        {
            SkipRow();
            ShowStylizedMessage(true, $"{confirmationMessage} {action}? (Y/N)", ConsoleColor.Black, ConsoleColor.Blue);

            string input = Console.ReadLine() ?? "N";

            if (input.ToLower() == "y")
            {
                return true;
            }
            else if (input.ToLower() == "n")
            {
                return false;
            }

            SkipRow();
            ShowStylizedMessage(true, "Unrecognized answer. Action canceled.", ConsoleColor.Yellow, ConsoleColor.Black);
            return false;
        }

        public static DateTime ReadDateInput()
        {
            string[] formats = { "dd/MM/yyyy", "dd-MM-yyyy" };
            
            string input = Console.ReadLine()?.Trim();

            if (DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime data))
            {
                return data;
            }

            return DateTime.MinValue;
        }

        public static void CreateInfoBox(string title, string[] lines,
            bool isContinuation = false,  // if true, continue drawing the box (draws ├───┤ instead of ┌───┐)
            bool keepOpen = false,        // if true, won't draw bottom borders
            int width = 40)
        {
            int innerSpace = width - 2;

            // Up borders
            if (!isContinuation)
            {
                Console.WriteLine("┌" + new string('─', innerSpace) + "┐");
            } 
            else
            {
                Console.WriteLine("├" + new string('─', innerSpace) + "┤");
            }
                
            // Title (if has one)
            if (!string.IsNullOrEmpty(title))
            {
                int leftSpace = (innerSpace - title.Length) / 2;
                int rightSpace = innerSpace - title.Length - leftSpace; // corrige o caso ímpar

                Console.WriteLine("│" + new string(' ', leftSpace) + title + new string(' ', rightSpace) + "│");
                Console.WriteLine("├" + new string('─', innerSpace) + "┤");
            }

            // Content aligned to the left
            foreach (string contentLine in lines)
            {
                string text = " " + contentLine;
                if (text.Length > innerSpace)
                {
                    text = text.Substring(0, innerSpace);
                }   

                Console.WriteLine("│" + text.PadRight(innerSpace) + "│");
            }

            // Bottom border if is the end of the info
            if (!keepOpen)
            {
                Console.WriteLine("└" + new string('─', innerSpace) + "┘");
            }
        }

        public static void OptionsList(string optionsTitleMessage, Dictionary<string, Action> actionsList)
        {
            CLISystem.ShowStylizedMessage(true, optionsTitleMessage, ConsoleColor.Yellow, ConsoleColor.Black);
            CLISystem.SkipRow();

            foreach (string actionName in actionsList.Keys)
            {
                Console.WriteLine(actionName);
            }

            string actionInput = Console.ReadLine() ?? "";

            if (int.TryParse(actionInput, out int actionIDX))
            {
                if (actionsList.Values.ToList().Count >= actionIDX && actionIDX > 0)
                {
                    Action action = actionsList.Values.ToList()[actionIDX - 1];
                    if (action != null)
                    {
                        action.Invoke();
                    }
                }
            }
        }

        public static double FormatedDoubleValue(string value)
        {
            try
            {
                double formatedValue = double.Parse(value.Replace(",", "."), CultureInfo.InvariantCulture);
                return formatedValue;
            }
            catch (FormatException)
            {
                return 0.0;
            }
        }
    }
}
