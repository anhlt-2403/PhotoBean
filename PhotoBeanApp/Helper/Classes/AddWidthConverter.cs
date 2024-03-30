using System;
using System.Globalization;
using System.Windows.Data;

namespace PhotoBeanApp.Helper.Classes
{
    public class AddWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 2 && values[0] is double leftWidth && values[1] is double rightWidth)
            {
                return leftWidth + rightWidth - (leftWidth + rightWidth) / 5 * 2;
            }

            return 200;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
