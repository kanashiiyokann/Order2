using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Order2.Entity
{
    public class Element : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public String Name { get; set; }
        public String Type { get; set; }

        public String No { get; set; }
        private string choosed { get; set; }

        public string Checked { get { return choosed; } set { choosed = value; OnPropertyChanged("Checked"); } }

        private List<Element> children;
        public List<Element> Children
        {
            set
            {
                foreach (Element child in value)
                {
                    this.AddChild(child);
                }
            }
            get { return children; }
        }
        public Element Parent { set; get; }

        public Element AddChild(Element child)
        {
            if (children == null) children = new List<Element>();
            children.Add(child);
            child.Parent = this;
    return this;

        }
    }
}
