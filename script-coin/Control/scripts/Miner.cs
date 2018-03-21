using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace scriptcoin
{
    class Mining
    {
        public static List<Miner> Miners { get; set; } = new List<Miner>();

        public static int MineCount = 0;

        public static void MineStart()
        {
            // Initialize miner threads
            Miner.InitializeAll();
            Miner.StartAll();

            Console.ReadKey();

            Miner.StopAll();
            Console.WriteLine("Mining has stopped. {0} mining threads were used.", Miners.Count());
        }
    }

    class Miner
    {
        public Thread Thread { get; set; }
        public bool IsActive { get; set; }

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

        public void Mine()
        {

            Random rnd = new Random();
            byte[] hashValue;
            string Difficulty = "00000";

            while (!_quitThread)
            {
                string Reward = Blockchain.Reward();

                string seed = rnd.Next().ToString();

                SHA256 sha256 = SHA256.Create();
                hashValue = sha256.ComputeHash(Encoding.ASCII.GetBytes(seed));
                string hash = Convert.ToBase64String(hashValue);

                if (hash.Substring(0, (Difficulty.Length)) == Difficulty)
                {
                    Console.WriteLine(hash + (DateTimeOffset.Now.ToUnixTimeSeconds().ToString()) + Reward);
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
