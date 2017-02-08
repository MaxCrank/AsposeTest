﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AsposeFormatConverter.Tests")]
namespace AsposeFormatConverter.Base
{
    internal abstract class NotifyPropertyChangedBase : INotifyPropertyChanged, IDisposable
    {
        private PropertyChangedEventHandler _propertyChanged;
        private List<PropertyChangedEventHandler> _propertyChangedDelegates = new List<PropertyChangedEventHandler>();

        public void AddPropertyChangedHandler(PropertyChangedEventHandler eventHandler)
        {
            if (eventHandler == null)
            {
                throw new ArgumentNullException("Passed event handler is null");
            }
            if (_propertyChangedDelegates.Contains(eventHandler))
            {
                throw new InvalidOperationException("Passed event handler was already added");
            }
            _propertyChanged += eventHandler;
            _propertyChangedDelegates.Add(eventHandler);
        }

        public void RemovePropertyChangedHandler(PropertyChangedEventHandler eventHandler)
        {
            if (eventHandler == null)
            {
                throw new ArgumentNullException("Passed event handler is null");
            }
            if (!_propertyChangedDelegates.Contains(eventHandler))
            {
                throw new InvalidOperationException("Passed event handler was not added");
            }
            _propertyChanged -= eventHandler;
            _propertyChangedDelegates.Remove(eventHandler);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {

            Debug.Assert(!string.IsNullOrEmpty(propertyName), "Propery name is null or empty, can't raise an event");
            if (_propertyChanged != null) _propertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
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