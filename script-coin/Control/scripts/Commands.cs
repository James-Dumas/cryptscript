using System;
using System.IO;

namespace scriptcoin
{
    public abstract class Command
    {
        public virtual void Execute() => Util.PrintInfo("This command has not yet been implemented.");
        public virtual string Name { get; }
        public virtual string Description { get; }
    }

    public class Help : Command
    {
        public override void Execute()
        {
            // Get spacing for tabination
            int tabination = 0;
            foreach (Command command in Util.Commands)
                tabination = command.Name.Length > tabination
                    ? command.Name.Length
                    : tabination;

            // Print help message
            Util.WriteLineColor("Below is a list of all commands:", ConsoleColor.Yellow);
            foreach (Command command in Util.Commands)
            {
                Util.WriteColor("    " + command.Name, ConsoleColor.Yellow);
                for (int i = 0; i < tabination - command.Name.Length; i++)
                    Console.Write(' ');
                Util.WriteLineColor(" -> " + command.Description, ConsoleColor.Yellow);
            }
        }

        public override string Name => "help";
        public override string Description => "Displays this message";
    }

    public class New : Command
    {
        public override void Execute()
        {
            // Get key pair
            Tuple<string, string> keys = Encryptor.Encrypt();

            // Print key pair
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Public key:  {0}", keys.Item1);
            Console.WriteLine("Private Key: {0}", keys.Item2);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public override string Name => "new";
        public override string Description => "Creates and displays information about a wallet";
    }

    public class Send : Command
    {
        public override void Execute()
        {
            Console.Write("Enter your public key: ");
            string publicKey = Util.ReadLineColor(ConsoleColor.DarkGray);
            Console.Write("Enter your private key: ");
            string privateKey = Util.ReadLineColor(ConsoleColor.DarkGray);

            if (Encryptor.CheckKeyPair(publicKey, privateKey))
            {
                // Get the destination address
                Console.Write("Enter the destination address: ");
                string destination = Util.ReadLineColor(ConsoleColor.DarkGray).Trim() + "=" + publicKey;

                // Get the transaction amount
                double amount = 0;
                Console.Write("Enter the amount to send: ");
                try
                {
                    amount = Convert.ToDouble(Util.ReadLineColor(ConsoleColor.DarkGray).Trim());
                }
                catch
                {
                    Util.PrintError("Amount invalid.");
                    return;
                }

                // Confirm transaction
                Console.Write("Are you sure you want to send {0} Script-Coins to the destination specified? [y/n] ", amount);
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Y:
                        long blockTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                        string hashText = blockTime + "=" + destination + "=" + amount;
                        using (StreamWriter writer = File.AppendText(Directory.GetCurrentDirectory() + "\\Documents\\localNode.txt"))
                            writer.WriteLine(hashText);
                        Console.WriteLine("\nTransaction complete!");
                        break;

                    default:
                        Console.WriteLine("\nTransaction cancelled.");
                        break;
                }
            }
            else
                Util.PrintError("Invalid key pair.");

            // Reset variables
            publicKey = null;
            privateKey = null;
        }

        public override string Name => "send";
        public override string Description => "Send ScriptCoin to a destination";
    }

    public class Clear : Command
    {
        public override void Execute() => Console.Clear();
        public override string Name => "clear";
        public override string Description => "Clears the console window";
    }

    public class Exit : Command
    {
        public override void Execute() => Program.Quit = true;
        public override string Name => "exit";
        public override string Description => "Quits the program";
    }

    public class Wallet : Command
    {
        public override string Name => "wallet";
        public override string Description => "Displays information about the current wallet";
    }

    public class Mine : Command
    {
        public override void Execute()
        {
            while (!Console.KeyAvailable)
            {
                Mining.MineStart();
            }
        }

        public override string Name => "mine";
        public override string Description => "Sets the program mining ScriptCoin";
    }

    public class Code : Command
    {
        public override string Name => "code";
        public override string Description => "Opens the programming window";
    }

    public class Joke : Command
    {
        public override void Execute() =>
            Util.WriteLineColor(Util.Jokes[new Random().Next(Util.Jokes.Count)], ConsoleColor.DarkCyan);

        public override string Name => "joke";
        public override string Description => "Prints a random joke";
    }
}
