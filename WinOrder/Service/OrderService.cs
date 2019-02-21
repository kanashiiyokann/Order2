using Artifact.Net;
using Artifact.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Order.Service
{
    public class OrderService
    {
        private string url_meal_list = "http://cloudfront.dgg.net/cloud-front/dinner/getUsableMealList";
        private string url_order_meal = "";
        private string url_cancel_meal = "";
        private string url_meal_record = "";


        public List<String> OrderMeal(string mealId, List<Meta> empList)
        {

            foreach (Meta emp in empList)
            {
                string url = String.Format("{0}?onLineId={1}", this.url_order_meal, emp["onLineId"]);
                HttpClient http = new HttpClient();
                http.PreparePost(url);
                http.SetParameter("");
                http.AddCookie("sesstionId", emp["sessionId"]);
                http.AddHeader("....", "....");

                string result = http.GetResponseString();

            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaId">CDTY27L</param>
        /// <param name="emp"></param>
        /// <returns></returns>
        public List<Meta> GetMealList(string areaId, Meta emp)
        {
            string url = String.Format("{0}?onLineId={1}", this.url_order_meal, emp["onLineId"]);
            string content = String.Format("{{\"haveMealCode\":\"{0}\"}}", areaId);
            HttpClient http = new HttpClient();
            http.PreparePost(url);

            http.AddCookie("FblockSession", emp["sesstionId"]).SetUserAgent("okhttp/3.8.0");

            http.AddHeader("token", "cc8c66df-d697-428b-9674-4c5d2fa0cc62")
                  .AddHeader("authorize", "cloudmanage-android")
                  .AddHeader("channel", "app")
                  .AddHeader("iboss-defkey", "")
                  .AddHeader("sysCode", "18052801")
                  .AddHeader("versionCode", "190214101")
                  .AddHeader("Accept-Encoding", "gzip")
                  .AddHeader("versionName", "2.4.7")
                  .AddHeader("machineNo", "c67b309761aca6aa540587d30ec7ae2");

            


            return null;
        }

    }
}
