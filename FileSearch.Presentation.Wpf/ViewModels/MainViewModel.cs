using FileSearch.Domain;
using FileSearch.Utilities.Wpf;
using FileSearch.Utilities.Wpf.Attributes;
using FileSearch.Utilities.Wpf.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace FileSearch.Presentation.Wpf.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private Thread thread;
        private readonly ISynchronizationContext synchronizationContext;
        private readonly StackBasedIteration stackBasedIteration;
        private IEnumerable<DriveInfo> drives;
        private string searchMask = string.Empty;
        private DriveInfo selectedDrive;
        private readonly Command searchCommand;
        private readonly Command pauseCommand;
        private readonly Command resumeCommand;
        private readonly Command stopCommand;
        private ICollection<FileListViewModel> findedFiles = new ObservableCollection<FileListViewModel>();

        public MainViewModel(ISynchronizationContext synchronizationContext)
        {
            drives = DriveInfo.GetDrives();
            stackBasedIteration = new StackBasedIteration();
            searchCommand = new DelegateCommand(Search, () => CanSearch);
            pauseCommand = new DelegateCommand(Pause, () => CanPause);
            resumeCommand = new DelegateCommand(Resume);
            stopCommand = new DelegateCommand(Stop);
            stackBasedIteration.FilesFinded += (sender, e) => TryAddFindedFiles(e);
            this.synchronizationContext = synchronizationContext;
        }

        private void TryAddFindedFiles(FilesEventArgs e)
        {
            synchronizationContext.Invoke(() =>
            {
                foreach (FileInfo file in e.Files)
                {
                    findedFiles.Add(new FileListViewModel(file));
                }
            });
        }

        private void Stop()
        {
            try
            {
                thread.Abort();
            }
            catch (ThreadStateException)
            {
                thread.Resume();
            }
        }

        private void Resume()
        {
            thread.Resume();
        }

        private void Pause()
        {
            thread.Suspend();
        }

        private void Search()
        {
            findedFiles.Clear();
            thread = new Thread(() => stackBasedIteration.WalkDriveTree(selectedDrive, searchMask)) { IsBackground = true };
            thread.Start();
        }

        public IEnumerable<DriveInfo> Drives
        {
            get => drives;
            set => SetProperty(ref drives, value);
        }

        public string SearchMask
        {
            get => searchMask;
            set => SetProperty(ref searchMask, value);
        }

        public DriveInfo SelectedDrive
        {
            get => selectedDrive;
            set => SetProperty(ref selectedDrive, value);
        }

        [RaiseCanExecuteDependsUpon(nameof(CanSearch))]
        public Command SearchCommand => searchCommand;

        [RaiseCanExecuteDependsUpon(nameof(CanPause))]
        public Command PauseCommand => pauseCommand;

        public Command ResumeCommand => resumeCommand;

        public Command StopCommand => stopCommand;

        [DependsUponProperty(nameof(SearchMask))]
        [DependsUponProperty(nameof(SelectedDrive))]        
        public bool CanSearch => searchMask != string.Empty && selectedDrive != null;

        public ICollection<FileListViewModel> FindedFiles
        {
            get => findedFiles;
            set => SetProperty(ref findedFiles, value);
        }

        [RaiseCanExecuteDependsUpon(nameof(searchCommand))]
        public bool CanPause => searchCommand.CanExecute();
    }
}