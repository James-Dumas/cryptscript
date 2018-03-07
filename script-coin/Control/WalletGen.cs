using System;
using System.Security.Cryptography;
using System.Text;

namespace ScriptCoin
{
    public class WalletGen
    {
        public static Tuple<string, string> WalletHash()
        {
            //Initializes all vars, strings, and byte[]s
            var random = new Random();

            string seed1 = string.Empty;
            string seed2 = string.Empty;
            string privKey = string.Empty;
            string pubKey = string.Empty;
            string checkSum = string.Empty;

            byte[] privData = new byte[512];
            byte[] pubData = new byte[512];
            byte[] tempData = new byte[512];
            byte[] checkSeed = new byte[512];

            SHA256 sHA256 = new SHA256Managed();
            SHA512 sha512 = new SHA512Managed();

            //Generates seed1 and seed 1, then combines them into 64 bit string
            for (int i = 0; i < 32; i++)
            {
                seed1 = String.Concat(seed1, random.Next(10).ToString());
            }
            for (int i = 0; i < 32; i++)
            {
                seed2 = String.Concat(seed2, random.Next(10).ToString());
            }
            byte[] seedFinal = Encoding.ASCII.GetBytes(seed1 + seed2);

            //Generates private key
            for (int i = 0; i < 3; i++)
            {
                privData = sha512.ComputeHash(seedFinal);
                privData = sHA256.ComputeHash(privData);
                tempData = privData;
            }

            //Generates the checksum
            checkSeed = sha512.ComputeHash(tempData);
            checkSeed = sha512.ComputeHash(checkSeed);
            checkSeed = sHA256.ComputeHash(checkSeed);

            //Converts bytes to strings
            privKey = Convert.ToBase64String(privData);
            checkSum = Convert.ToBase64String(checkSeed);

            //Generates public address
            pubData = Encoding.ASCII.GetBytes("0x00" + privKey + checkSum);
            pubData = sHA256.ComputeHash(pubData);
            pubKey = Convert.ToBase64String(pubData);
            pubKey = ("0x00" + privKey + "a1x0");

            //Returns info
            return new Tuple<string, string>(pubKey, privKey);
        }
    }
}