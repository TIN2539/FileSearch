using FileSearch.Domain;
using System;
using System.Threading;
using System.Windows;

namespace FileSearch.Presentation.Wpf.ViewModels
{
    public class WpfSynchromizationContext : ISynchronizationContext
    {

        public void Invoke(Action action)
        {
            Application.Current?.Dispatcher.Invoke(action);
        }
    }
}