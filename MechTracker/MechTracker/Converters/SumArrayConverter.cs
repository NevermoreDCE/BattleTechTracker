using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using System.Linq;

namespace MechTracker.Converters
{
    public class SumArrayConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int[] arr)
                return arr.Sum();
            return 0;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
