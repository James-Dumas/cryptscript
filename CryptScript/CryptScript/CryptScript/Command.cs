using System;

namespace CryptScript
{
    public class Command
    {
        private static bool CheckIfValid(string[] command, string match, ref bool isHandled)
        {
            isHandled = !isHandled && command[0].ToLower() == match.ToLower();
            return isHandled;
        }

        public static void Subscribe(UserInput input, UserInput.InputHandler handler) =>
            input.HandleInput += handler;

        public static void SubscribeAll(UserInput input)
        {
            input.HandleInput += Clear;
            input.HandleInput += Exit;
        }

        public static void Clear(string[] command, ref bool isHandled)
        {
            if(CheckIfValid(command, "clear", ref isHandled))
            {
                Console.Clear();
            }
        }

        public static void Exit(string[] command, ref bool isHandled)
        {
            if(CheckIfValid(command, "exit", ref isHandled))
            {
                Program.IsRunning = false;
            }
        }
    }
}
