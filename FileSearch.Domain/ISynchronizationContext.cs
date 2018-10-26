using System;

namespace FileSearch.Domain
{
    public interface ISynchronizationContext
    {
        void Invoke(Action action);
    }
}