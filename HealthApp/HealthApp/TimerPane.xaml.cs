/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */


using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace HealthApp
{
    /// <summary>
    /// Interaction logic for TimerPane.xaml
    /// </summary>
    public partial class TimerPane : UserControl
    {
        private readonly Dispatcher _dispatcher;
        public static Stopwatch paneTimer = new();

        public TimerPane()
        {
            InitializeComponent();
            _dispatcher = Application.Current.Dispatcher;
            Task.Factory.StartNew(() =>
            {
                UpdateTimerText(this,_dispatcher);
            });
        }

        private static async Task UpdateTimerText(TimerPane tp, Dispatcher dp)
        {
            DateTime lastUpdate = DateTime.MinValue;

            while (true)
            {
                // only update once a second
                if (paneTimer.IsRunning && (paneTimer.Elapsed.TotalMilliseconds - lastUpdate.Millisecond > 1000))
                {
                    if (!dp.HasShutdownStarted)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            // update label with message we received
                            tp.timerTextBlock.Text = new string($"{paneTimer.Elapsed.ToString(@"hh\:mm\:ss")}");
                        });
                    }
                }
                Thread.Sleep(100);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            paneTimer.Start();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            paneTimer.Stop();
        }
    }
}
