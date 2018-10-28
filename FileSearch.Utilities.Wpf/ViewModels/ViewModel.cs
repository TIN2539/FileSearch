using FileSearch.Utilities.Wpf.Attributes;
using FileSearch.Utilities.Wpf.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FileSearch.Utilities.Wpf
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        private readonly Type currentType;
        private readonly PropertyInfo[] properties;

        public ViewModel()
        {
            currentType = GetType();
            properties = currentType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                HandlePropertiesAttributes(property);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected void SetProperty<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!oldValue?.Equals(newValue) ?? newValue != null)
            {
                oldValue = newValue;

                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }

        private IEnumerable<PropertyInfo> FindNotiFyCollectionChangedImplementProperty()
        {
            ICollection<PropertyInfo> result = new List<PropertyInfo>();

            foreach (PropertyInfo property in properties)
            {
                if (property.GetValue(this) is INotifyCollectionChanged)
                {
                    result.Add(property);
                }
            }

            return result;
        }

        private void HandleDependsUpponCollectionAttribute(PropertyInfo appliedProperty)
        {
            IEnumerable<DependsUponCollectionAttribute> attributes = appliedProperty.GetCustomAttributes<DependsUponCollectionAttribute>();
            foreach (DependsUponCollectionAttribute attribute in attributes)
            {
                IEnumerable<PropertyInfo> notifyCollections = FindNotiFyCollectionChangedImplementProperty();
                foreach (PropertyInfo notifyCollection in notifyCollections)
                {
                    if (notifyCollection.Name == attribute.CollectionName)
                    {
                        ((INotifyCollectionChanged)notifyCollection.GetValue(this)).CollectionChanged += (sender, e) =>
                        {
                            OnPropertyChanged(new PropertyChangedEventArgs(appliedProperty.Name));
                        };
                    }
                }
            }
        }

        private void HandleDependsUpponProperties(PropertyInfo appliedProperty)
        {
            IEnumerable<DependsUponPropertyAttribute> attributes = appliedProperty.GetCustomAttributes<DependsUponPropertyAttribute>();
            foreach (DependsUponPropertyAttribute attribute in attributes)
            {
                PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == attribute.PropertyName)
                    {
                        OnPropertyChanged(new PropertyChangedEventArgs(appliedProperty.Name));
                    }
                };
            }
        }

        private void HandlePropertiesAttributes(PropertyInfo appliedProperty)
        {
            HandleDependsUpponProperties(appliedProperty);
            HandleRaiseCanExecuteDependsUpponProperties(appliedProperty);
            HandleDependsUpponCollectionAttribute(appliedProperty);
        }

        private void HandleRaiseCanExecuteDependsUpponProperties(PropertyInfo appliedProperty)
        {
            RaiseCanExecuteDependsUponAttribute attribute = appliedProperty.GetCustomAttribute<RaiseCanExecuteDependsUponAttribute>();
            if (attribute != null)
            {
                PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == attribute.PropertyName)
                    {
                        Command command = (Command)appliedProperty.GetValue(this);
                        command.RaiseCanExecuteChanged();
                    }
                };
            }
        }
    }
}