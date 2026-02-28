using AppRunner.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WaterTracker.ViewModel
{
    public class EditDialogViewModel : BaseViewModel
    {
        WaterEntryItemVm _waterEntry;
        WaterRepository _waterRepository;

        public EventHandler closeDialogEvent;
        public EditDialogViewModel(WaterEntryItemVm waterEntry, WaterRepository waterRepository)
        {
            WaterEntryProp = waterEntry;
            _waterRepository = waterRepository;
            UpdateEntryCommand = new RelayCommand(async _ => await UpdateEntry());
            CloseDialogCommand = new RelayCommand(_ => CloseDialog());
        }

        public WaterEntryItemVm WaterEntryProp
        {
            get { return _waterEntry; }
            set { _waterEntry = value; OnPropertyChanged(); }
        }

        public ICommand UpdateEntryCommand { get; set; }
        public ICommand CloseDialogCommand { get; set; }

        public async Task UpdateEntry()
        {
            await _waterRepository.UpadteEntryAsync(WaterEntryProp.Entry);
            closeDialogEvent?.Invoke(this, EventArgs.Empty);
        }

        public void CloseDialog()
        {
            closeDialogEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
