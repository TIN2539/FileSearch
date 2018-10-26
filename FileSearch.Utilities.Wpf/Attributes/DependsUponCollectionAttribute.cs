using System;

namespace FileSearch.Utilities.Wpf.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class DependsUponCollectionAttribute : Attribute
    {
        private readonly string collectionName;

        public DependsUponCollectionAttribute(string collectionName)
        {
            this.collectionName = collectionName;
        }

        public string CollectionName => collectionName;
    }
}