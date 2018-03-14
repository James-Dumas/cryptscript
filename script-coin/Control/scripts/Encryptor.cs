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

        public static bool CheckKeyPair(string publicKey, string privateKey)
        {
            SHA256 sha = new SHA256Managed();

            string publicChecksum = publicKey.Split('=')[publicKey.Split('=').Length - 1];
            byte[] privateBytes = new byte[16];
            privateBytes = Encoding.ASCII.GetBytes(privateKey);
            string checkSum = Convert.ToBase64String(sha.ComputeHash(sha.ComputeHash(privateBytes)));

            return publicChecksum == (checkSum.Substring(0, publicChecksum.Length))
                && publicKey.Length != 0 && privateKey.Length != 0;
        }
    }
}