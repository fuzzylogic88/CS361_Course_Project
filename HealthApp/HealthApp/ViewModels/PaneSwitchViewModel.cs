/*
 * HealthApp - Daniel Green (greend5@oregonstate.edu)
 * CS361, Spring 2025
 * 
 * Facilitates switching between active content in UI
 */

namespace HealthApp.ViewModels
{
    internal class PaneSwitchViewModel
    {
        public object _currentPane { get; private set; }
        public void SwitchActivePane(string name)
        {
            _currentPane = name switch
            {
                "timer" => new TimerPane(),
                "calorie" => new CalorieReferencePane(),
            };
        }
    }
}
