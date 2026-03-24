using ImagePixel.utils;
using Microsoft.Win32;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using static ImagePixel.utils.Pixelizer;
using static System.Net.Mime.MediaTypeNames;


namespace ImagePixel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SixLabors.ImageSharp.Image<Rgba32>? currentImage;
        private static int size;
        bool originalResolution = false;
        Pixelizer pixelizer = new Pixelizer();
        public PixelMethod Method { get; set; } = PixelMethod.Average;


        public MainWindow()
        {
            InitializeComponent();

            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("ImagePixel.Resources.image.jpg");
            currentImage = SixLabors.ImageSharp.Image.Load<Rgba32>(stream);

            OriginalImage.Source = pixelizer.ToBitmapImage(currentImage);

            var pixelated = Pixelate(currentImage, size, originalResolution, Method);
            ChangedImage.Source = pixelizer.ToBitmapImage(pixelated);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).SelectionEnd = e.NewValue;
            size = (int)e.NewValue;
             
            if (currentImage != null && size != 0)
            {
                var pixelated = Pixelate(currentImage, size, originalResolution, Method);
                ChangedImage.Source = pixelizer.ToBitmapImage(pixelated);
            }
        }

        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Выберите изображение";
            dialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg";
            dialog.DefaultExt = ".png";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filename = dialog.FileName;

                currentImage = SixLabors.ImageSharp.Image.Load<Rgba32>(filename);

                OriginalImage.Source = pixelizer.ToBitmapImage(currentImage);

                var pixelated = Pixelate(currentImage, size, originalResolution, Method);
                ChangedImage.Source = pixelizer.ToBitmapImage(pixelated);
                
            }
        }


        private void saveFile_Click(object sender, RoutedEventArgs e)
        {
            if (currentImage == null)
            {
                MessageBox.Show("Сначала откройте изображение!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new SaveFileDialog();
            string outFileName = $"pixelated_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            dialog.FileName = outFileName;
            dialog.DefaultExt = ".png";
            dialog.Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg)|*.jpg|All files (*.*)|*.*";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                try
                {
                    string filename = dialog.FileName;

                    var pixelatedImage = Pixelate(currentImage, size, originalResolution, Method);

                    pixelatedImage.Save(filename);

                    MessageBox.Show("Изображение успешно сохранено!", "Успех",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void checkBoxOriginalResolution_Checked(object sender, RoutedEventArgs e)
        {
            originalResolution = true;
            if (currentImage != null)
            {
                var pixelated = Pixelate(currentImage, size, originalResolution, Method);
                ChangedImage.Source = pixelizer.ToBitmapImage(pixelated);
            }
        }

        private void checkBoxOriginalResolution_Unchecked(object sender, RoutedEventArgs e)
        {
            originalResolution = false;
            if (currentImage != null)
            {
                var pixelated = Pixelate(currentImage, size, originalResolution, Method);
                ChangedImage.Source = pixelizer.ToBitmapImage(pixelated);
            }
        }

        private void TextBlock_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DevWindow devWindow = new DevWindow(this);
            devWindow.Show();
        }
    }
}