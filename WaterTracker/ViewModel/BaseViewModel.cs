using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WaterTracker.ViewModel
{
    public class BaseViewModel
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            if (Application.Current?.Dispatcher?.CheckAccess() == true)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            else
                Application.Current?.Dispatcher?.Invoke(() =>
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)));
        }
    }
}
