/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */

using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HealthApp
{
    /// <summary>
    /// Interaction logic for CalorieReferencePane.xaml
    /// </summary>
    public partial class CalorieReferencePane : UserControl
    {
        public CalorieReferencePane()
        {
            InitializeComponent();

            var foods = new List<FoodEntryControl>()
            {
                new() {
                    ImageSource = new BitmapImage(new Uri("/Assets/FoodImages/burger.png", UriKind.Relative)),
                    FoodName = "Burger",
                    Description = "Burger text description"
                },
                new() {
                    ImageSource = new BitmapImage(new Uri("/Assets/FoodImages/burrito.png", UriKind.Relative)),
                    FoodName = "Burrito",
                    Description = "Burrito text description"
                },
            };
            foodList.ItemsSource = foods;
        }
    }
}
