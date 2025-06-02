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

        public static System.Windows.Controls.Grid? bgContentGrid;

        public MainWindow(bool guest, string username)
        {
            InitializeComponent();
            mv = new MainViewModel();
            DataContext = mv;

            bgContentGrid = MainContentBackground;

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

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadBackgroundImage();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MicroserviceHelpers.StopMicroservices();
        }

        private static async Task LoadBackgroundImage()
        {
            try
            {
                if (bgContentGrid != null)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        // Check for local, last-user-configured background and apply if available
                        if (!String.IsNullOrEmpty(Settings.Default.UserBackgroundSource) && File.Exists(Settings.Default.UserBackgroundSource))
                        {
                            // file exists and is configured, apply to app background!
                            byte[] img = File.ReadAllBytes(Settings.Default.UserBackgroundSource);

                            using var ms = new MemoryStream(img);
                            BitmapImage bmp = new();
                            bmp.BeginInit();
                            bmp.StreamSource = ms;
                            bmp.CacheOption = BitmapCacheOption.OnLoad;
                            bmp.EndInit();
                            bmp.Freeze();

                            ImageBrush ib = new()
                            {
                                ImageSource = bmp,
                                Stretch = Stretch.UniformToFill
                            };

                            bgContentGrid.Background = ib;
                        }

                        // otherwise, use default background file
                        else
                        {
                            ImageSource isrc = new BitmapImage(new Uri(@"Assets/default_bg.jpg", UriKind.Relative));
                            ImageBrush ib = new(isrc);
                            bgContentGrid.Background = ib;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message);
            }
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