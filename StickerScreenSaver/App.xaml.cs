using System.Diagnostics;
using System.Windows;
using Application = System.Windows.Application;
using System.Linq;
using System.Windows.Interop;
using System;

namespace StickerScreenSaver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Used to host WPF content in preview mode, attach HwndSource to parent Win32 window.
        private HwndSource winWPFContent;
        private MainWindow winSaver;

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            var thisprocessname = Process.GetCurrentProcess().ProcessName;
            var currentInstances = Process.GetProcesses().Where(p => p.ProcessName == thisprocessname);
            foreach (var instance in currentInstances)
            {
                if (Process.GetCurrentProcess().Id != instance.Id)
                {
                    instance.Kill();
                }
            }

            if (e.Args.Length == 0 || e.Args[0].ToLower().StartsWith("/s"))
            {
                ShowScreensaver();
            }
            else if (e.Args[0].ToLower().StartsWith("/p"))
            {
                winSaver = new MainWindow();

                Int32 previewHandle = Convert.ToInt32(e.Args[1]);

                IntPtr pPreviewHnd = new IntPtr(previewHandle);

                RECT lpRect = new RECT();
                bool bGetRect = Win32API.GetClientRect(pPreviewHnd, ref lpRect);

                HwndSourceParameters sourceParams = new HwndSourceParameters("sourceParams");

                sourceParams.PositionX = 0;
                sourceParams.PositionY = 0;
                sourceParams.Height = lpRect.Bottom - lpRect.Top;
                sourceParams.Width = lpRect.Right - lpRect.Left;
                sourceParams.ParentWindow = pPreviewHnd;
                sourceParams.WindowStyle = (int)(WindowStyles.WS_VISIBLE | WindowStyles.WS_CHILD | WindowStyles.WS_CLIPCHILDREN);

                winWPFContent = new HwndSource(sourceParams);
                winWPFContent.Disposed += new EventHandler(winWPFContent_Disposed);
                winWPFContent.RootVisual = winSaver.mainViewbox;
            }
            else if (e.Args[0].ToLower().StartsWith("/c"))
            {
                var configurationWindow = new Configuration();
                configurationWindow.Show();
            }            
        }

        /// <summary>
        /// Event that triggers when parent window is disposed--used when doing
        /// screen saver preview, so that we know when to exit.  If we didn't 
        /// do this, Task Manager would get a new .scr instance every time
        /// we opened Screen Saver dialog or switched dropdown to this saver.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void winWPFContent_Disposed(object sender, EventArgs e)
        {
            winSaver.Close();
        }

        /// <summary>
        /// Shows screen saver by creating one instance of Window1 for each monitor.
        /// 
        /// Note: uses WinForms's Screen class to get monitor info.
        /// </summary>
        public static void ShowScreensaver()
        {
            //creates window on primary screen
            var primaryWindow = new MainWindow();
            primaryWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            System.Drawing.Rectangle location = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            primaryWindow.WindowState = WindowState.Maximized;

            //creates window on other screens
            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                if (screen == System.Windows.Forms.Screen.PrimaryScreen)
                    continue;

                var window = new MainWindow();
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                location = screen.Bounds;

                //covers entire monitor
                window.Left = location.X - 7;
                window.Top = location.Y - 7;
                window.Width = location.Width + 14;
                window.Height = location.Height + 14;
            }

            //show non-primary screen windows
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window != primaryWindow)
                    window.Show();
            }

            ///shows primary screen window last
            primaryWindow.Show();
        }
    }
}