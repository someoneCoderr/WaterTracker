using WaterTracker.ViewModel;
using System.ComponentModel;
using System.Windows;

namespace WaterTracker.Views
{
    public partial class HomeView : Window
    {
        public HomeView()
        {
            HomeViewModel vm = new HomeViewModel();
            this.DataContext = vm;
            //WaterRepository wr = new WaterRepository();
            //wr.InitializeAsync();
            InitializeComponent();
        }
    }
}