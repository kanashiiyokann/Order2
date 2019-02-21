using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Artifact.Net
{
    public class HttpClient
    {
        private HttpWebRequest request;
        private string url;
        private string type = "POST";
        private Encoding encoding = Encoding.UTF8;
        private Dictionary<string, string> cookies = new Dictionary<string, string>();
        private Dictionary<string, Object> paras = new Dictionary<string, object>();
        private Dictionary<string, string> headers = new Dictionary<string, string>();
        private string content;
        private string contentType = "application/json; charset=utf-8";
        private Boolean keepAlive = true;
        private string userAgent;
        private string acceptEncoding;


        public HttpClient PreparePost(string url)
        {
            this.url = url;
            return this;
        }
        public HttpClient PreparePost(string url, Dictionary<String, Object> paras)
        {
            this.url = url;
            this.paras = paras;
            return this;
        }

        public HttpClient PrepareGet(String url)
        {
            this.url = url;
            this.type = "GET";
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
            this.headers.Add(name, value);
            return this;
        }
        public HttpClient AddParameter(string key, string value)
        {
            this.headers.Add(key, value);
            return this;
        }
        public HttpClient AddCookie(string key, string value)
        {
            this.cookies.Add(key, value);
            return this;
        }
        public HttpClient SetParameter(String content)
        {
            this.content = content;
            this.paras.Clear();
            return this;
        }
        public HttpClient SetKeepAlive(bool keepAlive)
        {
            this.keepAlive = keepAlive;
            return this;

        }
        public HttpClient SetUserAgent(string agent)
        {
            this.userAgent = agent;
            return this;

        }
        public HttpClient SetAcceptEncoding(string type)
        {
            this.acceptEncoding = type;
            return this;

        }
        #endregion




        private void GenerateRequest()
        {
            //基本要素
            request = (HttpWebRequest)WebRequest.Create(this.url);
            request.Method = type.ToUpper();
            request.ContentType = this.contentType;
            request.KeepAlive = this.keepAlive;
            if (this.userAgent != null) request.UserAgent = this.userAgent;
           
            // header
            if (this.headers.Count > 0)
            {
                foreach (KeyValuePair<string, string> ky in this.headers)
                {
                    request.Headers.Add(ky.Key, ky.Value);
                }
            }


            //cookie加入
            if (cookies.Count != 0)
            {
                Uri uri = new Uri(this.url);
                Uri host = new Uri(String.Format("{0}://{1}",uri.Scheme,uri.Host));
                CookieContainer container = new CookieContainer();
                foreach (KeyValuePair<String, String> ky in cookies)
                {
                    container.Add(host, new Cookie(ky.Key, ky.Value));
                }
                request.CookieContainer = container;
            }
            //参数加入
            if (paras.Count != 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<String, Object> ky in paras)
                {
                    sb.Append("&");
                    sb.AppendFormat("{0}={1}", ky.Key, ky.Value.ToString());

                }
                if (sb.Length > 0) this.content = sb.Remove(0, 1).ToString();


            }
   
            if (this.content != null && this.content.Length != 0)
            {
                byte[] data = this.encoding.GetBytes(this.content);
                request.ContentLength = data.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
            }





        }
        /// <summary>
        /// 获取字符串形式的响应结果
        /// </summary>
        /// <returns></returns>
        public String GetResponseString()
        {
            GenerateRequest();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, this.encoding);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;

        }


    }
}
