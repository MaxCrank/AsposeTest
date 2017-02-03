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

        public void AddDataItem(IFormatDataItem dataItem, bool cloneInputDataItem = false)
        {
            Debug.Assert(dataItem != null, "Can't add null-valued data item");
            Debug.Assert(!Data.Contains(dataItem), "Item is already in the Data collection. Please, use clone option to make a copy if that was your intention.");
            _dataCollection.Add(cloneInputDataItem ? (dataItem.Clone() as IFormatDataItem) : dataItem);
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
            Debug.Assert(!string.IsNullOrEmpty(filePath), "Can't read file at null or empty path");
            Debug.Assert(File.Exists(filePath), $"File {filePath} does not exist");
            bool result = false;
            try
            {
                result = ParseBytes(File.ReadAllBytes(filePath));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
            Debug.Assert(!string.IsNullOrEmpty(filePath), "Can't save file at null or empty path");
            bool result = false;
            string tempFilePath = filePath + FormatConversionSettings.TempFileExtension;
            string backupFilePath = filePath + FormatConversionSettings.BackupFileExtension;
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
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
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Writes formatted data to the stream using format-specific features
        /// </summary>
        /// <param name="data"></param>
        /// <param name="stream"></param>
        /// <returns>If data was successfully written to the stream</returns>
        protected virtual void WriteFormattedDataToStream(object data, Stream stream)
        {
            Debug.Assert(data is byte[], "Can't write null or invalid data type to stream");
            Debug.Assert(stream != null, "Can't write data to null stream");
            using (var binaryWriter = new BinaryWriter(stream))
            {
                binaryWriter.Write((byte[]) data);
                binaryWriter.Flush();
                binaryWriter.Close();
                stream.Close();
            }
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
                Debug.Assert(_readonlyDataCollection.Count > 0 && index >= 0 && index < _readonlyDataCollection.Count, "Data item index is out of range");
                return _readonlyDataCollection[index];
            }
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
