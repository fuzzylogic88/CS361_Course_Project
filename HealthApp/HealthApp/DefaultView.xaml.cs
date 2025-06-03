/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */


using HealthApp.ViewModels;
using NetMQ;
using NetMQ.Sockets;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace HealthApp
{
    /// <summary>
    /// Interaction logic for DefaultView.xaml
    /// </summary>
    public partial class DefaultView : UserControl
    {
        private readonly string reminder_scriptname = "ReminderServer.py";

        string[] types = { "Run", "Cycle", "Swim", "Hike" };

        public DefaultView()
        {
            InitializeComponent();

            greetingTextBlock.Text = $"Welcome to HealthApp, {MainWindow.global_username}.";

            // grab logged-in users reminders and post to UI
            if (!string.IsNullOrEmpty(MainWindow.global_username))
            {
                // spin up reminder microservice
                MicroserviceHelpers.StartPythonScript(reminder_scriptname);

                foreach (string type in types)
                {
                    rTypeComboBox.Items.Add(type);
                }

                string dat = GetUserJSONData(MainWindow.global_username);
                ObservableCollection<ScrollListItemControl> fEntries = ParseReminders(dat);
                if (fEntries.Count > 0)
                {
                    reminderList.ItemsSource = fEntries;
                }
            }
        }

        public void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (reminderDPicker.SelectedDate.Value != null)
            {
                // connect to microservice
                using var client = new RequestSocket();

                client.Connect("tcp://127.0.0.1:5558");
                client.SendFrame($"ADD,{MainWindow.global_username},{reminderTextBox.Text},{rTypeComboBox.SelectedItem.ToString()},{reminderDPicker.SelectedDate.Value.ToString("M-dd-yyyy")}");

                var msg = client.ReceiveFrameString();
                MessageBox.Show("Reminder added!");

                string dat = GetUserJSONData(MainWindow.global_username);
                ObservableCollection<ScrollListItemControl> fEntries = ParseReminders(dat);
                if (fEntries.Count > 0)
                {
                    reminderList.ItemsSource = fEntries;
                }
                Thread.Sleep(250);
            }
        }

        [STAThread]
        public static ObservableCollection<ScrollListItemControl> ParseReminders(string user_json_data)
        {
            ObservableCollection<ScrollListItemControl> rColl = [];

            //parse json to get reminder infos
            user_json_data = user_json_data.Replace("'", "\"");
            var reminders = JsonSerializer.Deserialize<Dictionary<string, string>>(user_json_data);

            foreach (var reminder in reminders)
            {
                if (!string.IsNullOrEmpty(reminder.Value))
                {
                    var split = reminder.Value.Split(",");
                    DateTime Date = DateTime.Parse(split[0]);
                    string Activity = split[1];
                    string Type = split[2];

                    string imageloc = string.Empty;
                    if (Type == "Run")
                    {
                        imageloc = "Assets/Run.png";
                    }
                    else if (Type == "Cycle")
                    {
                        imageloc = "Assets/Cycle.png";
                    }
                    else if (Type == "Swim")
                    {
                        imageloc = "Assets/Swim.png";
                    }
                    else if (Type == "Hike")
                    {
                        imageloc = "Assets/Hike.png";
                    }

                    if (Date.Date == DateTime.Today)
                    {
                        ScrollListItemControl reminderControl = new()
                        {
                            FoodName = Activity,
                            Description = "Reminder for today: " + DateTime.Today.ToShortDateString(),
                            ImageSource = new BitmapImage(new Uri(imageloc, UriKind.Relative)),
                        };
                        rColl.Add(reminderControl);
                    }
                }
            }
            return rColl;
        }

        public static string GetUserJSONData(string username)
        {
            // connect to microservice
            using var client = new RequestSocket();

            client.Connect("tcp://127.0.0.1:5558");
            client.SendFrame($"READ,{username}");

            var msg = client.ReceiveFrameString();

            Console.WriteLine("From microservice: {0}", msg);

            return msg.ToString();
        }
    }
}
