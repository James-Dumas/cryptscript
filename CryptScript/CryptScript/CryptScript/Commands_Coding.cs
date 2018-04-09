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
        /// List of commands used in the coding dialogue
        /// </summary>
        public static List<Command> CodingCommands = new List<Command>
        {
            // Command: Interpreter -> Opens the CryptScript interpreter
            new Command("csi", "Opens the CryptScript interpreter", delegate(string[] input)
            {
                Alert.Info("Command not yet implemented.");
            }),

            // Command: Run -> Runs an existing CryptScript file
            new Command("run", "Runs an existing CryptScript file", delegate(string[] input)
            {
                Alert.Info("Command not yet implemented.");
            }),

            // Command: Return -> Returns to the main dialogue
            new Command("ret", "Exits coding mode", delegate(string[] input)
            {
                Display.Update(DisplayMode.Main);
            })
        };
    }
}
