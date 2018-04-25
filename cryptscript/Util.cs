using System;
using System.Collections.Generic;
using System.Text;

namespace cryptscript
{
    public static class Util
    {
        /// <summary>
        /// Prompts the user for a string input
        /// </summary>
        public static string GetInput(string prompt)
        {
            Console.Write(prompt);
            string input = BetterReadline.Readline();

            return input == null ? "" : input.Trim();
        }

        public static string EscapeString(string literal)
        {
            string escaped = literal.Replace(@"\a", "\a");
            escaped = escaped.Replace(@"\b", "\b");
            escaped = escaped.Replace(@"\f", "\f");
            escaped = escaped.Replace(@"\n", "\n");
            escaped = escaped.Replace(@"\r", "\r");
            escaped = escaped.Replace(@"\t", "\t");
            escaped = escaped.Replace(@"\v", "\v");
            escaped = escaped.Replace(@"\\", "\\");
            escaped = escaped.Replace(@"\'", "\'");
            escaped = escaped.Replace(@"\""", "\"");

            return escaped;
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
            string disp = obj.ToString();
            if(obj is IObject)
            {
                disp = ((IObject) obj).Repr();
            }

            Console.WriteLine("[DEBUG] " + disp);
        }
    }
}
