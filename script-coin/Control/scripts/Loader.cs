using System;
using System.IO;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ScriptCoin
{
    public class UserInterface
    {
        public static void Main()
        {
            string path = @"C:\Users\WollaMat\Documents\GitHub\cryptscript\script-coin\Control\documents\localNode.txt";

            Initialize:
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Type in a command or use `help` for more information.");
            Console.ForegroundColor = ConsoleColor.White;

            string UserInput = Console.ReadLine().ToLower();

            if (Dictionaries.Commands.ContainsKey(UserInput))
            {
                if (UserInput == "help")
                {
                    Console.WriteLine("Below are a list of commands and their use;");
                    foreach (var pair in Dictionaries.Commands)
                    {
                        Console.WriteLine("{0},{1}", pair.Key, pair.Value);
                    }
                }
                if (UserInput == "new")
                {
                    Tuple<string, string> keys = Encryptor.Encrypt();
                    string pubKey = keys.Item1;
                    string privKey = keys.Item2;

                    Console.WriteLine("Your public address is; " + pubKey);
                    Console.WriteLine("Your private address is; " + privKey);
                }
                if (UserInput == "send")
                {
                    string userInfo = Encryptor.Decrypt();
                    if (userInfo == "valid")
                    {
                        Console.WriteLine("Where do you want to send Script-Coins to?");
                        string scriptDest = Console.ReadLine();
                        Console.WriteLine("How many Script-Coins do you want to send?");
                        string scriptAmount = Console.ReadLine();

                        Console.WriteLine("Are you sure you want to send " + scriptAmount + " Script-Coins to " + scriptDest + " yes - no");
                        string userVerify = Console.ReadLine().ToLower();
                        if (userVerify == "yes")
                        {
                            long blockTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                            string sendAmount = blockTime + "=" + scriptDest + "=" + scriptAmount;
                            using (StreamWriter hash = File.AppendText(path))
                            {
                                hash.WriteLine(sendAmount);
                            }
                        }
                        else Console.WriteLine("The transaction has be cancelled");
                    }
                }
                if (UserInput == "wallet")
                {
                    string userInfo = Encryptor.Decrypt();
                    Console.WriteLine(userInfo);
                }
                if (UserInput == "mine")
                {
                    Console.WriteLine("Please enter your public address;");
                    string userID = Console.ReadLine();
                    
                    while (!Console.KeyAvailable)
                    {                        
                        string Hash = Miner.Hasher() + userID + "=1"; 

                        using (StreamWriter hash = File.AppendText(path))
                        {
                            hash.WriteLine(Hash);
                        }

                        Console.WriteLine(Hash + userID);
                    }
                }
                if (UserInput == "code")
                {
                    Console.WriteLine("This hasn't been implemented yet");
                }
                if (UserInput == "joke")
                {
                    Random rnd = new Random();
                    int joke = rnd.Next(Dictionaries.Jokes.Count);
                    Console.WriteLine((string)Dictionaries.Jokes[joke]);
                }
            }
            else
            {
                Console.WriteLine("Unknown command, use `help` for more information");
            }

            goto Initialize;
        }
    }
}