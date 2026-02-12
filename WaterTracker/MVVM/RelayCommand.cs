using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AppRunner.MVVM
{
    public class RelayCommand : ICommand
    {
        private readonly Func<object?, Task>? _asyncExecute;
        private readonly Action<object?>? _execute;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action<object?> execute) => _execute = execute;
        public RelayCommand(Func<object?, Task> asyncExecute) => _asyncExecute = asyncExecute;

        public bool CanExecute(object? parameter) => true;

        public async void Execute(object? parameter)
        {
            if (_asyncExecute != null) await _asyncExecute(parameter);
            else _execute?.Invoke(parameter);
        }
    }
}
