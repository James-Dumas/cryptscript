using System;
using System.Collections.Generic;

namespace CryptScript
{
    public class Command
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public UserInput.InputHandler Function { get; set; }

        public delegate void Implementation(string[] input);

        public Command(string name, string description, Implementation function)
        {
            Name = name.ToLower();
            Description = description;
            Function = delegate(string[] input, ref bool isHandled)
            {
                if (CheckIfValid(input, Name, ref isHandled))
                {
                    function(input);
                }
            };
        }

        #region Static Command Logic

        public static bool CheckIfValid(string[] command, string match, ref bool isHandled)
        {
            // Check if the command matches the request
            if (!isHandled && command[0].ToLower() == match.ToLower())
            {
                isHandled = true;
                return true;
            }

            // Return false if the command doesn't match or if the request has already been handled
            return false;
        }

        public static void Subscribe(UserInput input, UserInput.InputHandler handler) =>
            input.HandleInput += handler;

        #endregion
    }
}
