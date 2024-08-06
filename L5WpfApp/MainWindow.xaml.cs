using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace L5WpfApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button lastSelectedColor;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG (*.png)|*.png|Все файлы (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                BitmapImage bitmapImage = new BitmapImage(new Uri(filePath, UriKind.Absolute));
                inkCanas.Strokes.Clear();
                inkCanas.Background = new ImageBrush(bitmapImage);
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var pSource = PresentationSource.FromVisual(Application.Current.MainWindow);
            Matrix m = pSource.CompositionTarget.TransformToDevice;
            double dpiX = m.M11 * 96;
            double dpiY = m.M22 * 96;

            int width = (int)this.inkCanas.ActualWidth;
            int height = (int)this.inkCanas.ActualHeight;


            var elementBitmap = new RenderTargetBitmap(width, height, dpiX, dpiY, PixelFormats.Default);

            var drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                var visualBrush = new VisualBrush(inkCanas);
                drawingContext.DrawRectangle(
                    visualBrush,
                    null,
                    new Rect(new Point(0, 0), new Size(width / m.M11, height / m.M22)));
            }

            elementBitmap.Render(drawingVisual);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(elementBitmap));

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG (*.png)|*.png|Все файлы (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                using (var imageFile = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    encoder.Save(imageFile);
                    imageFile.Flush();
                    imageFile.Close();
                }
            }

        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (lastSelectedColor != null)
            {
                lastSelectedColor.BorderThickness = new Thickness(1);
                lastSelectedColor.BorderBrush = Brushes.Black;
            }

            Button clickedButton = sender as Button;
            clickedButton.BorderThickness = new Thickness(3);
            clickedButton.BorderBrush = Brushes.Blue;

            lastSelectedColor = clickedButton;

            inkCanas.DefaultDrawingAttributes.Color = ((SolidColorBrush)clickedButton.Background).Color;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            inkCanas.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            inkCanas.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inkCanas != null)
            {
                inkCanas.DefaultDrawingAttributes.Width = (sender as Slider).Value;
                inkCanas.DefaultDrawingAttributes.Height = (sender as Slider).Value;
            }
        }
    }
}
