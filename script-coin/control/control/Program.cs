using System;

namespace Script_Coin
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
            Initialize:

                Console.WriteLine("Type in a command or use `-h` for help");
                string userInput = Console.ReadLine();
                int testLength = userInput.Length;

                for (int i = 0; i <= testLength; i++)
                {
                    if (userInput.Contains("-n"))
                    {
                        Console.WriteLine("Here is a new wallet" + System.Environment.NewLine);
                        goto Initialize;
                    }
                    else if (userInput.Contains("-s"))
                    {
                        Console.WriteLine("Please type in the public wallet ID of the destination");
                        string destination = Console.ReadLine();
                        Console.WriteLine("How much do you want to send (in Script-Coin)?");
                        string ammount = Console.ReadLine();
                        Console.WriteLine("You are sending " + ammount + " Script-Coins to " + destination + ". Is this correct? [-y or -n]");
                        string verification = Console.ReadLine();
                        int varLength = verification.Length;
                        if (verification.Contains("-y"))
                        {
                            Console.WriteLine("You have sent " + ammount + " Script-Coins to " + destination);
                        }
                        else if (verification.Contains("-n"))
                        {
                            Console.WriteLine("Transaction cancelled");
                        }
                        else
                        {
                            Console.WriteLine("Input unknowm, tranasction cancelled");
                        }

                        Console.WriteLine(System.Environment.NewLine);
                        goto Initialize;
                    }
                    else if (userInput.Contains("-w"))
                    {
                        Console.WriteLine("Here is your wallet information" + System.Environment.NewLine);
                        goto Initialize;
                    }
                    else if (userInput.Contains("-c"))
                    {
                        Console.WriteLine("The code will launch" + System.Environment.NewLine);
                        goto Initialize;
                    }
                    else if (userInput.Contains("-h"))
                    {
                        Console.WriteLine("-n -> Generate a new public wallet");
                        Console.WriteLine("-s -> Send Script-Coin to a location");
                        Console.WriteLine("-w -> Gives current wallet information");
                        Console.WriteLine("-c -> Launches code window");
                        goto Initialize;
                    }
                    else
                    {
                        Console.WriteLine("Invalid command");
                        goto Initialize;
                    }
                }
            }
        }
    }
}
