using System;
using System.Collections.Generic;

namespace CryptScript
{
    public class UserInput
    {
        public event InputHandler HandleInput;
        public delegate void InputHandler(string[] input, ref bool isHandled);

        public List<Tuple<string, string>> Commands;

        public List<string[]> Inputs { get; set; } = new List<string[]>();

        public void Parse(string input)
        {
            // Don't attempt to parse if the input is empty
            if (input.Trim() == "")
                return;

            // Get command segments
            Inputs = new List<string[]>();
            string[] segments = input.Trim().Split(new string[] { "&&" }, new StringSplitOptions());
            for(int i = 0; i < segments.Length; i++)
            {
                Inputs.Add(segments[i].Trim().Split(' '));
            }

            // Execute each command in sequence
            foreach(string[] segment in Inputs)
            {
                bool isHandled = false;

                HandleInput?.Invoke(segment, ref isHandled);

                if (!isHandled)
                {
                    Alert.Error("\"" + segment[0] + "\" is not a valid command.");
                    break;
                }
            }
        }
    }
}
