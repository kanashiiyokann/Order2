using Artifact.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Text.RegularExpressions;

namespace Order2.Convertor
{
    class MealTipConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Meta meta = value as Meta;
            StringBuilder sb = new StringBuilder();
            if (meta != null)
            {
               
                string dices = meta["foodName"];

                 string type=    Regex.Match(dices, "（.+）").ToString();
                dices = Regex.Replace(dices, "（.+）", "");
                sb.Append(meta["mealName"]);
                sb.Append(type+"\r\n");
                dices = Regex.Replace(dices, " ", "\r\n");
                sb.Append(dices+ "\r\n");
                sb.Append(meta["haveMealTime"].Split(' ')[0]);
                sb.Append(" "+meta["week"]+" " + meta["mealTypeName"]);

            }
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
