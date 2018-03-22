using System;
using System.Collections.Generic;

namespace CryptScript
{
    public static class Util
    {
        /// <summary>
        /// Prompts the user for a string input
        /// </summary>
        /// <param name="message">The message to prompt the user with</param>
        public static string PromptForString(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.Gray;
            string output = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
            return output;
        }

        /// <summary>
        /// Subscribes the given list of commands to the given user input stream
        /// </summary>
        /// <param name="commands">The list of subscribing commands</param>
        /// <param name="input">The user input stream to subscribe to</param>
        public static void SubscribeCommands(List<Command> commands, UserInput input)
        {
            foreach (var command in commands)
            {
                input.HandleInput += command.Function;
                input.Commands.Add(new Tuple<string, string>(command.Name, command.Description));
            }
        }

        /// <summary>
        /// Writes to the console in a certain color
        /// </summary>
        /// <param name="message">The message to write to the console</param>
        /// <param name="color">The color to write in</param>
        public static void WriteColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Writes to the console in a certain color, followed by a new line
        /// </summary>
        /// <param name="message">The message to write to the console</param>
        /// <param name="color">The color to write in</param>
        public static void WriteLineColor(string message, ConsoleColor color) =>
            WriteColor(message + Environment.NewLine, color);
    }
}
