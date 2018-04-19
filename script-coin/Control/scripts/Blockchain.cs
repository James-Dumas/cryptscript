using System;
using System.IO;

namespace scriptcoin
{
    public class Blockchain
    {
        public static void Connect()
        {
            
        }

        public static void Difficulty()
        {
            string difficulty = "00";
            string prevBlock = 152164899737.ToString();
            string currentBlock = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            long blockDelay = Math.Abs(Int64.Parse(prevBlock) - Int64.Parse(currentBlock));
            prevBlock = currentBlock;

            if (blockDelay < 10)
            {
                difficulty = difficulty + "0";
            }
            if (blockDelay > 20)
            {
                if (difficulty.Length < 1)
                {
                    difficulty.Substring(0, (difficulty.Length - 1));
                }
            }
            Miner.Difficulty = difficulty;
        }
        
        public static void Reward()
        {
            double reward = 50;
            double blockNumber = 0;
            
            reward = ((blockNumber % (211680)) == 1 && blockNumber == 1 ? 50 : reward * 0.75);

            Miner.Reward =  reward.ToString();
        }

        public static void Send()
        {

        }

        public static void Recieve()
        {

        }
    }
}

