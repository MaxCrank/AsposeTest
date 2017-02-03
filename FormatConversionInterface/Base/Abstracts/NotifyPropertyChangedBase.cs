using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AsposeFormatConverter.Base
{
    internal abstract class NotifyPropertyChangedBase : INotifyPropertyChanged, IDisposable
    {
        private PropertyChangedEventHandler _propertyChanged;
        private List<PropertyChangedEventHandler> _propertyChangedDelegates = new List<PropertyChangedEventHandler>();

        public void AddPropertyChangedHandler(PropertyChangedEventHandler eventHandler)
        {
            Debug.Assert(eventHandler != null, "EventHandler is null");
            Debug.Assert(!_propertyChangedDelegates.Contains(eventHandler), "EventHandler is already added");
            _propertyChanged += eventHandler;
            _propertyChangedDelegates.Add(eventHandler);
        }

        public void RemovePropertyChangedHandler(PropertyChangedEventHandler eventHandler)
        {
            Debug.Assert(eventHandler != null, "EventHandler is null");
            Debug.Assert(_propertyChangedDelegates.Contains(eventHandler), "EventHandler was not added, therefore can't be removed");
            _propertyChanged -= eventHandler;
            _propertyChangedDelegates.Remove(eventHandler);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "Propery name is null or empty, can't raise an event");
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