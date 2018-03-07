using System;

namespace Script_Coin
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("ScriptCoin Console a1.0");
            
            for(bool quit = false; !quit;)
            {
                // Get user input
                Console.Write(">>> ");
                string[] userInput = Console.ReadLine().Split(' ');
                Command parsedCommand = GetCommand(userInput[0]);

                // Execute command
                switch(parsedCommand)
                {
                    case Command.NewWallet:
                        Console.WriteLine("New wallet not yet implemented.");
                        break;

                    case Command.Send:
                        // Get destination wallet ID
                        Console.Write("Destination wallet ID: ");
                        string destinationWallet = Console.ReadLine().Trim();

                        // Get amount of ScriptCoin to send
                        Console.Write("Transaction Amount: ");
                        double transactionAmount = Convert.ToDouble(Console.ReadLine().Trim());

                        // Confirm transaction
                        Console.Write("Sending {0} ScriptCoin to wallet #{1}. Confirm? [y/N]",
                            transactionAmount, destinationWallet);
                        switch(Console.ReadKey().Key)
                        {
                            case ConsoleKey.Y:
                                Console.WriteLine("Transaction complete.");
                                break;
                            default:
                                Console.WriteLine("Transaction cancelled.");
                                break;
                        }
                        break;

                    case Command.WalletInformation:
                        Console.WriteLine("Wallet information not yet implemented.");
                        break;

                    case Command.Code:
                        Console.WriteLine("Code editing not yet implemented.");
                        break;

                    case Command.Help:
                        WriteColorLine("Command documentation for ScriptCoin:\n",          ConsoleColor.Yellow);
                        WriteColorLine("new, -n    -> Create a new wallet",                ConsoleColor.Yellow);
                        WriteColorLine("send, -s   -> Send ScriptCoin to another wallet",  ConsoleColor.Yellow);
                        WriteColorLine("wallet, -w -> Displays your wallet's information", ConsoleColor.Yellow);
                        WriteColorLine("code, -c   -> Code editor",                        ConsoleColor.Yellow);
                        WriteColorLine("clear, -l  -> Clears the screen",                  ConsoleColor.Yellow);
                        WriteColorLine("help, -h   -> Displays this message",              ConsoleColor.Yellow);
                        WriteColorLine("quit, -q   -> Quit the console",                   ConsoleColor.Yellow);
                        break;

                    case Command.Exit:
                        quit = true;
                        break;

                    case Command.Clear:
                        Console.Clear();
                        break;

                    default:
                        WriteColorLine("Error: \"" + userInput[0] + "\" is not a valid command. Type \"help\" for more information.", ConsoleColor.Red);
                        break;
                }
            }
        }

        static Command GetCommand(string input)
        {
            switch(input)
            {
                case "-n":
                case "new":
                    return Command.NewWallet;
                case "-s":
                case "send":
                    return Command.Send;
                case "-w":
                case "wallet":
                    return Command.WalletInformation;
                case "-c":
                case "code":
                    return Command.Code;
                case "-h":
                case "help":
                    return Command.Help;
                case "-q":
                case "quit":
                    return Command.Exit;
                case "clear":
                case "cls":
                    return Command.Clear;
                default:
                    return Command.Error;
            }
        }

        static void WriteColor(string input, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(input);
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void WriteColorLine(string input, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(input);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public enum Command
        {
            NewWallet,
            Send,
            WalletInformation,
            Code,
            Help,
            Exit,
            Clear,
            Error
        }
    }
}
