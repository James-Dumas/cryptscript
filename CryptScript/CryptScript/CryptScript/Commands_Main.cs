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
        /// List of commands used in the main dialogue
        /// </summary>
        public static List<Command> MainCommands = new List<Command>
        {
            // Command: Mine -> Enters the mining dialogue
            new Command("mine", "Enters the mining dialogue", delegate(string[] input, ref bool isHandled)
            {
                if(Command.CheckIfValid(input, "mine", ref isHandled))
                {
                    Display.Update(DisplayMode.Mining);
                }
            }),

            // Command: Code -> Enters the coding dialogue
            new Command("code", "Enters the coding dialogue", delegate(string[] input, ref bool isHandled)
            {
                if(Command.CheckIfValid(input, "code", ref isHandled))
                {
                    Display.Update(DisplayMode.Coding);
                }
            })
        };
    }
}
