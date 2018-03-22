using System;
using System.Collections.Generic;

namespace CryptScript
{
    public class UserInput
    {
        public event InputHandler HandleInput;
        public delegate void InputHandler(string[] input, ref bool isHandled);

        public List<string[]> Inputs = new List<string[]>();

        public void Parse(string input)
        {
            // Reset Inputs
            Inputs = new List<string[]>();

            // Get individual command segments
            string[] commands = input.Split(new string[] { "&&" }, new StringSplitOptions());

            // Split each command segment into its primary command and its parameters
            for(int i = 0; i < commands.Length; i++)
                Inputs.Add(commands[i].Split(' '));

            // Handle each command segment
            foreach(string[] command in Inputs)
            {
                bool isHandled = false;

                HandleInput?.Invoke(command, ref isHandled);

                if(!isHandled)
                {
                    Console.WriteLine("Error: \"{0}\" is not a valid command.", command[0]);
                    break;
                }
            }
        }
    }
}
