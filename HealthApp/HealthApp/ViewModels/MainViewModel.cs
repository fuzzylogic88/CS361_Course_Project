/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 */

using System.ComponentModel;

namespace HealthApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _isEventSet;
        private string? _labelText;

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}