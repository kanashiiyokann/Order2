using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Text.RegularExpressions;
using Artifact.Data;

namespace Order2.Convertor
{
    class MealInfoConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Meta meta = value as Meta;
            StringBuilder sb = new StringBuilder();
            if (meta != null)
            {
                string dices = meta["foodName"];

                string type = Regex.Match(dices, "（.+）").ToString();
                dices = Regex.Replace(dices, "（.+）", "");
                sb.Append(meta["mealName"]);
                sb.Append(type + "\r\n");
                dices = Regex.Replace(dices, " ", "\r\n");
                sb.Append(dices);
            }
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
