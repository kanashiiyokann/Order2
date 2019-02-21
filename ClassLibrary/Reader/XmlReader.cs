using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Artifact.Data;

namespace Artifact.Reader
{
    public class XmlReader
    {
        private XmlDocument document = new XmlDocument();

        public void Load(string path)
        {
            document.Load(path);
        }

        public List<Meta> Select(string xpath)
        {
            XmlNodeList nodeList = document.SelectNodes(xpath);
            List<Meta> retList = new List<Meta>();
            foreach (XmlNode node in nodeList)
            {
                retList.Add(parseNode(node));
            }

            return retList;
        }


        private Meta parseNode(XmlNode node)
        {
            if (node == null) return null;

            Meta ret = new Meta();

            foreach (XmlAttribute attr in node.Attributes)
            {
                ret[attr.Name] = attr.Value;
            }
            ret["_name"] = node.Name;
            if (node.InnerText != "") { ret["_value"] = node.InnerText; }
            if (node.HasChildNodes)
            {
                XmlNodeList children = node.ChildNodes;
                foreach (XmlNode child in children)
                {
                    ret.AppendChild(parseNode(child));
                }

            }

            return ret;
        }


    }
}
