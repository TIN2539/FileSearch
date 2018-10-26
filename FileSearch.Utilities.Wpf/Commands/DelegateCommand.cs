using System;

namespace FileSearch.Utilities.Wpf.Commands
{
    public sealed class DelegateCommand : Command
    {
        private readonly Func<bool> canExecuteMethod;
        private readonly Action executeMethod;

        public DelegateCommand(Action executeMethod) :
            this(executeMethod, () => true)
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            this.canExecuteMethod = canExecuteMethod;
            this.executeMethod = executeMethod;
        }

        public override bool CanExecute() => canExecuteMethod();

        public override void Execute() => executeMethod();
    }
}