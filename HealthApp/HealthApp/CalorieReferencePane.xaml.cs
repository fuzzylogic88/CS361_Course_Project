/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */

using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HealthApp
{
    /// <summary>
    /// Interaction logic for CalorieReferencePane.xaml
    /// </summary>
    public partial class CalorieReferencePane : UserControl
    {
        public readonly string food_fname = "foodinfo.txt";

        public CalorieReferencePane()
        {
            InitializeComponent();

            ObservableCollection<ScrollListItemControl> fEntries = [];

            List<string> fdata = [.. File.ReadAllLines(food_fname)];
            if (fdata.Count > 0)
            {
                HashSet<string> s = GetFoodCategories(fdata);
                fEntries = GetFoodEntries(fdata);

                // apply bindingSource to list control
                if (fEntries.Count > 0)
                {
                    foodList.ItemsSource = fEntries;
                }

                // add categories
                foreach (string cat in s)
                {
                    userChoiceComboBox.Items.Add(cat);
                }
            }
        }

        /// <summary>
        ///  Returns a unique list of categories from file data read earlier
        /// </summary>
        /// <param name="fdata"></param>
        /// <returns></returns>
        public static HashSet<string> GetFoodCategories(List<string> fdata)
        {
            HashSet<string> strings = [];
            foreach (string f in fdata)
            {
                string[] split = f.Split(',');
                // idx 0: name, 1: desc, 2: food URL, 3: image source, 4: category
                if (!string.IsNullOrEmpty(split[4]))
                {
                    strings.Add(split[4]);
                }
            }
            return strings;
        }

        public static ObservableCollection<ScrollListItemControl> GetFoodEntries(List<string> fdata)
        {
            ObservableCollection<ScrollListItemControl> fColl = [];
            foreach (string f in fdata)
            {
                string[] split = f.Split(",");
                if (!string.IsNullOrEmpty(split[0]))
                {
                    try
                    {
                        ScrollListItemControl foodEntryControl = new()
                        {
                            FoodName = split[0],
                            Description = split[1],
                            FoodLink = split[2],
                            ImageSource = new BitmapImage(new Uri(split[3], UriKind.Relative))
                        };
                        fColl.Add(foodEntryControl);
                    }
                    catch { /*eat this, probably a bad file location*/}
                }
            }
            return fColl;
        }

        /// <summary>
        /// Sorts foods by name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = searchTextBox.Text;
            if (!string.IsNullOrEmpty(query))
            {
                List<ScrollListItemControl> sortedList = [];

                List<string> fdata = [.. File.ReadAllLines(food_fname)];
                if (fdata.Count > 0)
                {
                    foreach (string f in fdata)
                    {
                        string[] split = f.Split(",");

                        // match category and add to list
                        if (split[0].Contains(query, StringComparison.OrdinalIgnoreCase) || query.Contains(split[0], StringComparison.OrdinalIgnoreCase))
                        {
                            sortedList.Add(new ScrollListItemControl()
                            {
                                FoodName = split[0],
                                Description = split[1],
                                FoodLink = split[2],
                                ImageSource = new BitmapImage(new Uri(split[3], UriKind.Relative))
                            });
                        }
                    }
                }
                if (sortedList.Count > 0)
                {
                    foodList.ItemsSource = null;
                    foodList.ItemsSource = sortedList;
                }
            }
        }

        /// <summary>
        /// Sorts foods by category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void foodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<ScrollListItemControl> sortedList = [];
            var cBox = sender as ComboBox;
            if (cBox != null)
            {
                string selectedItemText = (string)cBox.SelectedItem;
                if (!string.IsNullOrEmpty(selectedItemText))
                {
                    List<string> fdata = [.. File.ReadAllLines(food_fname)];
                    if (fdata.Count > 0)
                    {
                        foreach (string f in fdata)
                        {
                            string[] split = f.Split(",");

                            // match category and add to list
                            if (split[4].Equals(selectedItemText, StringComparison.OrdinalIgnoreCase))
                            {
                                sortedList.Add(new ScrollListItemControl()
                                {
                                    FoodName = split[0],
                                    Description = split[1],
                                    FoodLink = split[2],
                                    ImageSource = new BitmapImage(new Uri(split[3], UriKind.Relative))
                                });
                            }
                        }
                    }
                }
            }
            if (sortedList.Count > 0)
            {
                foodList.ItemsSource = sortedList;
            }
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<ScrollListItemControl> fEntries = [];

            List<string> fdata = [.. File.ReadAllLines(food_fname)];
            if (fdata.Count > 0)
            {
                fEntries = GetFoodEntries(fdata);

                // apply bindingSource to list control
                if (fEntries.Count > 0)
                {
                    searchTextBox.Text = string.Empty;
                    userChoiceComboBox.SelectedIndex = -1;
                    foodList.ItemsSource = fEntries;
                }
            }
        }
    }
}
