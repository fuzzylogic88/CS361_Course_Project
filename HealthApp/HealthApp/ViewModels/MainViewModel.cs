/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */

using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;

namespace HealthApp.ViewModels
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        private bool _isEventSet;
        private string? _labelText;
        private object _currentPane = new DefaultView();

        public bool IsEventSet
        {
            get => _isEventSet;
            set
            {
                if (_isEventSet != value)
                {
                    _isEventSet = value;
                    OnPropertyChanged(nameof(IsEventSet));
                }
            }
        }

        public string UserMessageLabelText
        {
            get => _labelText;
            set
            {
                if (_labelText != value)
                {
                    _labelText = value;
                    OnPropertyChanged(nameof(UserMessageLabelText));
                }
            }
        }

        public object CurrentPane
        {
            get => _currentPane;
            set
            {
                if (_currentPane != value)
                {
                    _currentPane = value;
                    OnPropertyChanged(nameof(CurrentPane));
                }
            }
        }

        public ICommand SwitchPaneCommand { get; }

        public MainViewModel()
        {
            SwitchPaneCommand = new RelayCommand<string>(SwitchPane);
        }

        private void SwitchPane(string paneType)
        {
            CurrentPane = paneType switch
            {
                "timer" => new TimerPane(),
                "calorie" => new CalorieReferencePane(),
                _ => CurrentPane
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}