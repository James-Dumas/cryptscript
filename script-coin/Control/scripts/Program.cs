using System;
using System.IO;
using System.Threading;

namespace scriptcoin
{
    public class Program
    {
        /// <summary>
        /// Gets the directory of the local node document
        /// </summary>
        public static string LocalNodeDir =>
            Path.Combine(Directory.GetParent(Environment.CurrentDirectory).ToString(), "Documents");
        
        /// <summary>
        /// Version string
        /// </summary>
        public static string Version = "v0.0.1";

        /// <summary>
        /// Exit variable
        /// </summary>
        public static bool Quit = false;

        public static void Main()
        {
            //Blockchain.Connect("ping");

            // Display version number
            Util.WriteLineColor("ScriptCoin " + Version, ConsoleColor.Cyan);
                                    
            while(!Quit)
            {
                // Get user input
                Console.Write(">>> ");
                string[] input = Console.ReadLine().Split(' ');
                string primaryCommand = input[0].ToLower().Trim();

                // Only check command if input string has content
                if (primaryCommand.Length != 0)
                {
                    // Check if command exists
                    bool commandExists = false;
                    int commandIndex = -1;
                    for (int i = 0; i < Util.Commands.Count; i++)
                    {
                        commandExists = primaryCommand == Util.Commands[i].Name || commandExists;
                        commandIndex = i;
                        if (commandExists) break;
                    }

                    // Execute the command
                    if (commandExists)
                        Util.Commands[commandIndex].Execute();
                    else
                        Util.PrintError("\"" + primaryCommand + "\" is not a valid command.");
                }
            }
        }
    }
}