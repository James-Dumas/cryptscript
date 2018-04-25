using System;
using System.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace cryptscript
{
    public static class BetterReadline
    {
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
            Console.TreatControlCAsInput = true;
            var queue = new BlockingCollection<ConsoleKeyInfo>();
            EventWaitHandle LoopExit = new AutoResetEvent(false);
            EventWaitHandle StartRead = new ManualResetEvent(false);
            bool interrupted = false;

            new Thread(() =>
            {
                while(true) 
                {
                    StartRead.Reset();
                    StartRead.WaitOne();
                    if(interrupted) { break; }
                    queue.Add(Console.ReadKey(true));
                }

                LoopExit.Set();
            }
            ) { IsBackground = true }.Start();

            StringBuilder input = new StringBuilder();
            ConsoleKeyInfo cki;
            StartRead.Set();
            while(true)
            {
                if(queue.TryTake(out cki, TimeSpan.FromMilliseconds(20)))
                {
                    if(cki.Key == ConsoleKey.Enter)
                    {
                        break;
                    }

                    if((cki.Modifiers & ConsoleModifiers.Control) != 0 && cki.Key == ConsoleKey.C)
                    {
                        Interpreter.ThrowError(new Error(ErrorType.KeyboardInterrupt));
                        break;
                    }

                    if(cki.Key == ConsoleKey.Backspace)
                    {
                        if(input.Length > 0)
                        {
                            input.Remove(input.Length - 1, 1);
                            Console.Write("\b \b");
                        }
                    }
                    else if(!IgnoredKeys.Contains(cki.Key))
                    {
                        input.Append(cki.KeyChar);
                        Console.Write(cki.KeyChar);
                    }
                }
                StartRead.Set();
            }

            interrupted = true;
            Thread.Sleep(1);
            StartRead.Set();
            LoopExit.WaitOne();
            Console.WriteLine();
            Console.TreatControlCAsInput = false;

            return input.ToString();
        }
    }
}