using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Order2.Convertor
{
  public  class CheckedConvertor : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? ret = false;
            if (value == null) return ret;
            string str = value.ToString().ToLower();
            if (str.Equals("true"))
            {
                ret = true;
            }
            else if (str.Equals("vague"))
            {
                ret = null;
            }
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string ret = "False";
            bool? bl = value as bool?;

            if (bl == true)
            {
                ret = "True";
            }
            else if (bl == null)
            {
                ret = "Vague";
            }

            return ret;
        }
    }
}
