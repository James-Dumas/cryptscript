using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptScript
{
    public static partial class Data
    {
        /// <summary>
        /// List of commands used in the mining dialogue
        /// </summary>
        public static List<Command> MiningCommands = new List<Command>
        {
            // Command: Display -> Shows the mining display, WIP
            new Command("display", "Shows a realtime hash rate display", delegate(string[] input, ref bool isHandled)
            {
                if(Command.CheckIfValid(input, "display", ref isHandled))
                {
                    Alert.Info("This command is not implemented yet.");
                }
            }),

            // Command: Display -> Shows the mining display, WIP
            new Command("start", "Starts the miner", delegate(string[] input, ref bool isHandled)
            {
                if(Command.CheckIfValid(input, "start", ref isHandled))
                {
                    Program.IsMining = true;
                    Display.Write("Mining has started.", ConsoleColor.Green);
                }
            }),

            // Command: Display -> Shows the mining display, WIP
            new Command("stop", "Stops the miner", delegate(string[] input, ref bool isHandled)
            {
                if(Command.CheckIfValid(input, "stop", ref isHandled))
                {
                    Program.IsMining = false;
                    Display.Write("Mining has stopped.", ConsoleColor.Green);
                }
            }),

            // Command: Return -> Returns to the main dialogue
            new Command("ret", "Returns to the main dialogue", delegate(string[] input, ref bool isHandled)
            {
                if(Command.CheckIfValid(input, "ret", ref isHandled))
                {
                    Display.Update(DisplayMode.Main);
                }
            })
        };
    }
}
