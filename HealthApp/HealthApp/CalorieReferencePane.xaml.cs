/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */

using HealthApp.ViewModels;
using System.IO;
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
        private MainViewModel mv;

        public CalorieReferencePane()
        {
            InitializeComponent();

            Task.Factory.StartNew(() =>
            {
                List<string> fdata = [.. File.ReadAllLines(food_fname)];
                if (fdata.Count > 0)
                {
                    HashSet<string> s = GetFoodCategories(fdata);
                    List<FoodEntryControl> fEntries = GetFoodEntries(fdata);

                    // apply bindingSource to list control
                    if (fEntries.Count > 0)
                    {
                        foodList.ItemsSource = fEntries;
                    }

                    // add categories
                    foreach(string cat in s)
                    {
                        userChoiceComboBox.Items.Add(cat);
                    }
                }
            });
            
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

        /// <summary>
        ///  Returns a unique list of categories from file data read earlier
        /// </summary>
        /// <param name="fdata"></param>
        /// <returns></returns>
        public HashSet<string> GetFoodCategories(List<string> fdata)
        {
            HashSet<string> strings = new();
            foreach (string f in fdata)
            {
                string[] split = f.Split(',');
                // idx 0: name, 1: desc, 2: image source, 3: category
                if (!string.IsNullOrEmpty(split[3]))
                {
                    strings.Add(split[3]);
                }
            }
            return strings;
        }
        
        public List<FoodEntryControl> GetFoodEntries(List<string> fdata)
        {
            List<FoodEntryControl> fColl = new();
            foreach (string f in fdata)
            {
                string[] split = f.Split(",");
                if (!string.IsNullOrEmpty(split[0]) && !string.IsNullOrEmpty(split[1]))
                {
                    try
                    {
                        FoodEntryControl foodEntryControl = new()
                        {
                            Name = split[0],
                            Description = split[1],
                            ImageSource = new BitmapImage(new Uri(split[2], UriKind.Relative))
                        };
                        fColl.Add(foodEntryControl);
                    }
                    catch { /*eat this, probably a bad file location*/}
                }
            }
            return fColl;
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
