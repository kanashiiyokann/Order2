using Artifact.Data;
using Artifact.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artifact.Encrypt;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Threading;
using Order2.Entity;

namespace Order2.Service
{
    public class OrderService
    {
        private List<ManualResetEvent> manualResetEventList;

        private string token;




        public List<Meta> GetMealList(string areaCode)
        {

            string url_get_meal = "http://cloudfront.dgg.net/cloud-front/meal/admin/getUsableMealList";
            HttpClient http = getDefaultHttpCilent(url_get_meal);

            http.AddHeader("Referer", "http://xdy.dgg.net/dict/");
            http.AddHeader("Accept", "application/json, text/plain, */*");
            http.AddHeader("Accept-Encoding", "gzip, deflate");
            http.AddHeader("Connection", "keep-alive");
            http.AddHeader("timestamp", getTimeStamp());
            http.AddHeader("Accept-Language", "zh-CN,zh;q=0.9");
            http.AddHeader("Origin", "http://xdy.dgg.net");

            http.SetParameter(String.Format("{{\"haveMealCode\":\"{0}\"}}", areaCode));

            string res = http.GetResponseString();

            Dictionary<string, Object> dic = JsonConvert.DeserializeObject<Dictionary<string, Object>>(res);
            if (dic["code"].ToString().Equals("0"))
            {
                List<Dictionary<string, object>> meals = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(dic["data"].ToString());
                return Meta.Parse(meals);
            }

            return null;
        }

        public string Login(string name, string pwd)
        {
            name = Base64Cryptor.Encrypt(name);
            pwd = Md5Cryptor.Encrypt(pwd);

            string url_login = "http://211.149.178.98:8088/sysuser/login";
            HttpWebRequest request = WebRequest.Create(url_login) as HttpWebRequest;

            request.Method = "POST";
            request.ContentType = "application/json";
            string content = String.Format("{{\"username\":\"{0}\",\"loginPwd\":\"{1}\"}}", name, pwd);
            byte[] data = Encoding.UTF8.GetBytes(content);
            request.ContentLength = data.Length;
            request.Headers.Add("token", token);
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36";
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string retString = null;
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                retString = reader.ReadToEnd();
            }
            Stream myResponseStream = response.GetResponseStream();


            return retString;
        }


        public void GetOrderRecord(List<Meta> empList)
        {
            // getToken();

            string url_order_record = "http://cloudfront.dgg.net/cloud-front/dinner/admin/getDinnerRecordList";
            HttpClient http = getDefaultHttpCilent(url_order_record);
            http.AddHeader("Referer", "http://xdy.dgg.net/dict/");
            http.AddHeader("Accept", "application/json, text/plain, */*");
            http.AddHeader("Accept-Encoding", "gzip, deflate");
            http.AddHeader("Connection", "keep-alive");
            http.AddHeader("timestamp", getTimeStamp());
            http.AddHeader("Accept-Language", "zh-CN,zh;q=0.9");
            http.AddHeader("Origin", "http://xdy.dgg.net");

            string content = String.Format("{{\"deptId\":\"\",\"haveMealTimeEnd\":null,\"haveMealTimeStart\":null,\"mealType\":\"\",\"page\":1,\"pageSize\":3,\"peopleNo\":\"{0}\",\"searchType\":1,\"sourceType\":\"\"}}'", empList[0].Children[0]["no"]);
            http.SetParameter(content);
            string str = http.GetResponseString();

            object data = JsonConvert.DeserializeObject(str);

        }

        public void getToken()
        {
            this.token = null;
            string url_token = "http://211.149.178.98:8088/session/gettoken";
            HttpClient http = getDefaultHttpCilent(url_token);

            string res = http.GetResponseString();

            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<String, Object>>(res);
            if (dict.ContainsKey("data"))
            {
                res = dict["data"].ToString();
                dict = JsonConvert.DeserializeObject<Dictionary<String, Object>>(res);
                this.token = dict["token"].ToString();
            }

        }


        public List<String> OrderMeal(string mealId, List<Element> empList)
        {
            List<String> failedList = new List<string>();
            string url_order_meal = "http://cloudfront.dgg.net/cloud-front/dinner/admin/addDinnerMeal";

            manualResetEventList = new List<ManualResetEvent>(empList.Count);

            foreach (Element emp in empList)
            {
                ManualResetEvent manualResetEvent = new ManualResetEvent(false);
                manualResetEventList.Add(manualResetEvent);

                ThreadPool.QueueUserWorkItem((args) =>
                {
                    Object[] argArray = args as Object[];

                    Meta emplyoee = argArray[0] as Meta;
                    ManualResetEvent resetEvent = argArray[1] as ManualResetEvent;
                    HttpClient http = getDefaultHttpCilent(url_order_meal);
                    http.AddHeader("Referer", "http://xdy.dgg.net/dict/");
                    http.AddHeader("Accept", "application/json, text/plain, */*");
                    http.AddHeader("Accept-Encoding", "gzip, deflate");
                    http.AddHeader("Connection", "keep-alive");
                    http.AddHeader("timestamp", getTimeStamp());
                    http.AddHeader("Accept-Language", "zh-CN,zh;q=0.9");
                    http.AddHeader("Origin", "http://xdy.dgg.net");

                    string content = String.Format("{{\"areaId\":\"CDTY27L\",\"mealId\":\"{0}\",\"peopleNo\":\"{1}\"}}", mealId, emp.No);
                    http.SetParameter(content);

                    string res = http.GetResponseString();

                    Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);

                    if (!dict["code"].ToString().Equals("0"))
                    {
                        failedList.Add(emp.Name);
                    }

                    resetEvent.Set();
                }, new Object[] { emp, manualResetEvent });


            }


            WaitHandle.WaitAll(manualResetEventList.ToArray());

            return failedList;

        }
        private string getTimeStamp()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            DateTime nowTime = DateTime.Now;
            long unixTime = (long)System.Math.Round((nowTime - startTime).TotalMilliseconds, MidpointRounding.AwayFromZero);
            return unixTime.ToString();
        }




        private HttpClient getDefaultHttpCilent(string url)
        {
            HttpClient http = new HttpClient();


            http.PreparePost(url);
            http.AddHeader("Content-Type", "application/json;charset=UTF-8");
            if (this.token != null) http.AddHeader("token", token);
            http.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36");


            return http;
        }

    }


}
