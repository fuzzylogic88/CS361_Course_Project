using HealthApp.ViewModels;
using NetMQ;
using NetMQ.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace HealthApp
{
    /// <summary>
    /// Interaction logic for SettingsPane.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        private Dispatcher _dispatcher;
        public Page1()
        {

        }
        public void PlayButton_Click()
        {
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
    }
}
