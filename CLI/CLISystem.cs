using System;
using System.Collections.Generic;
using System.Drawing;
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
    }
}
