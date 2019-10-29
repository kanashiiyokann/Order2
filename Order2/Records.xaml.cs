using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Order2
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Records : Window
    {
        public Records(List<String> records)
        {
            InitializeComponent();
            afterInit(records);
        }
        private void afterInit(List<String> records)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("序号").MaxLength=50;
            dt.Columns.Add("姓名");
            dt.Columns.Add("用餐");
            dt.Columns.Add("时间");


            int index = 0;
            foreach(String str in records)
            {
                List<Dictionary<String, Object>> meals = JsonConvert.DeserializeObject<List<Dictionary<String, Object>>>(str);
                DataRow dr = dt.NewRow();
                if (meals.Count > 0)
                {
                    Dictionary<String, Object> last = meals[0];
                    

                    dr["序号"] = ++index;
                    dr["姓名"] = last["peopleName"].ToString();
                    dr["用餐"] = String.Format("{0} {1}", last["mealName"], last["mealTypeName"]);
                    dr["时间"] = last["haveMealTime"].ToString().Split(' ')[0];
                }
                dt.Rows.Add(dr);


            }
            this.dataGrid.ItemsSource = dt.DefaultView;



        }
    }
}
