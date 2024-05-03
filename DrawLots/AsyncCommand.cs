using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DrawLots
{
    internal class AsyncCommand : ICommand
    {
        private Func<object, Task> _execute;
        private Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public AsyncCommand(Func<object, Task> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute.Invoke(parameter);
        }

        public async void Execute(object parameter)
        {
            await _execute(parameter);
        }
    }
}
