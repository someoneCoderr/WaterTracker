using WaterTracker.ViewModel;
using System.ComponentModel;
using System.Windows;

namespace WaterTracker.Views
{
    public partial class HomeView : Window
    {
        public HomeView()
        {
            InitializeComponent();
            DataContext = new HomeViewModel();
        }
    }
}