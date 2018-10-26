using System;
using System.IO;
using System.Threading;

namespace FileSearch.Domain
{
    public class StackBasedIteration
    {
        public void WalkDriveTree(DriveInfo drive, string searchMask)
        {
            WalkDirectoryTree(drive.RootDirectory, searchMask);
        }

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

        public event EventHandler<FilesEventArgs> FilesFinded;
    }
}