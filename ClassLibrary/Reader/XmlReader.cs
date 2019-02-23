using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;

namespace Artifact.Reader
{
    public class XmlReader
    {
        private XmlDocument document = new XmlDocument();

        public void Load(string path)
        {
            document.Load(path);
        }

    
        public List<T> Select<T> (string xpath)
        {
            XmlNodeList nodeList = document.SelectNodes(xpath);
            List<T> list = new List<T>();
            foreach (XmlNode node in nodeList)
            {
                T t = System.Activator.CreateInstance<T>();
                Type type = t.GetType();

                PropertyInfo pop;
                foreach (XmlAttribute attr in node.Attributes)
                {
                    pop= type.GetProperty(attr.Name);
                    if (pop != null)
                    {
                        pop.SetValue(t, attr.Value, null);
                    }
                }

                string name = node.Name;
                 pop = type.GetProperty("Type");
                if (pop != null)
                {
                    pop.SetValue(t, name, null);
                }


                list.Add(t);

            }
             
            return list;
        }


    }
}
