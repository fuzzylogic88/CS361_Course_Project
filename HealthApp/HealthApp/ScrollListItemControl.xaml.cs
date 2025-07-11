﻿/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HealthApp
{
    /// <summary>
    /// Interaction logic for ScrollListItemControl.xaml
    /// </summary>
    public partial class ScrollListItemControl : UserControl
    {
        public ScrollListItemControl()
        {
            InitializeComponent();
            this.MouseDoubleClick += OnMouseDoubleClick;
            DataContext = this;
        }
        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // determine which control was clicked by the user and open the URL in a browser
            if (!string.IsNullOrEmpty(FoodLink))
            {
                OpenLinkInBrowser(FoodLink);
            }
        }

        public static readonly DependencyProperty ImageSourceDP =
            DependencyProperty.Register("FoodImageSource", typeof(ImageSource), typeof(ScrollListItemControl));

        public static readonly DependencyProperty FoodNameDP =
            DependencyProperty.Register("FoodName", typeof(string), typeof(ScrollListItemControl));

        public static readonly DependencyProperty DescriptionDP =
            DependencyProperty.Register("FoodDescription", typeof(string), typeof(ScrollListItemControl));

        public static readonly DependencyProperty InternetLocationDP = 
            DependencyProperty.Register("FoodLink",typeof(string), typeof(ScrollListItemControl));   

        public string FoodLink
        {
            get => (string)GetValue(InternetLocationDP);
            set => SetValue(InternetLocationDP, value);
        }

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceDP);
            set => SetValue(ImageSourceDP, value);
        }
        public string FoodName
        {
            get => (string)GetValue(FoodNameDP);
            set => SetValue(FoodNameDP, value);
        }
        public string Description
        {
            get => (string)GetValue(DescriptionDP);
            set => SetValue(DescriptionDP, value);
        }

        private void OpenLinkInBrowser(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}
