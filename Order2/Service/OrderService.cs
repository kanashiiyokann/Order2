using Artifact.Data;
using Artifact.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artifact.Encrypt;

namespace Order2.Service
{
  public  class OrderService
    {
        private string url_meal_list = "http://cloudfront.dgg.net/cloud-front/dinner/getUsableMealList";
        private string url_order_meal = "";
        private string url_cancel_meal = "";
        private string url_meal_record = "";


        public List<Meta> GetMealList(string areaCode,Meta emp)
        {
            HttpClient http = new HttpClient();
            string url = String.Format("{0}?onLineId={1}", this.url_meal_list, emp["onLineId"]);

            http.PreparePost(url);
            http.SetUserAgent("okhttp/3.8.0").AddCookie("FblockSession", emp["sesstionId"]);
            http.SetParameter(String.Format("{{\"haveMealCode\":\"{0}\"}}", areaCode));
            http.AddHeader("token", "56708986-9796-4fcc-90a7-9e49e56fbe6b")
                  .AddHeader("authorize", "cloudmanage-android")
                  .AddHeader("channel", "app")
                  .AddHeader("iboss-defkey", "")
                  .AddHeader("sysCode", "18052801")
                  .AddHeader("Accept-Encoding", "gzip")
                  .AddHeader("versionCode", "190214101")
                  .AddHeader("versionName", "2.4.7")
                  .AddHeader("machineNo", "c67b309761aca6aa540587d30ec7ae2");
            string res = http.GetResponseString();

            return null;
        }

        public void Login(string name,string pwd)
        {
            name = Base64Cryptor.Encrypt(name);
            pwd = Md5Cryptor.Encrypt(pwd);



        }
    }
}
