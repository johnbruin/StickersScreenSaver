using System.Windows;
using Application = System.Windows.Application;

namespace StickerScreenSaver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 0 || e.Args[0].ToLower().StartsWith("/s"))
            {
                var window = new MainWindow()
                {
                    Topmost = true,
                };
                window.Show();

            }
            else if (e.Args[0].ToLower().StartsWith("/p"))
            {

            }
            else if (e.Args[0].ToLower().StartsWith("/c"))
            {

            }
        }
    }
}