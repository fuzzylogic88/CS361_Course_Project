/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */

using System.Windows;

namespace HealthApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(bool guest, string username)
        {
            InitializeComponent();

            if (guest)
            {
                this.Title = "HealthApp - Guest";
            }
            else
            {
                this.Title = $"HealthApp - {username}";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SettingsPaneButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TimerPaneButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FoodReferencePaneButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataViewPaneButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TrackerPaneButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}