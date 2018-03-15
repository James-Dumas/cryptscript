using System;
using System.Collections.Generic;
using System.Text;

namespace scriptcoin
{
    class MiningDisplay
    {
        public string Wallet { get; set; }
        public int HashRate { get; set; }

        private List<string[]> HashBlocks = new List<string[]>();

        private readonly Dictionary<char, string[]> Blocks = new Dictionary<char, string[]>
        {
            { '1', new string[]
                {
                    "  █  ",
                    " ██  ",
                    "  █  ",
                    "  █  ",
                    "  █  ",
                    "  █  ",
                    "█████"
                }
            },
            { '2', new string[]
                {
                    " ███ ",
                    "█   █",
                    "   █ ",
                    "  █  ",
                    " █   ",
                    "█    ",
                    "█████"
                }
            },
            { '3', new string[]
                {
                    " ███ ",
                    "█   █",
                    "    █",
                    "  ██ ",
                    "    █",
                    "█   █",
                    " ███ "
                }
            },
            { '4', new string[]
                {
                    "█   █",
                    "█   █",
                    "█   █",
                    "█████",
                    "    █",
                    "    █",
                    "    █"
                }
            },
            { '5', new string[]
                {
                    "█████",
                    "█    ",
                    "█    ",
                    "████ ",
                    "    █",
                    "█   █",
                    " ███ "
                }
            },
            { '6', new string[]
                {
                    " ███ ",
                    "█   █",
                    "█    ",
                    "████ ",
                    "█   █",
                    "█   █",
                    " ███ ",
                }
            },
            { '7', new string[]
                {
                    "█████",
                    "    █",
                    "   █ ",
                    "   █ ",
                    "   █ ",
                    "   █ ",
                    "   █ "
                }
            },
            { '8', new string[]
                {
                    " ███ ",
                    "█   █",
                    "█   █",
                    " ███ ",
                    "█   █",
                    "█   █",
                    " ███ "
                }
            },
            { '9', new string[]
                {
                    " ███ ",
                    "█   █",
                    "█   █",
                    " ████",
                    "    █",
                    "█   █",
                    " ███ "
                }
            },
            { '0', new string[]
                {
                    " ███ ",
                    "█   █",
                    "█   █",
                    "█   █",
                    "█   █",
                    "█   █",
                    " ███ "
                }
            },
            { 'H', new string[]
                {
                    "█   █",
                    "█   █",
                    "█   █",
                    "█████",
                    "█   █",
                    "█   █",
                    "█   █",
                }
            },
            { '/', new string[]
                {
                    "  █",
                    "  █",
                    " █ ",
                    " █ ",
                    " █ ",
                    "█  ",
                    "█  ",
                }
            },
            { 'S', new string[]
                {
                    " ███",
                    "█   █",
                    "█    ",
                    " ███ ",
                    "    █",
                    "█   █",
                    " ███ ",
                }
            },
            { ' ', new string[]
                {
                    "   ",
                    "   ",
                    "   ",
                    "   ",
                    "   ",
                    "   ",
                    "   ",
                }
            },
        };

        public void Display()
        {
            Console.Clear();

            // Convert hash rate to block letters
            Blockinate();

            // Find center of the console window
            Tuple<int, int> Center = new Tuple<int, int>(Console.WindowWidth / 2, Console.WindowHeight / 2);

            // Convert block letters to strings
            string[] hashStrings = new string[7];
            foreach(string[] block in HashBlocks)
            {
                for(int i = 0; i < block.Length; i++)
                {
                    hashStrings[i] += block[i] + ' ';
                }
            }

            // Get starting positions
            Tuple<int, int> BlockPosition = new Tuple<int, int>(
                Center.Item1 - (hashStrings[0].Length / 2),
                Center.Item2 - (hashStrings.Length / 2));

            Tuple<int, int> WalletPosition = new Tuple<int, int>(
                Center.Item1 - (Wallet.Length / 2),
                Center.Item2 + (hashStrings.Length / 2) + 2);

            // Print the blocks
            for(int i = 0; i < hashStrings.Length; i++)
            {
                Console.SetCursorPosition(BlockPosition.Item1, BlockPosition.Item2 + i);
                Util.WriteColor(hashStrings[i], ConsoleColor.DarkCyan);
            }

            // Print the wallet
            Console.SetCursorPosition(WalletPosition.Item1, WalletPosition.Item2);
            Util.WriteColor(Wallet, ConsoleColor.DarkGray);
        }

        private void Blockinate()
        {
            // Clear the hash block list
            HashBlocks = new List<string[]>();

            // Convert the hash rate into a character array
            char[] hashRate = (HashRate.ToString() + " H/S").ToCharArray();

            // Map each character to a block letter
            for(int i = 0; i < hashRate.Length; i++)
                HashBlocks.Add(Blocks[hashRate[i]]);
        }
    }
}
