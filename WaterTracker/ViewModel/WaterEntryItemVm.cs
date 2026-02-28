using static WaterTracker.WaterRepository;

namespace WaterTracker.ViewModel
{
    public class WaterEntryItemVm : BaseViewModel
    {
        private WaterEntry _entry;
        public WaterEntryItemVm(WaterEntry entry)
        {
            _entry = entry;
        }
        public string TimeText => _entry.Timestamp.ToString("HH:mm");
        public string AmountText => $"{_entry.AmountMl} ml";

        public WaterEntry Entry
        {
            get { return _entry; }
            set
            {
                _entry = value;
                OnPropertyChanged();
            }
        }
    }
}
