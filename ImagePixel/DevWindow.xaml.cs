using ImagePixel.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImagePixel
{
    /// <summary>
    /// Логика взаимодействия для DevWindow.xaml
    /// </summary>
    public partial class DevWindow : Window
    {
        private MainWindow mainWindow;

        public DevWindow(MainWindow mw)
        {
            InitializeComponent();
            mainWindow = mw;

            switch (mainWindow.Method)
            {
                case Pixelizer.PixelMethod.Average:
                    AverageRB.IsChecked = true;
                    break;
                case Pixelizer.PixelMethod.Contrast:
                    ContrastRB.IsChecked = true;
                    break;
                case Pixelizer.PixelMethod.FirstPixel:
                    ExperimentalRB.IsChecked = true;
                    break;
            }
        }

        private void AverageRB_Checked(object sender, RoutedEventArgs e)
        {
            mainWindow.Method = Pixelizer.PixelMethod.Average;
        }

        private void ContrastRB_Checked(object sender, RoutedEventArgs e)
        {
            mainWindow.Method = Pixelizer.PixelMethod.Contrast;
        }

        private void ExperimentalRB_Checked(object sender, RoutedEventArgs e)
        {
            mainWindow.Method = Pixelizer.PixelMethod.FirstPixel;
        }
    }
}
