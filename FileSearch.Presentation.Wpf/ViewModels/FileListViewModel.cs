using System.IO;

namespace FileSearch.Presentation.Wpf.ViewModels
{
    public class FileListViewModel
    {
        private readonly string name;
        private readonly string path;

        public FileListViewModel(FileInfo file)
        {
            name = file.Name;
            path = file.DirectoryName;
        }

        public string Name => name;
        public string Path => path;
    }
}