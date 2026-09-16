using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RPGCardMaker.Converters
{
    public class HexToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string hex = value as string;

            if (string.IsNullOrWhiteSpace(hex))
                return Brushes.Transparent;

            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hex);
                return new SolidColorBrush(color);
            }
            catch
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}