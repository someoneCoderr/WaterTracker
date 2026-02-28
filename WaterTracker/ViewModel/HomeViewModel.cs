using AppRunner.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WaterTracker.ViewModel;
using WaterTracker.Views;

namespace WaterTracker.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly WaterRepository _repo;

        public ObservableCollection<WaterEntryItemVm> Entries { get; } = new();

        public WaterEntryItemVm SelectedWaterEntry 
        {
            get { return _selectedWaterEntry; } 
            set 
            { 
                _selectedWaterEntry = value; 
                OnPropertyChanged(nameof(SelectedWaterEntry));
            }

        }

        private int _dailyGoalMl = 2500;
        public int DailyGoalMl
        {
            get => _dailyGoalMl;
            set
            {
                _dailyGoalMl = value;
                OnPropertyChanged();
                RecalcProgress();
            }
        }

        private int _todayTotalMl;
        public int TodayTotalMl
        {
            get => _todayTotalMl;
            set
            {
                _todayTotalMl = value;
                OnPropertyChanged();
                RecalcProgress();
            }
        }

        private double _progressPercent;
        public double ProgressPercent
        {
            get => _progressPercent;
            private set
            {
                _progressPercent = value;
                OnPropertyChanged(nameof(ProgressPercent));
                OnPropertyChanged(nameof(ProgressText));
            }
        }


        public string ProgressText => $"{ProgressPercent:0}% erreicht";

        private string _customAmountText = "";
        public string CustomAmountText
        {
            get => _customAmountText;
            set { _customAmountText = value; OnPropertyChanged(nameof(CustomAmountText)); }
        }

        private string _errorText = "";
        public string ErrorText
        {
            get => _errorText;
            private set { _errorText = value; OnPropertyChanged(nameof(ErrorText)); }
        }

        private DateTime _selectedDate = DateTime.Today;
        private WaterEntryItemVm _selectedWaterEntry;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set { _selectedDate = value.Date; OnPropertyChanged(); OnPropertyChanged(nameof(SelectedDateLabel)); _ = LoadEntriesForDateAsync(_selectedDate); }
        }

        public string SelectedDateLabel => $"({SelectedDate:dd.MM.yyyy})";

        public ICommand AddPresetCommand { get; }
        public ICommand AddCustomCommand { get; }
        public ICommand OpenGoalCommand { get; }
        public ICommand EditEntryCommand { get; }

        public HomeViewModel()
        {
            _repo = new WaterRepository();

            AddPresetCommand = new RelayCommand(async p => await AddPresetAsync(p));
            AddCustomCommand = new RelayCommand(async _ => await AddCustomAsync());
            OpenGoalCommand = new RelayCommand(_ => ChangeGoalSimple());
            EditEntryCommand = new RelayCommand(_ => EditEntry());      

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await _repo.InitializeAsync();

            DailyGoalMl = await _repo.GetDailyGoalAsync() ?? 2500;

            await LoadTodayAsync();
            await LoadEntriesForDateAsync(DateTime.Today);
        }

        private async Task AddPresetAsync(object? parameter)
        {
            if (parameter is null) return;
            if (!int.TryParse(parameter.ToString(), out int ml)) return;

            await AddAmountAsync(ml);
        }

        private async Task AddCustomAsync()
        {
            ErrorText = "";

            if (!int.TryParse(CustomAmountText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int ml))
            {
                ErrorText = "Bitte eine Zahl in ml eingeben.";
                return;
            }

            if (ml <= 0 || ml > 5000)
            {
                ErrorText = "Menge muss zwischen 1 und 5000 ml liegen.";
                return;
            }

            CustomAmountText = "";
            await AddAmountAsync(ml);
        }

        private async Task AddAmountAsync(int ml)
        {
            await _repo.AddEntryAsync(DateTime.Now, ml);

            await LoadTodayAsync();
            await LoadEntriesForDateAsync(SelectedDate);
        }

        private async Task LoadTodayAsync()
        {
            TodayTotalMl = await _repo.GetTotalForDateAsync(DateTime.Today);
        }

        private async Task LoadEntriesForDateAsync(DateTime date)
        {
            var entries = await _repo.GetEntriesForDateAsync(date);

            Entries.Clear();
            foreach (var e in entries.OrderByDescending(x => x.Timestamp))
                Entries.Add(new WaterEntryItemVm(e));
        }

        private void RecalcProgress()
        {
            if (DailyGoalMl <= 0) { ProgressPercent = 0; return; }
            ProgressPercent = Math.Min(100, (double)TodayTotalMl / DailyGoalMl * 100.0);
        }

        private void ChangeGoalSimple()
        {
            DailyGoalMl = (DailyGoalMl == 2500) ? 3000 : 2500;
            _ = _repo.SetDailyGoalAsync(DailyGoalMl);
        }

        public void EditEntry()
        {
            EditDialogViewModel vm = new EditDialogViewModel(SelectedWaterEntry, _repo);
            EditDialog editDialog = new EditDialog(vm);
            vm.closeDialogEvent += (s, e) =>
            {
                editDialog.Close();
                _ = LoadTodayAsync();
                _ = LoadEntriesForDateAsync(SelectedDate);
            };
            // ShowDialog() stops the code here, so event subscribing must be done before (error like in Metrohm)
            editDialog.ShowDialog();
        }
    }

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
