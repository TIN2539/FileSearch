using System;
using System.Collections.Generic;
using System.IO;

namespace FileSearch.Domain
{
    public sealed class FilesEventArgs : EventArgs
    {
        private readonly IEnumerable<FileInfo> files;

        public FilesEventArgs(IEnumerable<FileInfo> files)
        {
            this.files = files;
        }

        public IEnumerable<FileInfo> Files => files;
    }
}