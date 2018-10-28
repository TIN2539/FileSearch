using System;
using System.IO;

namespace FileSearch.Domain
{
    public class StackBasedIteration
    {
        public event EventHandler<FilesEventArgs> FilesFinded;
        public event EventHandler SearchFinished;

        public void WalkDirectoryTree(DirectoryInfo root, string searchMask)
        {
            DirectoryInfo[] childDirectories = null;
            FileInfo[] files = null;
            try
            {
                files = root.GetFiles(searchMask);
                if (files.Length > 0)
                {
                    FilesFinded?.Invoke(this, new FilesEventArgs(files));
                }
            }
            catch (UnauthorizedAccessException) { }

            try
            {
                childDirectories = root.GetDirectories();
                foreach (System.IO.DirectoryInfo directoryInfo in childDirectories)
                {
                    WalkDirectoryTree(directoryInfo, searchMask);
                }
            }
            catch (UnauthorizedAccessException) { }
        }

        public void WalkDriveTree(DriveInfo drive, string searchMask)
        {
            WalkDirectoryTree(drive.RootDirectory, searchMask);
            SearchFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}