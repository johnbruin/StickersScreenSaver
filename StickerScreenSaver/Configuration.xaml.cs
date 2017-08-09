using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StickerScreenSaver
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class Configuration : Window
    {
        List<CheckBox> StickerSets;

        public Configuration()
        {
            InitializeComponent();

            var myDropShadowEffect = new DropShadowEffect();

            // Set the color of the shadow to Black.
            var myShadowColor = new Color()
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

            imgOrdina.Effect = myDropShadowEffect;

            var settings = Settings.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Stickers.xml");
            var savedSets = settings.Sets;
            sliderSpeed.Value = settings.Speed;

            var folderPath = AppDomain.CurrentDomain.BaseDirectory + @"\Stickers";
            StickerSets = new List<CheckBox>();
            foreach (var foldername in Directory.EnumerateDirectories(folderPath))
            {
                var checkBox = new CheckBox { Content = foldername.Split('\\').Last() };
                if (savedSets.Contains(foldername))
                {
                    checkBox.IsChecked = true;
                }
                StickerSets.Add(checkBox);
                stpStickerSets.Children.Add(StickerSets.Last());                
            }
            if (!StickerSets.Any(c => c.IsChecked == true))
            {
                foreach (var cb in StickerSets)
                {
                    cb.IsChecked = true;
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!StickerSets.Where(s => s.IsChecked == true).Any())
            {
                MessageBox.Show("Please select at least one sticker set!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                var folders = string.Empty;
                foreach (var set in StickerSets.Where(s => s.IsChecked == true))
                {
                    folders += AppDomain.CurrentDomain.BaseDirectory + @"\Stickers\" + set.Content + ";";
                }
                var settings = new Settings();
                settings.Speed = sliderSpeed.Value;
                settings.Sets = folders;
                settings.Save(AppDomain.CurrentDomain.BaseDirectory + @"\Stickers.xml");
                Close();
            }
        }
    }
}
