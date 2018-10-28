using System;
using System.IO;

namespace FileSearch.Domain
{
    public class StackBasedIteration
    {
        public event EventHandler<FilesEventArgs> FilesFinded;

        public event EventHandler SearcFinished;

        public void WalkDirectoryTree(DirectoryInfo root, string searchMask)
        {
            DirectoryInfo[] subDirs = null;
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
                subDirs = root.GetDirectories();
                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    WalkDirectoryTree(dirInfo, searchMask);
                }
            }
            catch (UnauthorizedAccessException) { }
        }

        public void WalkDriveTree(DriveInfo drive, string searchMask)
        {
            WalkDirectoryTree(drive.RootDirectory, searchMask);
            SearcFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}