using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Empeños.Controles
{
    public class AngleToPointConverter : IValueConverter
    {
        private const double ToRadians = Math.PI / 180;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double scale = double.Parse((string)parameter);
            double angle = (double)value;
            return new Point(
              100 + scale * Math.Sin(angle * ToRadians),
              100 - scale * Math.Cos(angle * ToRadians));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
