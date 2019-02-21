using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Artifact.Reader;
using Artifact.Data;

namespace WinOrder
{
    public partial class Order : Form
    {
        private string appPath = System.AppDomain.CurrentDomain.BaseDirectory;
        public Order()
        {
            InitializeComponent();
        }

        private void Order_Load(object sender, EventArgs e)
        {
            //加載所有用戶
            XmlReader xmlReader = new XmlReader();
            xmlReader.Load(this.appPath + "//Resource//employees.xml");
            List<Meta> employeeList = xmlReader.Select("/employees")[0].Children;

            this.listView1.BeginUpdate();
            this.listView1.CheckBoxes = true;
            foreach (Meta emp in employeeList)
            {
                ListViewItem item = new ListViewItem();
                item.Text = emp["name"];
                item.Tag = emp;
               

                this.listView1.Items.Add(item);

            }
            this.listView1.EndUpdate();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {

        }
    }
}
