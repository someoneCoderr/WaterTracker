using System.Configuration;
using System.Data;
using System.Windows;

namespace WaterTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();
            base.OnStartup(e);
        }
    }

}
