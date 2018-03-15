using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace scriptcoin
{
    public class Miner
    {
        //MULTITHREADING CONTROL 
        public static string Hasher()
        {
            string Hash = Hashing();

            //Thread thread1 = 

            return Hash;
        }
        
        //ACTUAL MINER
        private static string Hashing()
        {
            string difficulty = "0";

            Random rnd = new Random();

            start:
            string seed = rnd.Next(10).ToString();
            string hash = string.Empty;

            byte[] blockTest = new byte[512];
            byte[] tempData = Encoding.ASCII.GetBytes(seed);

            SHA512 sha512 = new SHA512Managed();
            blockTest = sha512.ComputeHash(tempData);
            string Hash = Convert.ToBase64String(blockTest);
            string testHash = Hash.Substring(0, difficulty.Length);

            if (testHash == difficulty)
            {
                string blockHash = Hash.Substring(0, (Hash.Length - 10));
                return blockHash;
            }
            else goto start;
        }
    }
}

