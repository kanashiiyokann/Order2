using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Artifact.Reader
{
    public class IniReader
    {
        private string path;
        private int size = 1024;
        private bool allowCreateFile = true;
        private Dictionary<string, Dictionary<string, string>> cacheData = new Dictionary<string, Dictionary<string, string>>();
        /// <summary>
        /// 在写操作时，若不存在路径文件，允许以该路径创建文件；默认为true。
        /// </summary>
        public bool AllowCreateFile { set { allowCreateFile = value; } }
        /// <summary>
        /// 读取值的最大长度；默认为1024字节。
        /// </summary>
        public int Size
        {
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("读取的长度请设置为大于0的数值！");
                }
                size = value + 1;
            }
        }

        public delegate string AfterReadDelegate(string value);
        public event AfterReadDelegate AfterRead;
        public delegate string BeforeWriteDelegate(string value);
        public event BeforeWriteDelegate BeforeWrite;
        /// <summary>
        /// 根据文件路径装载一个ini文件。
        /// </summary>
        /// <param name="path">ini文件路径</param>
        public IniReader(string path)
        {
            this.path = path;
        }
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string
        section,
        string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
        string key, string def, StringBuilder retVal,
        int size, string filePath);
        /// <summary>
        /// 读取某个节点下的某一个键所对应的值。
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">键值</param>
        /// <param name="def">缺省值</param>
        /// <returns></returns>
        public string Read(string section, string key, string def)
        {
            string result = null;
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("未找到指定路径的文件！");
            }
            result = ReadFromCache(section, key);
            if (result == null)
            {

                StringBuilder retVal = new StringBuilder();
                GetPrivateProfileString(section, key, def, retVal, size, path);
                result = retVal.ToString();

                if (AfterRead != null)
                {
                    result = AfterRead(result);
                }
                WriteToCache(section, key, result);
            }
            return result;
        }
        /// <summary>
        /// 读取某个节点下的某一个键所对应的值，若不存在则返回缺省值：""。
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public string Read(string section, string key)
        {
            return Read(section, key, "");
        }
        /// <summary>
        /// 向ini文件中写入一个值
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">键值</param>
        /// <param name="value">需要写入的值</param>
        public void Write(string section, string key, string value)
        {
            if (!allowCreateFile && !File.Exists(path))
            {
                string message = string.Format("路径 {0} 下文件不存在，无法写入。请先创建或允许创建。", path);
                throw new FileNotFoundException(message);
            }
            WriteToCache(section, key, value);
            if (BeforeWrite != null)
            {
                value = BeforeWrite(value);
            }
            WritePrivateProfileString(section, key, value, path);

        }

        private string ReadFromCache(string section, string key)
        {
            string result = null;
            if (cacheData.ContainsKey(section))
            {
                Dictionary<string, string> sectionDict = cacheData[section];
                if (sectionDict.ContainsKey(key))
                {
                    result = sectionDict[key];
                }
            }
            return result;
        }
        private void WriteToCache(string section, string key, string value)
        {
            if (!cacheData.ContainsKey(section))
            {
                cacheData.Add(section, new Dictionary<string, string>());
            }
            Dictionary<string, string> sectionDict = cacheData[section];
            if (sectionDict.ContainsKey(key))
            {
                sectionDict[key] = value;
            }
            else
            {
                sectionDict.Add(key, value);
            }

        }
        public bool test()
        {
            return true;
        }
    }
}
