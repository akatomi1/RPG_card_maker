using System;
using System.Linq;
using System.Windows;

namespace RPGCardMaker
{
    public partial class App : Application
    {
        // Метод для миттєвої зміни теми
        public static void ChangeTheme(string themeName)
        {
            var newTheme = new ResourceDictionary
            {
                Source = new Uri($"Resources/Themes/{themeName}.xaml", UriKind.Relative)
            };

            var dictionaries = Current.Resources.MergedDictionaries;

            var oldTheme = dictionaries.FirstOrDefault(d =>
                d.Source != null &&
                (d.Source.OriginalString.Contains("LightTheme") ||
                 d.Source.OriginalString.Contains("DarkTheme")));

            if (oldTheme != null)
                dictionaries.Remove(oldTheme);

            dictionaries.Add(newTheme);
        }
    }
}