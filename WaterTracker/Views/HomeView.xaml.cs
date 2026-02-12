using AppRunner.ViewModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            HomeViewModel vm = new HomeViewModel();
            this.DataContext = vm;
            InitializeComponent();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            // Hide instead of closing
            e.Cancel = true;
            Hide();
        }

    }
}