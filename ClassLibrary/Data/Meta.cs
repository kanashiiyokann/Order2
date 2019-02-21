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
    }
}
