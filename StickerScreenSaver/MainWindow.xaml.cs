using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Configuration;

namespace StickerScreenSaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> StickerPaths = new List<string>();
        List<Image> Stickers = new List<Image>();
        Random rnd;
        int index;
        const int MAXSTICKERS = 300;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawStickers()
        {
            rnd = new Random();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        TimeSpan LastRenderTime;
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            TimeSpan timeSinceLastRender;

            timeSinceLastRender = (DateTime.Now.TimeOfDay - LastRenderTime);
            if (timeSinceLastRender.TotalSeconds < 1)
                return;

            LastRenderTime = DateTime.Now.TimeOfDay;
            AddSticker();
        }

        private void AddSticker()
        {
            if (StickerPaths.Count == 0)
            {
                return;
            }

            var i = rnd.Next(StickerPaths.Count);

            var imgTemp = new Image()
            {
                Source = new BitmapImage(new Uri(StickerPaths[i]))
            };
            var x = rnd.NextDouble() * mainCanvas.Width;
            var y = rnd.NextDouble() * mainCanvas.Height;
            var scale = 0.6 + rnd.NextDouble() * 0.5;
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

            if (Stickers.Count < index + 1)
            {
                Stickers.Add(imgTemp);
                mainCanvas.Children.Add(Stickers.Last());
            }
            else
            {
                mainCanvas.Children.Remove(Stickers[index]);
                Stickers[index] = imgTemp;
                mainCanvas.Children.Add(Stickers[index]);
            }
            index++;
            if (index > MAXSTICKERS)
            {
                index = 0;
            }
        }

        private void LoadStickers(string folderPath)
        {
            var settings = Settings.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Stickers.xml");
            StickerPaths = new List<string>();

            var savedSets = settings.Sets.Split(';');
            foreach (var setPath in savedSets)
            {                
                if (!string.IsNullOrEmpty(setPath))
                {
                    foreach (string filename in Directory.EnumerateFiles(setPath, "*.png", SearchOption.AllDirectories))
                    {
                        StickerPaths.Add(filename);
                    }
                }
            }
            if (!StickerPaths.Any())
            {
                foreach (string filename in Directory.EnumerateFiles(folderPath, "*.png", SearchOption.AllDirectories))
                {
                    StickerPaths.Add(filename);
                }
            }
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow.WindowState = WindowState.Maximized;

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