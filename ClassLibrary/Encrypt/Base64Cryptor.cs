using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artifact.Encrypt
{
    public class Base64Cryptor : Crypt
    {
        public static string Encrypt(string input)
        {
            byte[] inputByteArray = encoding.GetBytes(input);
            return Convert.ToBase64String(inputByteArray);
        }

        public static string Decrypt(string input)
        {

            byte[] dataByteArray = Convert.FromBase64String(input);
            return encoding.GetString(dataByteArray);
        }
    }
}
