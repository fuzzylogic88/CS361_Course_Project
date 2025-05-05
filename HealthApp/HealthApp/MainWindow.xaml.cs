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

    }
}