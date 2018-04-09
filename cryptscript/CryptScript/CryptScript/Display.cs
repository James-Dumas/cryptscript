using System;
using System.Collections.Generic;

namespace CryptScript
{
    public static class Display
    {
        public static UserInput Input { get; set; } = new UserInput();
        public static DisplayMode Mode { get; private set; }
        public static List<Tuple<string, ConsoleColor>> Messages = new List<Tuple<string, ConsoleColor>>();

        /// <summary>
        /// The list of command sets to be used with each type of display mode
        /// </summary>
        private static Dictionary<DisplayMode, List<List<Command>>> ModeCommands = new Dictionary<DisplayMode, List<List<Command>>>()
        {
            { DisplayMode.Main, new List<List<Command>> { Data.MainCommands, Data.StockCommands } },
            { DisplayMode.Mining, new List<List<Command>> { Data.MiningCommands, Data.StockCommands } },
            { DisplayMode.Coding, new List<List<Command>> { Data.CodingCommands, Data.StockCommands } }
        };

        internal static void PrintMessages()
        {
            // Remove excess messages
            while(Messages.Count > Console.WindowHeight - 4) Messages.RemoveAt(0);

            // Print in reverse
            for(int i = 0; i < Messages.Count; i++)
            {
                int index = Messages.Count - 1 - i;
                Console.SetCursorPosition(0, Console.WindowHeight - 3 - i);
                Util.WriteColor(Messages[index].Item1, Messages[index].Item2);
            }
        }

        public static string GetInput()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            return Util.PromptForString(">>> ");
        }

        /// <summary>
        /// Prints the layout of the display to the console window
        /// </summary>
        public static void PrintLayout()
        {
            Console.Clear();

            // Get the top bar's color and text
            ConsoleColor color = Mode == DisplayMode.Coding
                ? ConsoleColor.Magenta
                : Program.IsMining
                    ? ConsoleColor.Green
                    : ConsoleColor.Cyan;
            string modeText = Mode.ToString().ToUpper();

            // Set the colors to write in
            Console.BackgroundColor = color;
            Console.ForegroundColor = ConsoleColor.Black;

            // Print the top bar
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < Console.WindowWidth; i++)
                Console.Write(' ');
            Console.SetCursorPosition((Console.WindowWidth / 2) - (modeText.Length / 2), 0);
            Console.Write(modeText);

            // Switch the colors
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = color;

            // Print the bottom separator
            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            for (int i = 0; i < Console.WindowWidth; i++)
                Console.Write('\u2550');

            Console.ResetColor();
        }

        /// <summary>
        /// Updates the display to use the proper commands
        /// </summary>
        /// <param name="mode">The mode to set the display to</param>
        public static void Update(DisplayMode mode)
        {
            // Reset user input commands
            Mode = mode;
            Input = new UserInput();
            if (ModeCommands.ContainsKey(mode))
            {
                foreach (List<Command> CommandList in ModeCommands[mode])
                {
                    Util.SubscribeCommands(CommandList, Input);
                }
            }

            Write("Switched to the " + Mode.ToString().ToLower() + " dialogue.", ConsoleColor.DarkYellow);
        }

        public static void Write(string message, ConsoleColor color) =>
            Messages.Add(new Tuple<string, ConsoleColor>(message, color));

        public static void Clear() => Messages = new List<Tuple<string, ConsoleColor>>();
    }

    public enum DisplayMode
    {
        Main,
        Mining,
        Coding
    }
}
