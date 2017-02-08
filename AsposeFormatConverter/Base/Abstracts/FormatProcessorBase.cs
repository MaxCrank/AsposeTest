using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("AsposeFormatConverter.Tests")]
namespace AsposeFormatConverter.Base
{
    /// <summary>
    /// Base format processor class with general functionality. Intended for using by all format processor implementations.
    /// </summary>
    internal abstract class FormatProcessorBase : IFormatProcessor
    {
        private static readonly Regex FormatRegex = new Regex(FormatConversionSettings.FormatOnlyRegex);

        private static Dictionary<ConvertedFormat, Queue<IFormatProcessor>> FormatProcessorsCache = 
            new Dictionary<ConvertedFormat, Queue<IFormatProcessor>>();

        private readonly ObservableCollection<IFormatDataItem> _dataCollection = new ObservableCollection<IFormatDataItem>();

        private List<NotifyCollectionChangedEventHandler> _collectionChangedDelegates = new List<NotifyCollectionChangedEventHandler>();

        private readonly string _stringFormat;

        /// <remarks>
        /// Implements ReadOnlyObservableCollection anti-pattern
        /// </remarks>
        public IEnumerable<IFormatDataItem> Data => _dataCollection.AsEnumerable();

        public int DataItemsCount => _dataCollection.Count;

        public abstract ConvertedFormat Format { get; }

        protected FormatProcessorBase()
        {
            _stringFormat = Enum.GetName(typeof(ConvertedFormat), Format).ToLowerInvariant();
        }

        public void AddDataCollectionChangedHandler(NotifyCollectionChangedEventHandler eventHandler)
        {
            if (eventHandler == null)
            {
                throw new ArgumentNullException($"Passed {GetType().Name} collection event handler is null");
            }
            if (_collectionChangedDelegates.Contains(eventHandler))
            {
                throw new InvalidOperationException($"Passed {GetType().Name} collection event handler was already added");
            }
            _dataCollection.CollectionChanged += eventHandler;
            _collectionChangedDelegates.Add(eventHandler);
        }

        public void RemoveDataCollectionChangedHandler(NotifyCollectionChangedEventHandler eventHandler)
        {
            if (eventHandler == null)
            {
                throw new ArgumentNullException($"Passed {GetType().Name} collection event handler is null");
            }
            if (!_collectionChangedDelegates.Contains(eventHandler))
            {
                throw new InvalidOperationException($"Passed {GetType().Name} collection event handler was not added");
            }
            _dataCollection.CollectionChanged -= eventHandler;
            _collectionChangedDelegates.Remove(eventHandler);
        }

        /// <summary>
        /// Adds the data item. By default, item is cloned before adding to prevent entwined dependencies.
        /// </summary>
        /// <param name="dataItem"></param>
        /// <param name="cloneInputDataItem">If set to true (by default), item will be cloned before adding to prevent entwined dependencies.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void AddDataItem(IFormatDataItem dataItem, bool cloneInputDataItem = true)
        {
            if (dataItem == null)
            {
                throw new ArgumentNullException($"{nameof(IFormatDataItem)} instance is null and can't be added");
            }
            if (_dataCollection.Contains(dataItem) && !cloneInputDataItem)
            {
                throw new InvalidOperationException($"{nameof(IFormatDataItem)} instance is already in {nameof(Data)} collection. Please, use clone option to make a copy if that was your intention.");
            }
            var itemToAdd = cloneInputDataItem ? (dataItem.Clone() as IFormatDataItem) : dataItem;
            _dataCollection.Add(itemToAdd);
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
        /// Get ready-to-work processor instance of the same format. Disposed instances are being cached.
        /// </summary>
        /// <returns>New instance if there are no free processors, otherwise reuses the one that was disposed and now is free</returns>
        public IFormatProcessor GetFormatProcessor()
        {
            if (FormatProcessorsCache == null)
            {
                throw new Exception($"{nameof(FormatProcessorBase)}.{nameof(FormatProcessorsCache)} was not initialized");
            }
            if (FormatProcessorsCache.ContainsKey(Format) && FormatProcessorsCache[Format].Count > 0)
            {
                return FormatProcessorsCache[Format].Dequeue();
            }
            else
            {
                return Activator.CreateInstance(GetType()) as IFormatProcessor;
            }
        }

        public static void ClearFormatProcessorsCache()
        {
            FormatProcessorsCache.Clear();
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
            else if (!_dataCollection.Contains(dataItem))
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
        /// Clear and repopulate data collection
        /// </summary>
        /// <param name="initialData"></param>
        /// <param name="cloneInitialDataItems">If set to true (by default), each item will be cloned before adding to prevent entwined dependencies.</param>
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
        /// Tries to write or replace formatted file with complete formatted structure bytes representation at a specified path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>If file was successfully filled with formatted data</returns>
        public bool SaveToFile(string filePath, bool makeBackup = false)
        {
             return SaveToFile(filePath, true, makeBackup);
        }

        /// <summary>
        /// Tries to write formatted file with data items representation at a specified path
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="replace">If existing file is to be replaced</param>
        /// <returns>If file was successfully filled with formatted data></returns>
        public bool SaveToFile(string filePath, bool replace, bool makeBackup = false)
        {
            if (string.IsNullOrEmpty(filePath))
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
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    File.Copy(tempFilePath, filePath);
                    if (makeBackup)
                    {
                        File.Copy(tempFilePath, backupFilePath);
                    }
                    File.Delete(tempFilePath);
                    result = true;
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
        public IFormatDataItem this[int index] => _dataCollection[index];

        public IEnumerator<IFormatDataItem> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

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
