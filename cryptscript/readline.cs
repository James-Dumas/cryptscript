using System;
using System.Collections.Generic;
using System.Threading;

namespace cryptscript
{
    public static class BetterReadline
    {
        private static bool Interrupted = false;
        private static List<ConsoleKey> IgnoredKeys = new List<ConsoleKey>()
        {
            ConsoleKey.LeftArrow,
            ConsoleKey.RightArrow,
            ConsoleKey.DownArrow,
            ConsoleKey.UpArrow,
            ConsoleKey.Delete,
            ConsoleKey.Insert,
            ConsoleKey.Escape,
            ConsoleKey.Enter,
        };

        public static string Readline()
        {
            Interrupted = false;
            ConsoleKeyInfo keyInfo;
            string input = "";
            do
            {
                while(!Console.KeyAvailable)
                {
                    Thread.Sleep(10);
                    if(Interrupted)
                    {
                        Console.WriteLine();
                        return null;
                    }
                }

                keyInfo = Console.ReadKey(true);
                if(keyInfo.Key == ConsoleKey.Backspace)
                {
                    if(input.Length > 0)
                    {
                        input = input.Remove(input.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
                else if(!IgnoredKeys.Contains(keyInfo.Key))
                {
                    input += keyInfo.KeyChar;
                    Console.Write(keyInfo.KeyChar);
                }
            }
            while(!Interrupted && keyInfo.Key != ConsoleKey.Enter);
            Console.WriteLine();

            return input;
        }

        public static void Interrupt()
            => Interrupted = true;
    }
}