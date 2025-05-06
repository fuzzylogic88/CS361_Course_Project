/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */

using HealthApp.ViewModels;
using System.Windows;

namespace HealthApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mv;
        public MainWindow(bool guest, string username)
        {
            InitializeComponent();
            mv = new MainViewModel();
            DataContext = mv;

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
        }

        private void SettingsPaneButton_Click(object sender, RoutedEventArgs e)
        {

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