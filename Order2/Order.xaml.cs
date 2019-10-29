using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private delegate void ShowRecordsDelegate(List<String> records);
        private delegate void AlertOrderResultDelegate(List<String> failedList,List<Element> empList);
        

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


            Thread th = new Thread(new ParameterizedThreadStart((args)=> {
                List<Element> emplyoeeList=args as List<Element>;

                String areaCode = cfg.Read("setting", "addressId");
            List<String> failedList=    orderService.OrderMeal(this.mealId, areaCode,emplyoeeList);
                Dispatcher.BeginInvoke(new AlertOrderResultDelegate((List<String> fl, List<Element> empList)=> {
                    int failedCount = fl.Count;
                    int total = empList.Count;
                    if (failedCount == 0)
                    {
                        MessageBox.Show("全部订餐成功！");
                    }
                    else if(failedCount==total)
                    {
                        MessageBox.Show("全部订餐失败！");
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach(string name in failedList)
                        {
                            sb.Append("," + name);
                        }
                        MessageBox.Show(String.Format("{0}订餐失败，其余成功！",sb.Remove(0,1).ToString()),"订餐发生失败!");

                    }


                }),new object[] { failedList, emplyoeeList });

            }));
            th.Start(emps);



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
            String areaCode = cfg.Read("setting", "addressId");
           List<Meta> meals=  orderService.GetMealList(areaCode);

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
                    if (emp.Checked!=null &&  emp.Checked.ToLower().Equals("true"))
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
       
            Thread th = new Thread(new ThreadStart(doGetRecord));
            th.Start();
       

        }

        private void doGetRecord()
        {
            List<Element> empList = getCheckedEmpList();
            if (empList.Count == 0)
            {
                MessageBox.Show("请选择要查询的人员！");
                return;

            }
            List<String> mealRecordList = orderService.GetOrderRecord(empList);

            Dispatcher.BeginInvoke(new ShowRecordsDelegate( (List<String> records)=> {
                Records window = new Records(records);
                window.ShowDialog();
            }), mealRecordList);

          

        }

    
    }
}
