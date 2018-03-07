using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ScriptCoin
{
    public class UserInterface
    {
        static void Run()
        {
            Initialize:
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Type in a command or use `help` for more information.");
            Console.ForegroundColor = ConsoleColor.White;
            string UserInput = Console.ReadLine().ToLower();

            if (Commands.ContainsKey(UserInput))
            {
                if (UserInput == "help")
                {
                    Console.WriteLine("Below are a list of commands and their use;");
                    foreach (var pair in Commands)
                    {
                        Console.WriteLine("{0},{1}", pair.Key, pair.Value);
                    }
                }
                if (UserInput == "new")
                {
                    Tuple<string, string> keys = WalletGen.WalletHash();
                    string pubKey = keys.Item1;
                    string privKey = keys.Item2;

                    Console.WriteLine("Your public address is; " + pubKey);
                    Console.WriteLine("Your private address is; " + privKey);
                }
                if (UserInput == "send")
                {
                    Console.WriteLine("This hasn't been implemented yet");
                }
                if (UserInput == "wallet")
                {
                    Console.WriteLine("This hasn't been impleneted yet");
                }
                if (UserInput == "code")
                {
                    Console.WriteLine("This hasn't been implemented yet");
                }
            }
            else
            {
                Console.WriteLine("Unknown command, use `help` for more information");
            }

            goto Initialize;
        }

        public static Dictionary<string, string> Commands = new Dictionary<string, string>()
        {
            {"new", " Creates and displays information about new wallet "},
            {"send", " Send Script-Coin to a destination "},
            {"wallet", " Displays information about current wallet "},
            {"code", " Opens programming window "},
            {"yes", " Yes "},
            {"no", " No "},
            {"help", " Displays help menu "},
        };
    }
}