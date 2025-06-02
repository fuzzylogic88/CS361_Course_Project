/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */

using HealthApp.Properties;
using HealthApp.ViewModels;
using NetMQ;
using NetMQ.Sockets;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace HealthApp
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SettingsPane : UserControl
    {
        private Dispatcher _dispatcher;
        private static readonly string image_dest = @"Assets/user_background.png";
        public SettingsPane()
        {
            InitializeComponent();
        }
        public void ImageSelectButton_Click(object sender, RoutedEventArgs e)
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
            try
            {
                // connect to microservice
                using var client = new RequestSocket();

                client.Connect("tcp://127.0.0.1:5557");
                client.SendFrame("IMG");

                // get byte array data from microservice
                byte[] img = client.ReceiveFrameBytes();

                BitmapImage bmp = new();
                using (var ms = new MemoryStream(img))
                {
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.StreamSource = ms;
                    bmp.EndInit();
                    bmp.Freeze();
                }

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // save file locally for later use
                    JpegBitmapEncoder enc = new();
                    enc.Frames.Add(BitmapFrame.Create(bmp));

                    using var fstream = new FileStream(image_dest, FileMode.Create);
                    enc.Save(fstream);

                    Settings.Default.UserBackgroundSource = image_dest;
                    Settings.Default.Save();

                    // finally, apply the new background to the UI
                    if (!dispatcher.HasShutdownStarted)
                    {

                        ImageBrush ib = new()
                        {
                            ImageSource = bmp,
                            Stretch = Stretch.UniformToFill
                        };
                        MainWindow.bgContentGrid.Background = ib;
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message);
            }
        }
    }
}
