using System.Windows;
using System.Windows.Controls;
using RPGCardMaker.ViewModels;

namespace RPGCardMaker.UserControls
{
    public partial class EditorView : UserControl
    {
        public EditorView()
        {
            InitializeComponent();
        }

        // Коли користувач клікає на поле Назви
        private void NameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && DataContext is MainViewModel vm)
            {
                // Якщо там дефолтний текст (укр або англ), очищаємо поле при кліку
                if (textBox.Text == "Нова картка" || textBox.Text == "New Card")
                {
                    textBox.Text = string.Empty;
                }
            }
        }

        // Коли користувач виходить з поля Назви і воно пусте — повертаємо підказку
        private void NameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && DataContext is MainViewModel vm)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = vm.UIStrings["DefaultName"];
                }
            }
        }

        // Коли користувач клікає на поле Опису
        private void DescTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && DataContext is MainViewModel vm)
            {
                if (textBox.Text == "Введіть опис тут..." || textBox.Text == "Enter description here...")
                {
                    textBox.Text = string.Empty;
                }
            }
        }

        // Коли користувач виходить з поля Опису і воно пусте — повертаємо підказку
        private void DescTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && DataContext is MainViewModel vm)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = vm.UIStrings["DefaultDesc"];
                }
            }
        }
    }
}