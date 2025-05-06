/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */

using HealthApp.ViewModels;
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

            ObservableCollection<FoodEntryControl> fEntries = [];

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

        public static ObservableCollection<FoodEntryControl> GetFoodEntries(List<string> fdata)
        {
            ObservableCollection<FoodEntryControl> fColl = [];
            foreach (string f in fdata)
            {
                string[] split = f.Split(",");
                if (!string.IsNullOrEmpty(split[0]) && !string.IsNullOrEmpty(split[1]))
                {
                    try
                    {
                        FoodEntryControl foodEntryControl = new()
                        {
                            FoodName = split[0],
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
                List<FoodEntryControl> sortedList = [];

                List<string> fdata = [.. File.ReadAllLines(food_fname)];
                if (fdata.Count > 0)
                {
                    foreach (string f in fdata)
                    {
                        string[] split = f.Split(",");

                        // match category and add to list
                        if (split[0].Contains(query, StringComparison.OrdinalIgnoreCase) || query.Contains(split[0], StringComparison.OrdinalIgnoreCase))
                        {
                            sortedList.Add(new FoodEntryControl()
                            {
                                FoodName = split[0],
                                Description = split[1],
                                ImageSource = new BitmapImage(new Uri(split[2], UriKind.Relative))
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
            List<FoodEntryControl> sortedList = [];
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
                            if (split[3].Equals(selectedItemText, StringComparison.OrdinalIgnoreCase))
                            {
                                sortedList.Add(new FoodEntryControl()
                                {
                                    FoodName = split[0],
                                    Description = split[1],
                                    ImageSource = new BitmapImage(new Uri(split[2], UriKind.Relative))
                                });
                            }
                        }
                    }
                }
            }
            if (sortedList.Count > 0)
            {
                foodList.ItemsSource = null;
                foodList.ItemsSource = sortedList;
            }
        }



        //public bool FoodExists(string name)
        //{
        //    try
        //    {
        //        List<string> accountCollection = [.. File.ReadAllLines(account_fname)];
        //        foreach (string account in accountCollection)
        //        {
        //            string[] split = account.Split(',');
        //            if (split.Length == 2)
        //            {
        //                if (split[0] == user && pass == string.Empty)
        //                {
        //                    return true;
        //                }
        //                else if (split[0] == user && !string.IsNullOrEmpty(pass))
        //                {
        //                    if (split[1] == pass)
        //                    {
        //                        return true;
        //                    }
        //                    else
        //                    {
        //                        return false;
        //                    }
        //                }
        //            }
        //        }
        //        return false;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
    }
}
