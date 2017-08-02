using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace StickerScreenSaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> Stickers;
        Random rnd;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawStickers()
        {
            rnd = new Random();
            var timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += timer_Tick;
            timer.Start();
            AddSticker();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            AddSticker();
        }

        private void AddSticker()
        {
            var index = rnd.Next(Stickers.Count);
            var imgTemp = new Image()
            {
                Source = new BitmapImage(new Uri(Stickers[index]))
            };
            var x = rnd.NextDouble() * mainCanvas.Width;
            var y = rnd.NextDouble() * mainCanvas.Height;
            var scale = 0.4 + rnd.NextDouble() * 0.6;
            var rotate = -90 + rnd.Next(150);

            imgTemp.RenderTransformOrigin = new Point(0.5, 0.5);

            var rotateTransform = new RotateTransform(rotate);

            var scaleTransform = new ScaleTransform(scale, scale);

            var myTransformGroup = new TransformGroup();
            myTransformGroup.Children.Add(rotateTransform);
            myTransformGroup.Children.Add(scaleTransform);

            imgTemp.RenderTransform = myTransformGroup;

            Canvas.SetLeft(imgTemp, x - imgTemp.Source.Width * scale / 2);
            Canvas.SetTop(imgTemp, y - imgTemp.Source.Height * scale / 2);

            var myDropShadowEffect = new DropShadowEffect();

            // Set the color of the shadow to Black.
            Color myShadowColor = new Color()
            {
                ScA = 1,
                ScB = 0,
                ScG = 0,
                ScR = 0
            };
            myDropShadowEffect.Color = myShadowColor;

            // Set the direction of where the shadow is cast to 320 degrees.
            myDropShadowEffect.Direction = 320;

            // Set the depth of the shadow being cast.
            myDropShadowEffect.ShadowDepth = 5;

            // Set the shadow opacity to half opaque or in other words - half transparent.
            // The range is 0-1.
            myDropShadowEffect.Opacity = 0.5;

            imgTemp.Effect = myDropShadowEffect;

            mainCanvas.Children.Add(imgTemp);
        }

        private void LoadStickers(string folderPath)
        {
            Stickers = new List<string>();
            foreach (string filename in Directory.EnumerateFiles(folderPath, "*.png", SearchOption.AllDirectories))
            {
                Stickers.Add(filename);
            }
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow.WindowState = WindowState.Maximized;
            mainCanvas.Width = mainWindow.Width;
            mainCanvas.Height = mainWindow.Height;

            LoadStickers(AppDomain.CurrentDomain.BaseDirectory + @"\Stickers");

            DrawStickers();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}