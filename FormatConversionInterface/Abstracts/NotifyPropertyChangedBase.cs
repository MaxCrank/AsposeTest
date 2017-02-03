using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FormatConversion.Base
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged, IDisposable
    {
        private PropertyChangedEventHandler _propertyChanged;
        private List<PropertyChangedEventHandler> _propertyChangedDelegates = new List<PropertyChangedEventHandler>();

        public bool AddPropertyChangedHandler(PropertyChangedEventHandler eventHandler)
        {
            bool result = false;
            if (!_propertyChangedDelegates.Contains(eventHandler))
            {
                _propertyChanged += eventHandler;
                _propertyChangedDelegates.Add(eventHandler);
                result = true;
            }
            return result;
        }

        public bool RemovePropertyChangedHandler(PropertyChangedEventHandler eventHandler)
        {
            bool result = false;
            if (_propertyChangedDelegates.Contains(eventHandler))
            {
                _propertyChanged -= eventHandler;
                _propertyChangedDelegates.Remove(eventHandler);
                result = true;
            }
            return result;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                AddPropertyChangedHandler(value);
            }
            remove
            {
                RemovePropertyChangedHandler(value);
            }
        }

        public void Dispose()
        {
            _propertyChangedDelegates.ForEach(d => _propertyChanged -= d);
            _propertyChangedDelegates.Clear();
        }
    }
}