using System;

namespace FileSearch.Utilities.Wpf.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class DependsUponPropertyAttribute : Attribute
    {
        private readonly string propertyName;

        public DependsUponPropertyAttribute(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public string PropertyName => propertyName;
    }
}