using System;
using System.Security.Cryptography;
using System.Text;

namespace scriptcoin
{
    public class Encryptor
    {
        public static Tuple<string, string> Encrypt()
        {
            Tuple<string, string> keys = WalletGen.WalletHash();
            string pubEncrypt = keys.Item1;
            string privEncrypt = keys.Item2;

            return new Tuple<string, string>(pubEncrypt, privEncrypt);
        }

        public static string Decrypt()
        {
            SHA256 sHA256 = new SHA256Managed();
            string checkVar = string.Empty;
            string checkSum = string.Empty;
            byte[] checkPriv = new byte[16];

            Console.WriteLine("Please input your public address and private address;");
            string pubCheck = Console.ReadLine();
            string privCheck = Console.ReadLine();

            string[] pubVals = pubCheck.Split("="); //pubVals[2] == checksum
            checkPriv = Encoding.ASCII.GetBytes(privCheck);
            checkSum = Convert.ToBase64String(sHA256.ComputeHash(sHA256.ComputeHash(checkPriv)));

            if (pubVals[2] == (checkSum.Substring(0, pubVals[2].Length)))
            {
                checkVar = "valid";
            }
            else checkVar = "bad";

            return checkVar;
        }
    }
}