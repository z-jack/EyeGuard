using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Windows.UI.Notifications;

namespace EyeGuard
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Window app = null;
        ViewModel data = null;
        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        Core timer = new Core();

        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();

        public MainWindow()
        {
            //InitPath();
            Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch);
            timer.cbk = new Core.Inter(ShowToast);

            InitializeComponent();
            app = (MainWindow)Application.Current.MainWindow;
            data = (ViewModel)this.DataContext;
            InitComponentContent();
            string[] args = Environment.GetCommandLineArgs();
            if (args.Contains("StartUp"))
            {
                Hide();
            }
        }

        private void InitPath()
        {
            path = Path.Combine(path, "JackZ");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, "EyeGuard");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void InitComponentContent()
        {
            Settings.Default.Reload();
            data.Set("timer", Settings.Default.timer);
            timer.SetInterval(int.Parse(Settings.Default.timer) * 60 * 1000);
            data.Set("lockTimer", Settings.Default.lockTimer);
            data.Set("deamon", Settings.Default.deamon);
            if (rkApp.GetValue("JackZ-EyeGuard") == null)
            {
                Settings.Default["startup"] = false;
            }
            else
            {
                Settings.Default["startup"] = true;
            }
            Settings.Default.Save();
            data.Set("startup", Settings.Default.startup);
        }

        private void RegeditHandler()
        {
            if (Settings.Default.startup)
            {
                rkApp.SetValue("JackZ-EyeGuard", Assembly.GetExecutingAssembly().Location + " StartUp");
            }
            else
            {
                rkApp.DeleteValue("JackZ-EyeGuard", false);
            }
        }

        void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                if (Settings.Default.lockTimer)
                    timer.Stop();
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                if (Settings.Default.lockTimer)
                    timer.Start();
            }
        }

        private void KeyHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                Keyboard.ClearFocus();
        }

        private void FormApply(object sender, RoutedEventArgs e)
        {

            switch (((ButtonBase)sender).Content.ToString())
            {
                case "确认":
                    data.Apply(timer);
                    Settings.Default.Save();
                    RegeditHandler();
                    break;
                case "取消":
                    data.Restore();
                    break;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Blur(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                Keyboard.ClearFocus();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.deamon)
                Hide();
            else
                Application.Current.Shutdown();
        }

        private void Show(object sender, RoutedEventArgs e)
        {
            Show();
        }

        private void Hide(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ShowToast(object sender, EventArgs e)
        {
            // Get a toast XML template
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);

            // Fill in the text elements
            var stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode("护眼小分队-提示"));
            stringElements[1].AppendChild(toastXml.CreateTextNode("是时候休息一下啦！\n点击气泡可以快速锁屏哦_(:з」∠)_"));

            var toast = new ToastNotification(toastXml);
            toast.Activated += Toast_Activated;

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier("JackZToast").Show(toast);
        }

        private void Toast_Activated(ToastNotification sender, object args)
        {
            LockWorkStation();
        }
    }

}
