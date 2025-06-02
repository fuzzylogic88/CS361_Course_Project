/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */

using HealthApp.Properties;
using HealthApp.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HealthApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mv;

        private readonly string image_scriptname = "ImageServer.py";

        public MainWindow(bool guest, string username)
        {
            InitializeComponent();
            mv = new MainViewModel();
            DataContext = mv;

            // Check for local, last-user-configured background and apply if available
            if (!String.IsNullOrEmpty(Settings.Default.UserBackgroundSource) && File.Exists(Settings.Default.UserBackgroundSource))
            {
                // file exists and is configured, apply to app background!
                ImageSource isrc = new BitmapImage(new Uri(Settings.Default.UserBackgroundSource));
                ImageBrush ib = new(isrc);
                MainContentBackground.Background = ib;
            }
            // otherwise, use default background file
            else
            {
                ImageSource isrc = new BitmapImage(new Uri("/Assets/default_bg.jpg"));
                ImageBrush ib = new(isrc);
                MainContentBackground.Background = ib;           
            }

            if (guest)
            {
                this.Title = "HealthApp - Guest";
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBoxEx.Show(this, "Running in GUEST mode.\nPersonal data will not be retained.", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            else
            {
                this.Title = $"HealthApp - {username}";
            }

            // Spin up Image server to wait for request
            MicroserviceHelpers.StartPythonScript(image_scriptname);
        }

        private void SettingsPaneButton_Click(object sender, RoutedEventArgs e)
        {
            mv.SwitchPaneCommand.Execute("settings");
        }

        private void TimerPaneButton_Click(object sender, RoutedEventArgs e)
        {
            mv.SwitchPaneCommand.Execute("timer");
        }

        private void FoodReferencePaneButton_Click(object sender, RoutedEventArgs e)
        {
            mv.SwitchPaneCommand.Execute("calorie");
        }

        private void DataViewPaneButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TrackerPaneButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}