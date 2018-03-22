using System;
using System.Collections.Generic;

namespace CryptScript
{
    public static class Data
    {
        public static List<Command> MainCommands = new List<Command>
        {
            // Command: Mine -> Enters the mining dialogue
            new Command("mine", "Enters the mining dialogue", delegate(string[] input, ref bool isHandled)
            {
                if(Command.CheckIfValid(input, "mine", ref isHandled))
                {
                    Display.Update(DisplayMode.Mining);
                    Console.WriteLine("Mining has started.");
                }
            }),

            // Command: Help -> Displays all commands and their descriptions
            new Command("help", "Displays this message", delegate(string[] input, ref bool isHandled)
            {
                if(Command.CheckIfValid(input, "help", ref isHandled))
                {
                    Util.WriteLineColor("Below is a list of commands:", ConsoleColor.Yellow);

                    int tabinate = 0;
                    foreach(var command in Display.Input.Commands)
                        tabinate = Math.Max(tabinate, command.Item1.Length);

                    foreach(var command in Display.Input.Commands)
                    {
                        Util.WriteColor("    " + command.Item1, ConsoleColor.Yellow);

                        for(int i = command.Item1.Length; i < tabinate; i++)
                            Console.Write(" ");

                        Util.WriteLineColor(" -> " + command.Item2, ConsoleColor.Yellow);
                    }
                }
            }),

            // Command: Clear -> Clears the console window
            new Command("clear", "Clears the console window", delegate(string[] input, ref bool isHandled)
            {
                if(Command.CheckIfValid(input, "clear", ref isHandled))
                {
                    Console.Clear();
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

        public static List<Command> MiningCommands = new List<Command>
        {
            new Command("display", "Shows a realtime hash rate display", delegate(string[] input, ref bool isHandled)
            {
                if(Command.CheckIfValid(input, "display", ref isHandled))
                {
                    Alert.Info("This command is not implemented yet.");
                }
            }),

            new Command("stop", "Stops mining", delegate(string[] input, ref bool isHandled)
            {
                if(Command.CheckIfValid(input, "stop", ref isHandled))
                {
                    Console.WriteLine("Mining has stopped.");
                    Display.Update(DisplayMode.Main);
                }
            })
        };
    }
}
