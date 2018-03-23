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

        /// <summary>
        /// Prints the current display mode
        /// </summary>
        public static void PrintMode()
        {
            // Save the current console settings
            ConsoleColor initialForeground = Console.ForegroundColor;
            ConsoleColor initialBackground = Console.BackgroundColor;
            int line = Console.CursorTop;

            // Initialize display settings
            string modeText = Display.Mode.ToString().ToUpper();
            ConsoleColor newForeground = initialForeground;
            ConsoleColor newBackground = initialBackground;

            // Get the new foreground and background colors
            switch(Display.Mode)
            {
                case DisplayMode.Main:
                    newForeground = ConsoleColor.Black;
                    newBackground = ConsoleColor.Cyan;
                    break;

                case DisplayMode.Mining:
                    newForeground = ConsoleColor.Black;
                    newBackground = ConsoleColor.Green;
                    break;
            }

            // If the log has reached the bottom, reset the display
            if (line == Console.WindowHeight - 1)
            {
                line = 0;
                Console.Clear();
            }

            // Clear the area to be printed
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.BackgroundColor = newBackground;
            for (int i = 0; i < Console.WindowWidth; i++)
                Console.Write(' ');

            // Print the mode
            Console.SetCursorPosition((Console.WindowWidth / 2) - (modeText.Length / 2) - 1, Console.WindowHeight - 1);
            Console.ForegroundColor = newForeground;
            Console.BackgroundColor = newBackground;
            Console.Write(" {0} ", modeText);


            // Return to the initial console configuration
            Console.ForegroundColor = initialForeground;
            Console.BackgroundColor = initialBackground;

            Console.SetCursorPosition(0, line);
        }
    }
}
