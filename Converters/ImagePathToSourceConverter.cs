using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace RPGCardMaker.Converters
{
    public class ImagePathToSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value as string;

            if (string.IsNullOrWhiteSpace(path))
                return null;

            try
            {
                string fullPath = Path.IsPathRooted(path)
                    ? path
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

                if (!File.Exists(fullPath))
                    return null;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // щоб файл не лишався заблокованим на диску
                bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                bitmap.EndInit();
                return bitmap;
            }
            catch
            {
                return null; 
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}