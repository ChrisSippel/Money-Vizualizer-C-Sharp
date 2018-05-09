using System;
using System.Windows.Input;

namespace MoneyVisualizer.Helpers.Ui
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _executeAction;
        private readonly Predicate<object> _canExecute;

        private event EventHandler CanExecuteChangedInternal;

        public RelayCommand(Action<object> execute)
            : this(execute, DefaultCanExecute)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }

            _executeAction = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                CanExecuteChangedInternal += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
                CanExecuteChangedInternal -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute != null && _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }

        public void OnCanExecuteChanged()
        {
            CanExecuteChangedInternal?.Invoke(this, EventArgs.Empty);
        }

        private static bool DefaultCanExecute(object parameter)
        {
            return true;
        }
    }
}
