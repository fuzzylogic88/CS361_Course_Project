﻿/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */

using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace HealthApp
{
    /// <summary>
    /// Interaction logic for FoodListControl.xaml
    /// </summary>
    public partial class FoodListControl : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(FoodListControl));

        /// <summary>
        /// A control to organize the FoodEntryControl item
        /// </summary>
        public FoodListControl()
        {
            InitializeComponent();
            FoodItemsControl.ItemsSource = ItemsSource;
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set
            {
                SetValue(ItemsSourceProperty, value);
                FoodItemsControl.ItemsSource = value;
            }
        }
    }
}
