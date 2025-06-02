/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */

using HealthApp.ViewModels;
using NetMQ;
using NetMQ.Sockets;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Image = System.Drawing.Image;

namespace HealthApp
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SettingsPane : UserControl
    {
        private Dispatcher _dispatcher;

        public SettingsPane()
        {
            InitializeComponent();
        }
        public void ImageSelectButton_Click()
        {
            MainViewModel viewModel = (MainViewModel)this.DataContext;
            _dispatcher = Application.Current.Dispatcher;

            if (viewModel != null && _dispatcher != null)
            {
                Task.Factory.StartNew(() => BackgroundImageTask(viewModel, _dispatcher));
            }
        }

        private static async Task BackgroundImageTask(MainViewModel vm, Dispatcher dispatcher)
        {
            // connect to microservice
            using var client = new RequestSocket();

            client.Connect("tcp://127.0.0.1:5557");
            client.SendFrame("IMG");

            // get a base64 encoded image data from the microservice
            var msg = client.ReceiveFrameString();

            // decode and save to file, in application folder.
            Image new_img = B64StringToImage(msg);

            // save file locally for later use
            new_img.Save("user_background.png", System.Drawing.Imaging.ImageFormat.Png);


            if (!dispatcher.HasShutdownStarted)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // update label with message we received
                    vm.UserMessageLabelText = new string($"{msg}");
                });
            }
        }

        private static Image B64StringToImage(string b64_str)
        {
            byte[] img = Convert.FromBase64String(b64_str);
            using (var ms = new MemoryStream(img, 0, img.Length))
            {
                Image image = Image.FromStream(ms);
                return image;
            }
        }
    }
}
