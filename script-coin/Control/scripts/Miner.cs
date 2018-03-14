using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace scriptcoin
{
    public class Miner
    {
        //MULTITHREADING CONTROL 
        public static void Hasher()
        {
            int coreCount = (Environment.ProcessorCount - 1);
                                    
            for (int i = 1; i <= coreCount; i++)
            {
                int iCopy = i;
                Thread thread = new Thread(Hashing);
                thread.Name = "Thread - " + iCopy.ToString();
                thread.Start();
            }
            
            Miner.Hashing();
        }
        
        //ACTUAL MINER
        private static void Hashing()
        {
            Console.WriteLine("Please input your public key");
            string userAddress = Console.ReadLine();
            
            string difficulty = "00000";

            Random rnd = new Random();

            while (true)
            {
                start:
                string seed = rnd.Next(10).ToString();
                string hash = string.Empty;

                byte[] blockTest = new byte[512];
                byte[] tempData = Encoding.ASCII.GetBytes(seed);

                SHA256 sha256 = new SHA256Managed();
                blockTest = sha256.ComputeHash(tempData);
                string Hash = Convert.ToBase64String(blockTest);
                string testHash = Hash.Substring(0, difficulty.Length);

                Console.WriteLine(Hash + userAddress + "=10");
                /*
                if (testHash == difficulty)
                {
                    Console.WriteLine(Hash + "=" + userAddress + "=10");
                }
                */
                goto start;                 
            }            
        }
    }
}

