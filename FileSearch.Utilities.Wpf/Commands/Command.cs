using System;
using System.Windows.Input;

namespace FileSearch.Utilities.Wpf.Commands
{
    public abstract class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute() => true;

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        public abstract void Execute();

        void ICommand.Execute(object parameter)
        {
            Execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}