using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RPGCardMaker.Converters
{
    // Перетворює текстову рідкість картки на колір рамки PreviewView
    public class RarityToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string rarity = value as string ?? "";

            return rarity switch
            {
                "Рідкісна" or "Rare" => new SolidColorBrush(Color.FromRgb(0x25, 0x63, 0xEB)),     
                "Епічна" or "Epic" => new SolidColorBrush(Color.FromRgb(0x7C, 0x3A, 0xED)),        
                "Легендарна" or "Legendary" => new SolidColorBrush(Color.FromRgb(0xB4, 0x53, 0x09)),
                _ => new SolidColorBrush(Color.FromRgb(0x4B, 0x55, 0x63)),                       
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}