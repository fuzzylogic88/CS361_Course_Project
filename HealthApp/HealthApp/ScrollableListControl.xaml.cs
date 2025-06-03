/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */

using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace HealthApp
{
    /// <summary>
    /// Interaction logic for ScrollableListControl.xaml
    /// </summary>
    public partial class ScrollableListControl : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ScrollableListControl));

        /// <summary>
        /// A control to organize the ScrollListItemControl item
        /// </summary>
        public ScrollableListControl()
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
