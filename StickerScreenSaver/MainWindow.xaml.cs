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

namespace StickerScreenSaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double Speed;
        List<string> StickerPaths = new List<string>();
        Random rnd;

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
            if (timeSinceLastRender.TotalSeconds < Speed)
                return;

            LastRenderTime = DateTime.Now.TimeOfDay;
            AddSticker();
            var bitmap = SaveAsWriteableBitmap(mainCanvas);
            mainCanvas.Children.Clear();
            var brush = new ImageBrush();
            brush.ImageSource = bitmap;
            mainCanvas.Background = brush;
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
            var scale = 0.6 + rnd.NextDouble() * 0.5;
            var x = rnd.NextDouble() * mainCanvas.Width;
            var y = rnd.NextDouble() * mainCanvas.Height;
            
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
            var settings = Settings.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Stickers.xml");
            StickerPaths = new List<string>();
            Speed = 2 - settings.Speed;
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

        public WriteableBitmap SaveAsWriteableBitmap(Canvas surface)
        {
            if (surface == null) return null;

            // Save current canvas transform
            Transform transform = surface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;

            // Get the size of canvas
            Size size = new Size(surface.ActualWidth, surface.ActualHeight);
            // Measure and arrange the surface
            // VERY IMPORTANT
            surface.Measure(size);
            surface.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
              (int)size.Width,
              (int)size.Height,
              96d,
              96d,
              PixelFormats.Pbgra32);
            renderBitmap.Render(surface);


            //Restore previously saved layout
            surface.LayoutTransform = transform;

            //create and return a new WriteableBitmap using the RenderTargetBitmap
            return new WriteableBitmap(renderBitmap);
        }
    }
}