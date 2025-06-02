/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */

using HealthApp.ViewModels;
using NetMQ;
using NetMQ.Sockets;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace HealthApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly Dispatcher _dispatcher;

        private readonly string account_fname = @"accounts.txt";

        /// <summary>
        /// Microservice python scripts in application directory
        /// </summary>
        private readonly string msg_scriptname = "UserMessageServer.py";
        private readonly string login_scriptname = "LoginServer.py";

        public LoginWindow()
        {
            InitializeComponent();

            // spin up python apps
            MicroserviceHelpers.StartPythonScript(msg_scriptname);
            MicroserviceHelpers.StartPythonScript(login_scriptname);

            DataContext = new MainViewModel();

            // read version into label
            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            if (ver != null)
            {
                versionTextLabel.Content = "v" + ver.ToString();
            }

            MainViewModel viewModel = (MainViewModel)this.DataContext;
            viewModel.UserMessageLabelText = "Loading message...";
            _dispatcher = Application.Current.Dispatcher;
            if (viewModel != null && _dispatcher != null)
            {
                Task.Factory.StartNew(() => UpdateUserMessageText(viewModel, _dispatcher));
            }
        }

        private static async Task UpdateUserMessageText(MainViewModel vm, Dispatcher dispatcher)
        {
            // connect to microservice
            using var client = new RequestSocket();

            client.Connect("tcp://127.0.0.1:5555");
            client.SendFrame("MSG");

            var msg = client.ReceiveFrameString();

            Console.WriteLine("From microservice: {0}", msg);

            if (!dispatcher.HasShutdownStarted)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // update label with message we received
                    vm.UserMessageLabelText = new string($"{msg}");
                });
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string user = usernameTextBox.Text;
            string pass = passwordTextBox.Password;

            if (AccountExists(user, pass))
            {
                // successful login, pop up our main window
                var mw = new MainWindow(false, user);
                mw.Show();
                this.Close();
            }
            else if (!AccountExists(user, string.Empty))
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBoxEx.Show(this, $"User name is not associated with an account!\nPress 'Register to add {user} as a new user.", "User not registered.", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            else
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBoxEx.Show(this, $"Bad credentials! Try again.", "Invalid credentials.", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            
        }

        private void GuestLoginButton_Click(object sender, RoutedEventArgs e)
        {
            // guest login, no validation.
            var mw = new MainWindow(true, string.Empty);
            mw.Show();
            this.Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string user = usernameTextBox.Text;
            string pass = passwordTextBox.Password;

            if (!AccountExists(user, string.Empty))
            {
                bool addedOk = AddAccount(user, pass);
                if (addedOk)
                {
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        MessageBoxEx.Show(this, $"User added.\nPlease log in.", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                }
                else
                {
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        MessageBoxEx.Show(this, $"Failed to add user!\nCheck input and account filepath.", "Registration error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            }
            else
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBoxEx.Show(this, $"User added.\nPlease log in.", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
        }

        public bool AddAccount(string user, string pass)
        {
            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
            {
                // tack our new user onto the end of the file
                File.AppendAllText(account_fname, new($"{Environment.NewLine}{user},{pass}"));
                return true;
            }
            return false;
        }

        public bool AccountExists(string user, string pass)
        {
            try
            {
                List<string> accountCollection = [.. File.ReadAllLines(account_fname)];
                foreach (string account in accountCollection)
                {
                    string[] split = account.Split(',');
                    if (split.Length == 2)
                    {
                        // user is in file and no password provided (presence check)
                        if (split[0] == user && pass == string.Empty)
                        {
                            return true;
                        }
                        // user in file and password provided (validation)
                        else if (split[0] == user && !string.IsNullOrEmpty(pass))
                        {
                            if (split[1] == pass)
                            {
                                return true;
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
