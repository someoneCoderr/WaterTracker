using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterTracker.ViewModel
{
    public class EditDialogViewModel : BaseViewModel
    {
        WaterEntryItemVm _waterEntry;
        public EditDialogViewModel(WaterEntryItemVm waterEntry)
        {
            WaterEntryProp = waterEntry;
        }

        public WaterEntryItemVm WaterEntryProp
        {
            get { return _waterEntry; }
            set { _waterEntry = value; OnPropertyChanged(); }
        }
    }
}
