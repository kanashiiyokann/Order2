using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artifact.Data
{

    public class Meta : Dictionary<String, String>
    {
        public List<Meta> Children { set; get; }
        public Meta Parent { set; get; }
        public new String this[string key]
        {

            get
            {
                String str = "";
                if (this.ContainsKey(key))
                {

                    str = base[key];
                }
                return str;
            }
            set
            {
                if (this.ContainsKey(key))
                {
                    this.Remove(key);
                }
                this.Add(key, value);
            }
        }
        public bool HasChild()
        {
            return this.Children != null && this.Children.Count != 0;
        }
        public bool HasParent()
        {
            return this.Parent != null ;
        }
        public void AppendChild(Meta child)
        {
            if (Children == null)
            {
                Children = new List<Meta>();
            }
            Children.Add(child);
            child.Parent = this;
        }

        public Meta() { }
        public static Meta Parse(Dictionary<string,object> dict) {

            Meta ret = new Meta();
            foreach(KeyValuePair<string, object> ky in dict)
            {
                ret[ky.Key.ToString()] = ky.Value.ToString();

            }

            return ret;

        }
        public static List<Meta> Parse(List<Dictionary<string, object>> dictList)
        {

            List<Meta> retList = new List<Meta>();
            foreach (Dictionary<string, object> dict in dictList)
            {
                retList.Add(Meta.Parse(dict));

            }

            return retList;

        }
    }
}
