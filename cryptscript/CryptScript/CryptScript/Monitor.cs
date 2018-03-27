using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptScript
{
    public static class Monitor
    {
        public static int HashRate { get; set; } = 0;
        public static string Wallet { get; set; } = "";

        public static void Display()
        {
            while(true)
            {
                // Check for keyboard interrupt
                if(Console.KeyAvailable)
                {
                    if(Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }

                Print();
                HashRate = 0;
                System.Threading.Thread.Sleep(1000);
            }
        }

        public static void Print()
        {
            Console.Clear();

            // Write the exit instructions
            Console.SetCursorPosition(Console.WindowWidth - 26, Console.WindowHeight - 2);
            Util.WriteColor("Press [ESCAPE] to return", ConsoleColor.DarkGray);

            // Get the center of the screen
            Tuple<int, int> Center = new Tuple<int, int>(Console.WindowWidth / 2, Console.WindowHeight / 2);

            // Translate the hash rate to block letters
            char[] hash = (HashRate + " H/S").ToCharArray();
            string[] hashBlocks = new string[5];

            // For each character in the hash string...
            for(int i = 0; i < hash.Length; i++)
            {
                // If the character map has a key for this character...
                if(CharMaps.ContainsKey(hash[i]))
                {
                    // For each string of hashblocks...
                    for(int j = 0; j < hashBlocks.Length; j++)
                    {
                        // Add the character block string to the hash block string
                        hashBlocks[j] += CharMaps[hash[i]][j] + " ";
                    }
                }
            }

            // Print the block letters
            Tuple<int, int> BlockPosition = new Tuple<int, int>(Center.Item1 - (hashBlocks[1].Length / 2), Center.Item2 - 2);
            for(int i = 0; i < 5; i++)
            {
                Console.SetCursorPosition(BlockPosition.Item1, BlockPosition.Item2 + i);
                Util.WriteColor(hashBlocks[i], ConsoleColor.Green);
            }

            // Print the wallet address
            Console.SetCursorPosition(Center.Item1 - (Wallet.Length / 2), Center.Item2 + 4);
            Util.WriteColor(Wallet, ConsoleColor.Cyan);
        }

        private static Dictionary<char, string[]> CharMaps = new Dictionary<char, string[]>
        {
            { '0', new string[] { " ### ", "#   #", "#   #", "#   #", " ### ", } },
            { '1', new string[] { " ##  ", "# #  ", "  #  ", "  #  ", "#####", } },
            { '2', new string[] { " ### ", "#   #", "  ## ", " #   ", "#####", } },
            { '3', new string[] { " ### ", "#   #", "   # ", "#   #", " ### ", } },
            { '4', new string[] { "#   #", "#   #", "#####", "    #", "    #", } },
            { '5', new string[] { "#####", "#    ", "#### ", "    #", "#### ", } },
            { '6', new string[] { " ### ", "#    ", "#### ", "#   #", " ### ", } },
            { '7', new string[] { "#####", "#   #", "    #", "   # ", "   # ", } },
            { '8', new string[] { " ### ", "#   #", " ### ", "#   #", " ### ", } },
            { '9', new string[] { " ### ", "#   #", " ####", "    #", " ### ", } },
            { ' ', new string[] { "   ", "   ", "   ", "   ", "   ", } },
            { 'H', new string[] { "#   #", "#   #", "#####", "#   #", "#   #", } },
            { '/', new string[] { "  #", "  #", " # ", "#  ", "#  ", } },
            { 'S', new string[] { " ### ", "#    ", " ### ", "    #", " ### ", } }
        };
    }
}
