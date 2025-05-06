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
        public static List<string> FoodList = [];
        public readonly string food_fname = "foodinfo.txt";

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

        public void GetFoodCategories()
        {

        }

        public bool FoodExists(string name)
        {
            try
            {
                List<string> accountCollection = [.. File.ReadAllLines(account_fname)];
                foreach (string account in accountCollection)
                {
                    string[] split = account.Split(',');
                    if (split.Length == 2)
                    {
                        if (split[0] == user && pass == string.Empty)
                        {
                            return true;
                        }
                        else if (split[0] == user && !string.IsNullOrEmpty(pass))
                        {
                            if (split[1] == pass)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
