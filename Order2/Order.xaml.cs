using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Artifact.Data;
using Artifact.Net;
using Artifact.Reader;
using Order2.Service;

namespace Order2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Order : Window
    {
        private List<Meta> empList;
        private OrderService orderService = new OrderService();

        public Order()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            orderService.Login("24379471","dgg317412");

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //加载用户
            XmlReader reader = new XmlReader();
            reader.Load(System.AppDomain.CurrentDomain.BaseDirectory + "//Resource//employees.xml");
            List<Meta> dataList = reader.Select("/employees")[0].Children;
                this.empList = dataList;
            this.treeView.ItemsSource = dataList;
            //加载菜单
          //  orderService.GetMealList("CDTY27L",this.empList[0]);
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            Meta element = ckb.DataContext as Meta;
             
            if (element !=null)
            {
                bool? state = ckb.IsChecked;
                if (element.HasParent())
                {
                    Meta parent = element.Parent;
                    List<Meta> brothers = parent.Children;
                    int count = 0;
                    foreach(Meta bother in brothers)
                    {
                        if (bother["checked"].ToLower().Equals("true"))
                        {
                            count++;
                        }

                    }
                    if (count == brothers.Count)
                    {
                        parent["checked"] = "True";
                    }else if (count == 0)
                    {
                        parent["checked"] = "False";
                    }
                    else
                    {
                        parent["checked"] = "Vague";
                    }

                }
                if (element.HasChild())
                {
                    List<Meta> children = element.Children;
                    int count = 0;
                    foreach (Meta child in children)
                    {
                        if (child["checked"].ToLower().Equals("true"))
                        {
                            count++;
                        }

                    }
                    if (count == children.Count)
                    {
                        element["checked"] = "True";
                    }
                    else if (count == 0)
                    {
                        element["checked"] = "False";
                    }
                    else
                    {
                        element["checked"] = "Vague";
                    }



                }
                
              

           

            }
  

        }


        private List<Meta> getCheckedEmpList()
        {
            List<Meta> retList = new List<Meta>();
            foreach(Meta group in this.empList)
            {
                foreach(Meta emp in group.Children)
                {
                    if (emp["checked"].ToLower().Equals("true"))
                    {
                        retList.Add(emp);
                    }

                }

            }
            return retList;

        }
    }
}
