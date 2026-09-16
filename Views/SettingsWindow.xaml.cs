using System;
using System.Windows;
using System.Windows.Controls;

namespace RPGCardMaker.Views
{
    public partial class SettingsWindow : Window
    {
        public static event Action<string>? LanguageChanged;
        public static event Action<string>? ThemeChanged;
        private bool _isInitialized = false;

        public SettingsWindow(string currentLang, string currentTheme)
        {
            InitializeComponent();

            // Встановлюємо поточну мову у випадаючому списку без тригера подій
            foreach (ComboBoxItem item in LanguageComboBox.Items)
            {
                if (item.Tag?.ToString() == currentLang)
                {
                    item.IsSelected = true;
                    break;
                }
            }

            // Встановлюємо поточну тему у випадаючому списку
            foreach (ComboBoxItem item in ThemeComboBox.Items)
            {
                if (item.Tag?.ToString() == currentTheme)
                {
                    item.IsSelected = true;
                    break;
                }
            }

            ApplyTranslations(currentLang);
            _isInitialized = true;
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;

            if (ThemeComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
            {
                string themeName = selectedItem.Tag.ToString();
                App.ChangeTheme(themeName);
                ThemeChanged?.Invoke(themeName);
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;

            if (LanguageComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
            {
                string langCode = selectedItem.Tag.ToString();
                LanguageChanged?.Invoke(langCode);
                ApplyTranslations(langCode);
            }
        }

        private void ApplyTranslations(string langCode)
        {
            if (CloseBtn == null || ThemeLabel == null || LightItem == null || DarkItem == null || LangLabel == null)
                return;

            if (langCode == "en")
            {
                Title = "Settings";
                ThemeLabel.Text = "Theme Appearance:";
                LightItem.Content = "Light Theme";
                DarkItem.Content = "Dark Theme";
                LangLabel.Text = "Interface Language:";
                CloseBtn.Content = "Close";
            }
            else
            {
                Title = "Налаштування";
                ThemeLabel.Text = "Тема оформлення:";
                LightItem.Content = "Світла тема";
                DarkItem.Content = "Темна тема";
                LangLabel.Text = "Мова інтерфейсу:";
                CloseBtn.Content = "Закрити";
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}