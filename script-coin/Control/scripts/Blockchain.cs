using System;
using System.IO;

namespace scriptcoin
{
    public class Blockchain
    {
        public static string difficulty = "0";

        public static void Connect()
        {
            
        }

        public static void Difficulty()
        {
            string diffVal = difficulty;
            string prevBlock = "9737";
            string currentBlock = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            string currentTrunk = currentBlock.Substring(currentBlock.Length - 4);
            int blockDelay = Math.Abs(Int32.Parse(currentTrunk) - Int32.Parse(prevBlock));
            prevBlock = currentTrunk;

            if (blockDelay < 8900)
            {
                diffVal = diffVal + "0";
            }
            if (blockDelay > 9100)
            {
                if (difficulty.Length < 1)
                {
                    difficulty.Substring(0, (difficulty.Length - 1));
                }
            }

            difficulty = diffVal;
            Miner.Difficulty = diffVal;
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

