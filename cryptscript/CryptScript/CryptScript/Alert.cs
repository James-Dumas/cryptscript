using System;

namespace CryptScript
{
    public static class Alert
    {
        public static void Error(string message) =>
            Display.Write("Error: " + message, ConsoleColor.Red);

        public static void Warning(string message) =>
            Display.Write("Warning: " + message, ConsoleColor.Yellow);

        public static void Info(string message) =>
            Display.Write("Info: " + message, ConsoleColor.DarkGray);
    }
}
