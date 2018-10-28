using FileSearch.Domain;
using FileSearch.Utilities.Wpf;
using FileSearch.Utilities.Wpf.Attributes;
using FileSearch.Utilities.Wpf.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace FileSearch.Presentation.Wpf.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private readonly IEnumerable<DriveInfo> drives;
        private readonly ICollection<FileListViewModel> foundFiles = new ObservableCollection<FileListViewModel>();
        private readonly Command pauseCommand;
        private readonly Command resumeCommand;
        private readonly Command searchCommand;
        private readonly StackBasedIteration stackBasedIteration = new StackBasedIteration();
        private readonly Command stopCommand;
        private readonly ISynchronizationContext synchronizationContext;
        private bool canPause;
        private bool canResume;
        private bool canStop;
        private string searchMask = string.Empty;
        private DriveInfo selectedDrive;
        private Thread thread;

        public MainViewModel(ISynchronizationContext synchronizationContext)
        {
            drives = DriveInfo.GetDrives();
            selectedDrive = Enumerable.ElementAt(drives, 0);
            searchCommand = new DelegateCommand(Search, () => CanSearch);
            pauseCommand = new DelegateCommand(Pause, () => CanPause);
            resumeCommand = new DelegateCommand(Resume, () => CanResume);
            stopCommand = new DelegateCommand(Stop, () => CanStop);
            this.synchronizationContext = synchronizationContext;
            stackBasedIteration.FilesFound += (sender, e) => TryAddFoundFiles(e);
            stackBasedIteration.SearchFinished += (sender, e) => ChangeButtonsStateBecauseSearchFinished(synchronizationContext);
            SearchStarted += (sender, e) => { ChangeButtonsStateBecauseSearchStarted(); };
            SearchPaused += (sender, e) => { ChangeButtonsStateBecauseSearchPaused(); };
            ResumeSearch += (sender, e) => { ChangeButtonsStateBecauseResumeSearch(); };
            SearchStoped += (sender, e) => { ChangeButonsStateBecauseSearchStoped(); };
        }

        public bool CanPause
        {
            get => canPause;
            set => SetProperty(ref canPause, value);
        }

        public bool CanResume
        {
            get => canResume;
            set => SetProperty(ref canResume, value);
        }

        [DependsUponProperty(nameof(SearchMask))]
        [DependsUponProperty(nameof(CanStop))]
        public bool CanSearch => searchMask != string.Empty && !CanStop;

        public bool CanStop
        {
            get => canStop;
            set => SetProperty(ref canStop, value);
        }

        public IEnumerable<DriveInfo> Drives => drives;

        public ICollection<FileListViewModel> FoundFiles => foundFiles;

        [RaiseCanExecuteDependsUpon(nameof(CanPause))]
        public Command PauseCommand => pauseCommand;

        [RaiseCanExecuteDependsUpon(nameof(CanResume))]
        public Command ResumeCommand => resumeCommand;

        [RaiseCanExecuteDependsUpon(nameof(CanSearch))]
        public Command SearchCommand => searchCommand;

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

        [RaiseCanExecuteDependsUpon(nameof(CanStop))]
        public Command StopCommand => stopCommand;

        public event EventHandler ResumeSearch;
        public event EventHandler SearchPaused;
        public event EventHandler SearchStarted;
        public event EventHandler SearchStoped;

        private void ChangeButonsStateBecauseSearchStoped()
        {
            CanStop = false;
            CanResume = false;
            CanPause = false;
        }

        private void ChangeButtonsStateBecauseResumeSearch()
        {
            CanResume = false;
            CanPause = true;
            CanStop = true;
        }

        private void ChangeButtonsStateBecauseSearchFinished(ISynchronizationContext synchronizationContext)
        {
            synchronizationContext.Invoke(() =>
            {
                CanStop = false;
                CanResume = false;
                CanPause = false;
            }
            );
        }

        private void ChangeButtonsStateBecauseSearchPaused()
        {
            CanPause = false;
            CanResume = true;
            CanStop = true;
        }

        private void ChangeButtonsStateBecauseSearchStarted()
        {
            CanPause = true;
            CanStop = true;
            CanResume = false;
        }

        private void Pause()
        {
            thread.Suspend();
            SearchPaused?.Invoke(this, EventArgs.Empty);
        }

        private void Resume()
        {
            thread.Resume();
            ResumeSearch?.Invoke(this, EventArgs.Empty);
        }

        private void Search()
        {
            foundFiles.Clear();
            thread = new Thread(() => stackBasedIteration.WalkDriveTree(selectedDrive, searchMask)) { IsBackground = true };
            thread.Start();
            SearchStarted?.Invoke(this, EventArgs.Empty);
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
            thread.Abort();
            SearchStoped?.Invoke(this, EventArgs.Empty);
        }

        private void TryAddFoundFiles(FilesEventArgs e)
        {
            synchronizationContext.Invoke(() =>
            {
                foreach (FileInfo file in e.Files)
                {
                    foundFiles.Add(new FileListViewModel(file));
                }
            });
        }
    }
}