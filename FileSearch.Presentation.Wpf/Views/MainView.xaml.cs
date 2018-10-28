using FileSearch.Presentation.Wpf.ViewModels;
using System.Windows;

namespace FileSearch.Presentation.Wpf
{
    public partial class MainView : Window
    {
        public MainView(MainViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}