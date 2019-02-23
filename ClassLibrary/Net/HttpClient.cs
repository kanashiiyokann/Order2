using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace Artifact.Net
{
    public class HttpClient
    {
        private HttpWebRequest request;
        private Encoding encoding = Encoding.UTF8;



        public HttpClient PreparePost(string url)
        {
            request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            return this;
        }
        public HttpClient PrepareGet(string url)
        {
            request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ServicePoint.Expect100Continue = false;
            return this;
        }
        #region

        /// <summary>
        /// 设置编码字符集
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>   
        public HttpClient SetEncoding(Encoding encoding)
        {
            this.encoding = encoding;
            return this;
        }
        public HttpClient AddHeader(string name, string value)
        {
            if (this.request == null) throw new Exception("invoke PreparePost() first!");
            switch (name)
            {

                case ("Host"):
                    request.Host = value;
                    break;
                case ("Proxy-Connection"):
                    request.KeepAlive = value.Equals("keep-alive");
                    break;
                case ("Connection"):
                    request.KeepAlive = value.Equals("keep-alive");
                    break;
                case ("Referer"):
                    request.Referer = value;
                    break;
                case ("User-Agent"):
                    request.UserAgent = value;
                    break;
                case ("Accept"):
                    request.Accept = value;
                    break;
                case ("Content-Type"):
                    request.ContentType = value;
                    break;
                default:
                    request.Headers.Add(name, value);
                  
                    break;
            }

            return this;
        }

        public HttpClient AddCookie(string key, string value)
        {
            if (this.request == null) throw new Exception("invoke PreparePost() first!");

            Uri uri = request.RequestUri;
            Uri host = new Uri(String.Format("{0}://{1}", uri.Scheme, uri.Host));

            request.CookieContainer.Add(host, new Cookie(key, value));

            return this;
        }
        public HttpClient SetParameter(String content)
        {

            if (content != null && content.Length != 0)
            {
                byte[] data = this.encoding.GetBytes(content);
                request.ContentLength = data.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
            }

            return this;
        }

        public HttpClient SetParameter(Dictionary<string,object> paras)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<String, Object> ky in paras)
            {
                sb.Append("&");
                sb.AppendFormat("{0}={1}", ky.Key, ky.Value.ToString());
            }
            if (sb.Length > 0) {
                string  content = sb.Remove(0, 1).ToString();
                SetParameter(content);
            }
            return this;
        }
 
        #endregion

        /// <summary>
        /// 获取字符串形式的响应结果
        /// </summary>
        /// <returns></returns>
        public String GetResponseString()
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, this.encoding);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;

        }

        public static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }


    }
}
