using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AsposeFormatConverter.Base
{
    /// <summary>
    /// Base format processor class with general functionality. Intended for using by all format processor implementations.
    /// </summary>
    internal abstract class FormatProcessorBase : IFormatProcessor
    {
        private static readonly Regex FormatRegex = new Regex(FormatConversionSettings.FormatOnlyRegex);

        protected static Dictionary<ConvertedFormat, Queue<IFormatProcessor>> FormatProcessorsCache = new Dictionary<ConvertedFormat, Queue<IFormatProcessor>>();

        private ObservableCollection<IFormatDataItem> _dataCollection = new ObservableCollection<IFormatDataItem>();
        private ReadOnlyObservableCollection<IFormatDataItem> _readonlyDataCollection;

        private List<NotifyCollectionChangedEventHandler> _collectionChangedDelegates = new List<NotifyCollectionChangedEventHandler>();

        private readonly string _stringFormat;

        /// <remarks>
        /// Implements ReadOnlyObservableCollection anti-pattern
        /// </remarks>
        public ReadOnlyObservableCollection<IFormatDataItem> Data => _readonlyDataCollection ??
                                                                     (_readonlyDataCollection = new ReadOnlyObservableCollection<IFormatDataItem>(_dataCollection));

        public abstract ConvertedFormat Format { get; }

        protected FormatProcessorBase()
        {
            Debug.Assert(Format != ConvertedFormat.UNKNOWN, "Processor's format should not be unknown");
            _stringFormat = Enum.GetName(typeof(ConvertedFormat), Format).ToLowerInvariant();
        }

        public void AddDataCollectionChangedHandler(NotifyCollectionChangedEventHandler eventHandler)
        {
            Debug.Assert(eventHandler != null, "Event handler is null");
            Debug.Assert(!_collectionChangedDelegates.Contains(eventHandler), "Event handler is already added");
            _dataCollection.CollectionChanged += eventHandler;
            _collectionChangedDelegates.Add(eventHandler);
        }

        public void RemoveDataCollectionChangedHandler(NotifyCollectionChangedEventHandler eventHandler)
        {
            Debug.Assert(eventHandler != null, "Event handler is null");
            Debug.Assert(_collectionChangedDelegates.Contains(eventHandler), "Event handler was not added, therefore can't be removed");
            _dataCollection.CollectionChanged -= eventHandler;
            _collectionChangedDelegates.Remove(eventHandler);
        }

        public void AddDataItem(IFormatDataItem dataItem, bool cloneInputDataItem)
        {
            Debug.Assert(dataItem != null, "Can't add null-valued data item");
            Debug.Assert(!Data.Contains(dataItem), "Item is already in Data collection. Please, use clone option to make a copy if that was your intention.");
            _dataCollection.Add(cloneInputDataItem ? (dataItem.Clone() as IFormatDataItem) : dataItem);
        }


        public void AddNewDataItem(int year, int month, int day, string brandName, int price)
        {
            var newItem = new FormatDataItem();
            newItem.SetDate(year, month, day);
            newItem.SetBrandName(brandName);
            newItem.SetPrice(price);
            _dataCollection.Add(newItem);
        }

        public void AddNewDataItem(DateTime date, string brandName, int price)
        {
            var newItem = new FormatDataItem();
            newItem.SetDate(date);
            newItem.SetBrandName(brandName);
            newItem.SetPrice(price);
            _dataCollection.Add(newItem);
        }

        public void AddNewDataItem(string date, string brandName, int price)
        {
            var newItem = new FormatDataItem();
            newItem.SetDate(date);
            newItem.SetBrandName(brandName);
            newItem.SetPrice(price);
            _dataCollection.Add(newItem);
        }

        /// <summary>
        /// Removes all data items
        /// </summary>
        public void ClearData()
        {
            Debug.Assert(_dataCollection != null);
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
            Debug.Assert(FormatProcessorsCache != null);
            if (FormatProcessorsCache.ContainsKey(Format) && FormatProcessorsCache[Format].Count > 0)
            {
                return FormatProcessorsCache[Format].Dequeue();
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
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new ArgumentException("Can't read from path leading to non-existing file");
            }
            return ParseBytes(File.ReadAllBytes(filePath));
        }

        /// <summary>
        /// Parse complete formatted structure bytes representation
        /// </summary>
        /// <param name="allBytes"></param>
        /// <returns></returns>
        protected abstract bool ParseBytes(IEnumerable<byte> allBytes);

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

        /// <summary>
        /// Sets the data.
        /// </summary>
        /// <param name="initialData">The initial data.</param>
        /// <param name="cloneInitialDataItems">if set to <c>true</c> [clone initial data items].</param>
        public void SetData(IEnumerable<IFormatDataItem> initialData, bool cloneInitialDataItems = true)
        {
            ClearData();
            if (initialData != null) foreach(var dataItem in initialData)
            {
                AddDataItem(dataItem, cloneInitialDataItems);
            }
        }

        private string PrepareFormatString(string format)
        {
            Debug.Assert(!string.IsNullOrEmpty(format), "Can't prepare empty or null format string");
            return format?.Replace(".", "").ToLowerInvariant();
        }

        /// <summary>
        /// Checks if this processor supports specified format
        /// </summary>
        /// <param name="format"></param>
        /// <returns>If this processor supports specified format</returns>
        public bool SupportsFormat(string format)
        {
            return !string.IsNullOrEmpty(format) && FormatRegex.IsMatch(format) && PrepareFormatString(format) == _stringFormat;
        }

        /// <returns>Complete formatted structure bytes representation</returns>
        public abstract object GetData();

        /// <summary>
        /// Tries to write formatted file with complete formatted structure bytes representation at a specified path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>If file was successfully filled with formatted data</returns>
        public bool SaveToFile(string filePath)
        {
             return SaveToFile(filePath, true);
        }

        /// <summary>
        /// Tries to write formatted file with data items representation at a specified path
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="replace">If existing file is to be replaced</param>
        /// <returns>If file was successfully filled with formatted data></returns>
        public bool SaveToFile(string filePath, bool replace)
        {
            bool emptyPath = string.IsNullOrEmpty(filePath);
            Debug.Assert(!emptyPath, "Can't save file at null or empty path");
            if (emptyPath)
            {
                throw new ArgumentException("Can't save file at null or empty path");
            }
            bool result = false;
            string tempFilePath = filePath + FormatConversionSettings.TempFileExtension;
            string backupFilePath = filePath + FormatConversionSettings.BackupFileExtension;
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
            using (var fileStream = File.Open(tempFilePath, FileMode.Create))
            {
                if (!fileStream.CanWrite)
                {
                    File.Delete(tempFilePath);
                    fileStream.Close();
                    throw new IOException("FileStream can't write");
                }
                else if (!File.Exists(filePath) || replace)
                {
                    WriteFormattedDataToStream(GetData(), fileStream);
                    fileStream.Close();
                    result = true;
                    if (File.Exists(filePath))
                    {
                        File.Replace(tempFilePath, filePath, backupFilePath);
                    }
                    else
                    {
                        File.Copy(tempFilePath, filePath);
                        File.Delete(tempFilePath);
                    }
                }
                else
                {
                    Console.WriteLine("Can't save file because replacement option was disabled for existing files");
                }
            }
            return result;
        }

        /// <summary>
        /// Writes formatted data to the stream using format-specific features. Important: stream is not closed in case it needs to be processed further.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="stream"></param>
        /// <returns>If data was successfully written to the stream</returns>
        protected virtual void WriteFormattedDataToStream(object data, Stream stream)
        {
            Debug.Assert(data is IEnumerable<byte>, $"{GetType().Name} can't write null or invalid data type to stream");
            Debug.Assert(stream != null, $"{GetType().Name} can't write data to null stream");
            var binaryWriter = new BinaryWriter(stream);
            binaryWriter.Write(((IEnumerable<byte>) data).ToArray());
            binaryWriter.Flush();
        }

        /// <summary>
        /// Data items collection indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IFormatDataItem this[int index] => _readonlyDataCollection[index];

        public void Dispose()
        {
            _collectionChangedDelegates.ForEach(d => _dataCollection.CollectionChanged -= d);
            _collectionChangedDelegates.Clear();
            ClearData();
            if (!FormatProcessorsCache.ContainsKey(Format))
            {
                FormatProcessorsCache.Add(Format, new Queue<IFormatProcessor>());
            }
            FormatProcessorsCache[Format].Enqueue(this);
        }
    }
}
