using System;
using System.IO;

namespace scriptcoin
{
    public class Blockchain
    {
        public static void Connect()
        {
            string[] knownNodes = File.ReadAllLines(Environment.CurrentDirectory.ToString() + "\\documents\\knownNodes.txt");
            string request = "ping";
            string reply = "pong";
            int nodesConnected = 0;

            if (knownNodes.Length >= 5)
            {
                for (nodesConnected = 0; nodesConnected <= 5;)
                {
                    int i = 0;
                    string testIP = knownNodes[i];

                    if (true)
                    {
                        nodesConnected++;
                    }
                }
            }
        }

        public static string Difficulty()
        {
            string difficulty = "0;";
            /*
            long prevBlock = 152164899737;
            long currentBlock = DateTimeOffset.Now.ToUnixTimeSeconds();
            long blockDelay = Math.Abs(currentBlock - prevBlock);

            if (blockDelay > 220)
            {
                difficulty = difficulty + "0";
            }
            /*
            if (blockDelay < 1800)
            {
                if (difficulty.Length >= 1)
                {
                    difficulty = difficulty.Substring(0, (difficulty.Length - 1));
                }
            }
            */
            return difficulty;
        }
        
        public static string Reward()
        {
            double reward = 50;
            double blockNumber = 0;
            
            reward = ((blockNumber % (211680)) == 1 && blockNumber == 1 ? 50 : reward * 0.75);

            return reward.ToString();
        }

        public static void Send()
        {

        }

        public static void Recieve()
        {

        }
    }
}

