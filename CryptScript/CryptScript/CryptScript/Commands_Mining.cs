using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
                    if(Program.IsMining)
                    {
                        if(Monitor.Wallet == "")
                        {
                            Display.Write("Please input a wallet ID.", ConsoleColor.Yellow);
                            Display.PrintLayout();
                            Display.PrintMessages();
                            Monitor.Wallet = Display.GetInput();
                        }

                        Thread tempminer = new Thread(new ThreadStart(() =>
                        {
                            Random rand = new Random();
                            while(Program.IsMining)
                            {
                                Monitor.HashRate++;
                                System.Threading.Thread.Sleep(rand.Next(0, 5));
                            }
                        }));
                        tempminer.Start();
                        Monitor.Display();
                        tempminer.Abort();
                    }
                    else
                    {
                        Alert.Error("Miner is not active.");
                    }
                }
            }),
            
            new Command("start", "Starts the miner", delegate(string[] input, ref bool isHandled)
            {
                if(Command.CheckIfValid(input, "start", ref isHandled))
                {
                    Program.IsMining = true;
                    Display.Write("Mining has started.", ConsoleColor.Green);
                }
            }),
            
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
