using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Artifact.Encrypt
{
    public class AesCryptor : Crypt
    {
        private static string key = "AESCRYPTORKEY";
        public static string Key { set { key = value; } }

        public static string Encrpty(string input, string key)
        {
            if (input == null || input.Equals("")) return "";
            string result = null;
            byte[] inputByteArray = encoding.GetBytes(input);

            RijndaelManaged aes = new RijndaelManaged();

            Byte[] keyByteArray = new Byte[32];
            Array.Copy(encoding.GetBytes(key.PadRight(keyByteArray.Length)), keyByteArray, keyByteArray.Length);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.Key = keyByteArray;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
                    cryptoStream.FlushFinalBlock();
                    result = Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            aes.Clear();
            return result;
        }
        public static string Encrypt(string input)
        {
            return Encrpty(input, key);
        }
        public static string Decrypt(string input, string key)
        {
            if (input == null || input.Equals("")) return "";
            string result = null;
            byte[] inputByteArray = Convert.FromBase64String(input);

            Byte[] keyByteArray = new Byte[32];
            Array.Copy(encoding.GetBytes(key.PadRight(keyByteArray.Length)), keyByteArray, keyByteArray.Length);

            RijndaelManaged aes = new RijndaelManaged();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.Key = keyByteArray;

            using (MemoryStream memoryStream = new MemoryStream(inputByteArray))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    byte[] tmp = new byte[inputByteArray.Length + 32];
                    int len = cryptoStream.Read(tmp, 0, inputByteArray.Length + 32);
                    byte[] resultBytes = new byte[len];
                    Array.Copy(tmp, 0, resultBytes, 0, len);
                    result = encoding.GetString(resultBytes);

                }
            }
            aes.Clear();

            return result;
        }
        public static string Decrypt(string input)
        {
            return Decrypt(input, key);
        }
    }
}
