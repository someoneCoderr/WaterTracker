using AppRunner.MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppRunner.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        private int _drankWater;

        public HomeViewModel()
        {

        }

        public int DrankWater
        {
            get { return _drankWater; }
            set
            {
                _drankWater = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddWaterCommand { get; }

        //public void AddWater()
        //{
        //    DrankWater += 
        //}
    }
}
