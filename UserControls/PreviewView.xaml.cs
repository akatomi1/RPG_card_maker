using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RPGCardMaker.UserControls
{
    public partial class PreviewView : UserControl
    {
        public PreviewView()
        {
            InitializeComponent();
        }

        private void ExportToPng_Click(object sender, RoutedEventArgs e)
        {
            // Відкриваємо діалог для збереження файлу
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Зберегти картку",
                Filter = "Зображення PNG (*.png)|*.png",
                FileName = "МояКартка.png" // Ім'я за замовчуванням
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                // 1. Отримуємо фізичні розміри нашої рамки (CardContainer)
                int width = (int)CardContainer.ActualWidth;
                int height = (int)CardContainer.ActualHeight;

                if (width == 0 || height == 0) return;

                // 2. Створюємо "полотно" для рендеру з роздільною здатністю екрану (96 DPI)
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                    width, height, 96, 96, PixelFormats.Pbgra32);

                // 3. Малюємо картку на полотні
                renderBitmap.Render(CardContainer);

                // 4. Кодуємо полотно у формат PNG
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                // 5. Записуємо файл на диск
                using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                MessageBox.Show("Картку успішно збережено на комп'ютер!", "Експорт", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}