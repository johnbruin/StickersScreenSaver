using System;
using System.Collections.Generic;
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

            var settings = Settings.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Stickers.xml");
            var savedSets = settings.Sets;

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
                settings.Sets = folders;
                settings.Save(AppDomain.CurrentDomain.BaseDirectory + @"\Stickers.xml");
                Close();
            }
        }
    }
}
