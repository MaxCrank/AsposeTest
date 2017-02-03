using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FormatConversion.Base
{
    /// <summary>
    /// Base format processor class with general functionality. Intended for using by all format processor implementations.
    /// </summary>
    public abstract class FormatProcessorBase : IFormatProcessor
    {
        private static readonly Regex formatRegex = new Regex(FormatConversionSettings.FormatOnlyRegex);

        protected static Dictionary<string, Queue<IFormatProcessor>> formatProcessorsCache = new Dictionary<string, Queue<IFormatProcessor>>();

        private ObservableCollection<IFormatDataItem> _dataCollection = new ObservableCollection<IFormatDataItem>();
        private ReadOnlyObservableCollection<IFormatDataItem> _readonlyDataCollection;

        private List<NotifyCollectionChangedEventHandler> _collectionChangedDelegates = new List<NotifyCollectionChangedEventHandler>();

        private readonly string _preparedFormat;

        /// <remarks>
        /// Implements ReadOnlyObservableCollection anti-pattern
        /// </remarks>
        public ReadOnlyObservableCollection<IFormatDataItem> Data
        {
            get
            {
                if (_readonlyDataCollection == null)
                    _readonlyDataCollection = new ReadOnlyObservableCollection<IFormatDataItem>(_dataCollection);
                return _readonlyDataCollection;
            }
        }

        public abstract string Format { get; }

        public FormatProcessorBase()
        {
            _preparedFormat = PrepareFormatString(Format);
        }

        public bool AddDataCollectionChangedHandler(NotifyCollectionChangedEventHandler eventHandler)
        {
            bool result = false;
            if (!_collectionChangedDelegates.Contains(eventHandler))
            {
                _dataCollection.CollectionChanged += eventHandler;
                _collectionChangedDelegates.Add(eventHandler);
                result = true;
            }
            return result;
        }

        public bool RemoveDataCollectionChangedHandler(NotifyCollectionChangedEventHandler eventHandler)
        {
            bool result = false;
            if (_collectionChangedDelegates.Contains(eventHandler))
            {
                _dataCollection.CollectionChanged -= eventHandler;
                _collectionChangedDelegates.Remove(eventHandler);
                result = true;
            }
            return result;
        }

        public bool AddDataItem(IFormatDataItem dataItem, bool cloneInputDataItem = false)
        {
            bool result = false;
            if (dataItem == null)
            {
                Console.WriteLine("Can't add null-valued data item");
            }
            else if (Data.Contains(dataItem))
            {
                Console.WriteLine("You try to add an item that's already in the Data collection. Please, use Clone method to make a copy if that was your intention.");
            }
            else
            {
                _dataCollection.Add(cloneInputDataItem ? (dataItem.Clone() as IFormatDataItem) : dataItem);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Removes all data items
        /// </summary>
        public void ClearData()
        {
            foreach (var dataItem in _dataCollection)
            {
                dataItem.Dispose();
            }
            _dataCollection.Clear();
        }

        /// <summary>
        /// Get ready-to-work processor instance of the same format
        /// </summary>
        /// <returns>New instance if there are no free processors, otherwise reuses the one that was disposed and now is free</returns>
        public IFormatProcessor GetFormatProcessor()
        {
            if (formatProcessorsCache.ContainsKey(_preparedFormat) && formatProcessorsCache[_preparedFormat].Count > 0)
            {
                return formatProcessorsCache[_preparedFormat].Dequeue();
            }
            else
            {
                return Activator.CreateInstance(GetType()) as IFormatProcessor;
            }
        }

        /// <summary>
        /// Tries to read file at specified path, parse and fill data
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>If file was successfuly parsed</returns>
        public bool ReadFromFile(string filePath)
        {
            bool result = false;
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} does not exist");
            }
            else
            {
                try
                {
                    result = ParseBytes(File.ReadAllBytes(filePath));
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return result;
        }

        /// <summary>
        /// Parse complete formatted structure bytes representation
        /// </summary>
        /// <param name="allBytes"></param>
        /// <returns></returns>
        protected abstract bool ParseBytes(byte[] allBytes);

        public bool RemoveDataItem(IFormatDataItem dataItem)
        {
            bool result = false;
            if (dataItem == null)
            {
                Console.WriteLine("Can't remove null-valued data item");
            }
            else if (!Data.Contains(dataItem))
            {
                Console.WriteLine("Can't remove data item that's not in the collection");
            }
            else
            {
                _dataCollection.Remove(dataItem);
                result = true;
            }
            return result;
        }

        public void SetData(IEnumerable<IFormatDataItem> initialData, bool cloneInitialDataItems = true)
        {
            ClearData();
            foreach(var dataItem in initialData)
            {
                AddDataItem(dataItem, cloneInitialDataItems);
            }
        }

        private string PrepareFormatString(string format)
        {
            return format?.Replace(".", "").ToLowerInvariant();
        }

        /// <summary>
        /// Checks if this processor supports specified format
        /// </summary>
        /// <param name="format"></param>
        /// <returns>If this processor supports specified format</returns>
        public bool SupportsFormat(string format)
        {
            return !string.IsNullOrEmpty(format) && formatRegex.IsMatch(format) && PrepareFormatString(format) == _preparedFormat;
        }

        /// <returns>Complete formatted structure bytes representation</returns>
        public abstract byte[] GetData();

        /// <summary>
        /// Tries to write formatted file with complete formatted structure bytes representation at a specified path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>If file was successfully filled with formatted data</returns>
        public bool SaveToFile(string filePath)
        {
             return SaveToFile(filePath, true, true);
        }

        /// <summary>
        /// Tries to write formatted file with data items representation at a specified path
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="replace">If existing file is to be replaced</param>
        /// <param name="saveBackup">If existing file is to be saved with backup extension before replacement</param>
        /// <returnsIf file was successfully filled with formatted data></returns>
        public bool SaveToFile(string filePath, bool replace, bool saveBackup)
        {
            bool result = false;
            string tempFilePath = filePath + FormatConversionSettings.TempFileExtension;
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
            try
            {
                using (var fileStream = File.Open(tempFilePath, FileMode.Create))
                {
                    if (!fileStream.CanWrite)
                    {
                        Console.WriteLine("FileStream can't write");
                        File.Delete(tempFilePath);
                        fileStream.Close();
                    }
                    else
                    {
                        using (var binaryWriter = new BinaryWriter(fileStream))
                        {
                            var data = GetData();
                            if (data != null)
                            {
                                binaryWriter.Write(data);
                                binaryWriter.Flush();
                                binaryWriter.Close();
                                fileStream.Close();
                                result = true;
                            }
                            else
                            {
                                Console.WriteLine("Can't write null-valued data");
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Data items collection indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IFormatDataItem this[int index]
        {
            get
            {
                if (index < 0 || index >= _readonlyDataCollection.Count)
                {
                    throw new IndexOutOfRangeException("Data index " + index + " is out of range. Items count: " + _readonlyDataCollection.Count);
                }
                else
                {
                    return _readonlyDataCollection[index];
                }
            }
        }

        public void Dispose()
        {
            _collectionChangedDelegates.ForEach(d => _dataCollection.CollectionChanged -= d);
            _collectionChangedDelegates.Clear();
            ClearData();
            if (!formatProcessorsCache.ContainsKey(Format))
            {
                formatProcessorsCache.Add(_preparedFormat, new Queue<IFormatProcessor>());
            }
            formatProcessorsCache[_preparedFormat].Enqueue(this);
        }
    }
}
