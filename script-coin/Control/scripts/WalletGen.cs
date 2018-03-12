using System;
using System.Security.Cryptography;
using System.Text;

namespace ScriptCoin
{
    public class WalletGen
    {
        public static Tuple<string, string> WalletHash()
        {
            Random random = new Random();

            string seed1 = string.Empty;
            string seed2 = string.Empty;
            string privKey = string.Empty;
            string pubKey = string.Empty;
            string checkSum = string.Empty;

            byte[] privData = new byte[512];
            byte[] pubData = new byte[512];
            byte[] tempData = new byte[512];

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
            privKey = Convert.ToBase64String(privData);
            int startIndex = 0;
            int length = (privKey.Length);
            checkSum = privKey.Substring(startIndex, length);

            char[] charArray = checkSum.ToCharArray();
            byte[] byteArray = new byte[checkSum.Length];

            for(int i = 0; i < charArray.Length; i++)
                byteArray[i] = Convert.ToByte(charArray[i]);

            checkSum = Convert.ToBase64String(sHA256.ComputeHash(sHA256.ComputeHash(byteArray)));
            checkSum = checkSum.Substring(0, (checkSum.Length / 4));

            //Generates public address
            for (int i = 0; i < 3; i++)
            {
                pubData = sha512.ComputeHash(privData);
                pubData = sHA256.ComputeHash(pubData);
                tempData = pubData;
            }

            //Converts bytes to strings and normalizes data
            pubKey = Convert.ToBase64String(pubData);
            pubKey = Convert.ToBase64String(pubData);
            pubKey = "0x00=" + pubKey + checkSum;
            privKey = Convert.ToBase64String(privData);
            tempData = new byte[512];

            //Returns info
            return new Tuple<string, string>(pubKey, privKey);
        }
    }
}