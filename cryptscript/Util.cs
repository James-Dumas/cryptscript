using System;
using System.Collections.Generic;
using System.Text;

namespace cryptscript
{
    class Util
    {
        /// <summary>
        /// Prompts the user for a string input
        /// </summary>
        public static string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine().Trim();
        }

        /// <summary>
        /// Prompts the user for a key input
        /// </summary>
        public static ConsoleKey GetKey(string prompt) => GetKey(prompt, false);

        /// <summary>
        /// Prompts the user for a key input
        /// </summary>
        public static ConsoleKey GetKey(string prompt, bool intercept)
        {
            Console.Write(prompt);
            return Console.ReadKey(intercept).Key;
        }

        public static void WriteColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void DebugLog(object obj)
        {
            Console.WriteLine("[DEBUG] " + obj.ToString());
        }
    }
}
