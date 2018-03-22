using System;

namespace CryptScript
{
    public static class Alert
    {
        public static void Error(string message) =>
            Util.WriteLineColor("Error: " + message, ConsoleColor.Red);

        public static void Warning(string message) =>
            Util.WriteLineColor("Warning: " + message, ConsoleColor.Yellow);

        public static void Info(string message) =>
            Util.WriteLineColor("Info: " + message, ConsoleColor.DarkGray);
    }
}
