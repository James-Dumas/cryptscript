using System;
using System.Collections.Generic;

namespace CryptScript
{
    public static partial class Data
    {
        /// <summary>
        /// List of stock commands, should be included in most dialogues
        /// </summary>
        public static List<Command> StockCommands = new List<Command>
        {
            // Command: Help -> Displays all commands and their descriptions
            new Command("help", "Displays this message", delegate(string[] input, ref bool isHandled)
            {
                if(Command.CheckIfValid(input, "help", ref isHandled))
                {
                    Display.Write("Commands for the current dialogue:", ConsoleColor.Yellow);

                    int tabinate = 0;
                    foreach(var command in Display.Input.Commands)
                        tabinate = Math.Max(tabinate, command.Item1.Length);

                    foreach(var command in Display.Input.Commands)
                    {
                        string output =  "    " + command.Item1;

                        for(int i = command.Item1.Length; i < tabinate; i++)
                            output += ' ';

                        output += " -> " + command.Item2;

                        Display.Write(output, ConsoleColor.Yellow);
                    }
                }
            }),

            // Command: Clear -> Clears the console window
            new Command("clear", "Clears the console window", delegate(string[] input, ref bool isHandled)
            {
                if(Command.CheckIfValid(input, "clear", ref isHandled))
                {
                    Display.Clear();
                }
            }),

            // Command: Exit -> Exits the program
            new Command("exit", "Exits the program", delegate(string[] input, ref bool isHandled)
            {
                if(Command.CheckIfValid(input, "exit", ref isHandled))
                {
                    Program.IsRunning = false;
                }
            })
        };
    }
}
