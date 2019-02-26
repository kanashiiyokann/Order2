using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Order2
{
    class App
    {
        [STAThread]
        static void Main()
        {
            Application app = new Application();
            Order order = new Order();
            app.Run(order);

                    
        }
    }
}
