using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace scriptcoin
{
    class Mining
    {
        public static List<Miner> Miners { get; set; } = new List<Miner>();

        public static int MineCount = 0;

        public static void MineStart()
        {
            Console.WriteLine("Please input your private address;");
            Miner.PubAdd = Console.ReadLine().Trim();
            if (String.IsNullOrEmpty(Miner.PubAdd))
            {
                Miner.PubAdd = "0x00=jU0UrZBkqPXfp8MsMoILSRylevQGaUmJRnpFbfUvcGs=7lvpCgtyWl0";
            }
            
            // Initialize miner threads
            Miner.InitializeAll();
            Miner.StartAll();

            Console.ReadKey(true);

            Miner.StopAll();
            Console.WriteLine("Mining has stopped. {0} mining threads were used.", Miners.Count());
        }
    }
    
    class Miner
    {

        public Thread Thread { get; set; }
        public bool IsActive { get; set; }
        public static string PubAdd { get; set; }
        public static string Reward { get; set; }
        public static string Difficulty { get; set; }

        private bool _quitThread = false;

        public Miner()
        {
            bool MinerExists = false;
            if (Mining.Miners.Count() == 0)
            {
                Mining.Miners.Add(this);
            }
            else
            {
                for (int i = 0; i < Mining.Miners.Count(); i++)
                {
                    MinerExists = MinerExists || Mining.Miners[i] == this;

                    if (MinerExists)
                    {
                        Mining.Miners[i].IsActive = false;
                        return;
                    }
                    else
                    {
                        Mining.Miners.Add(this);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a certain number of miners
        /// </summary>
        /// <param name="number">Number of miners to initialize</param>
        public static void BatchInitialize(int number)
        {
            // Cap active miners at the number of logical cores minus 1
            number = Math.Min(number, Environment.ProcessorCount - 1 - Mining.Miners.Count());

            // Initialize miners
            for (int i = 0; i < number; i++)
            {
                Miner miner = new Miner();
            }
        }

        public void Abort() => _quitThread = true;

        public static int blockNumber = 0;

        public void Mine()
        {
            Blockchain.Difficulty();

            string path = Environment.CurrentDirectory.ToString() + "\\documents\\localNode.txt";

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Random rnd = new Random();
            byte[] hashValue;
            string pubAdd = PubAdd;

            while (!_quitThread)
            {
                string diffVal = Difficulty;
                string rewVal = Reward;

                string seed = rnd.Next().ToString();

                SHA256 sha256 = SHA256.Create();
                hashValue = sha256.ComputeHash(Encoding.ASCII.GetBytes(seed));
                string hash = Convert.ToBase64String(hashValue);
                string blockHash = hash + (DateTimeOffset.Now.ToUnixTimeSeconds().ToString()) + "=" + pubAdd + "=" + rewVal;

                if (hash.Substring(0, (diffVal.Length)) == diffVal)
                {
                    Console.WriteLine(blockHash);
                    
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(blockHash);
                    }

                    blockNumber = blockNumber + 1;

                    Blockchain.Difficulty();
                    Blockchain.Reward(blockNumber);
                }
            }
        }

        public static void StartAll() => BatchStart(Mining.Miners.Count());
        public static void InitializeAll() => BatchInitialize(Environment.ProcessorCount - 1);
        public static void StopAll() => BatchStop(Mining.Miners.Count());

        public static void BatchStart(int number)
        {
            number = Math.Min(number, Mining.Miners.Count());

            int i = 0;
            foreach (var miner in Mining.Miners)
            {
                if (!(i < number)) break;

                if (!miner.IsActive)
                {
                    miner.Thread = new Thread(new ThreadStart(miner.Mine));
                    miner.Thread.Start();
                    miner.IsActive = true;
                }
            }
        }

        public static void BatchStop(int number)
        {
            Console.ForegroundColor = ConsoleColor.White;
            number = Math.Min(number, Mining.Miners.Count());

            int i = 0;
            foreach (var miner in Mining.Miners)
            {
                if (!(i < number)) break;

                if (miner.Thread.IsAlive)
                {
                    miner.Abort();
                }
            }
        }
    }
}
