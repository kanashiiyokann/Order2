using System.Security.Cryptography;
using System.Text;

namespace Artifact.Encrypt
{
    public class Md5Cryptor:Crypt
    {

        /// <summary>
        /// 获取一个字符串的32位16进制字符串格式MD5码
        /// </summary>
        /// <param name="input">原字符串</param>
        /// <returns></returns>
        public static string Encrypt(string input)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] inputByteArray =encoding.GetBytes(input);
            byte[] data = md5Hasher.ComputeHash(inputByteArray);

            //将data中的每个字符都转换为16进制的
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
