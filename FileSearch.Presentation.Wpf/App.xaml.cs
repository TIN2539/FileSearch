using Ninject;
using Ninject.Extensions.Conventions;
using System.Windows;

namespace FileSearch.Presentation.Wpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var container = CreateContainer();
            MainView mainView = container.Get<MainView>();

            mainView.Show();
        }

        private StandardKernel CreateContainer()
        {
            var container = new StandardKernel();

            container.Bind(configurator => configurator
                .From("FileSearch.Presentation.Wpf", "FileSearch.Domain")
                .SelectAllClasses()
                .BindAllInterfaces()
            );

            return container;
        }
    }
}