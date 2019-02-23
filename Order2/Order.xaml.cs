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
using Artifact.Reader;
using Order2.Entity;
using Order2.Service;

namespace Order2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Order : Window
    {
        private List<Element> empList;
        private OrderService orderService = new OrderService();
          private  IniReader cfg;
        private String mealId;

        public Order()
        {
            InitializeComponent();
            //
            cfg = new IniReader(System.AppDomain.CurrentDomain.BaseDirectory + "//Resource//order.ini");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<Element> emps = getCheckedEmpList();
            if (emps.Count == 0)
            {
                MessageBox.Show("请选择订餐人员！");
                return;
            }

            if (this.mealId == null)
            {
                MessageBox.Show("请选择订餐！");
                return;
            }

         

         List<String> failedList=   orderService.OrderMeal(this.mealId, emps);

            if (failedList.Count == 0)
            {
                MessageBox.Show("全部订餐成功！");
            }



        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //加载用户
            XmlReader reader = new XmlReader();
            reader.Load(System.AppDomain.CurrentDomain.BaseDirectory + "//Resource//employees.xml");
            List<Element> dataList = reader.Select<Element>("//group");
               
            foreach(Element group in dataList)
            {
                List<Element> children = reader.Select<Element>("//group[@Name='" + group.Name + "']/*");
                group.Children = children;
            }

            this.treeView.ItemsSource = dataList;
            this.empList = dataList;
      
            //模拟登陆
            orderService.getToken();
      
            orderService.Login(cfg.Read("setting", "name"), cfg.Read("setting", "pwd"));
            
            //加载菜单

          List<Meta> meals=  orderService.GetMealList("CDTY27L");

            string skip = cfg.Read("setting", "skip");

            meals=   meals.FindAll((meal) => { return skip.IndexOf(meal["mealName"])<0; });
      
            this.listBox.ItemsSource = meals;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            Element element = ckb.DataContext as Element;

          
            if (element !=null)
            {
                string type =element.Type;
            
                if (type.Equals("employee"))
                {
                    Element parent = element.Parent;
                    List<Element> brothers = parent.Children;
                    int count = 0;
                    foreach (Element brother in brothers)
                    {
                        if (brother.Checked!=null && brother.Checked.ToLower().Equals("true"))
                        {
                            count++;
                        }

                    }
                    if (count == brothers.Count)
                    {
                        parent.Checked = "True";
                    }
                    else if (count == 0)
                    {
                        parent.Checked = "False";
                    }
                    else
                    {
                        parent.Checked = "Vague";
                    }

                }else if (type.Equals("group"))
                {
                    List<Element> children = element.Children;
                    foreach (Element child in children)
                    {
                
                        child.Checked=   ckb.IsChecked==true?"True":"False";
                    }



                }


            }
  

        }


        private List<Element> getCheckedEmpList()
        {
            List<Element> retList = new List<Element>();
            foreach (Element group in this.empList)
            {
                foreach (Element emp in group.Children)
                {
                    if (emp.Checked!=null &&  emp.Checked.Equals("True"))
                    {
                        retList.Add(emp);
                    }
                }
            }
            return retList;
        }



        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            Meta meal = btn.DataContext as Meta;

            this.mealId = meal["id"];

        }

    }
}
