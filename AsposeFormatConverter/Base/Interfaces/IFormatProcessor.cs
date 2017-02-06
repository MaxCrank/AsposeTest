using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace AsposeFormatConverter.Base
{
    /// <summary>
    /// Interface for a common format processor. Used to decouple specific format handling implementations from the main library.
    /// </summary>
    public interface IFormatProcessor : IDisposable
    {
        /// <summary>
        /// Returns format string representation
        /// </summary>
        ConvertedFormat Format { get; }

        /// <summary>
        /// Readonly collection of parsed data items. Modification is allowed with corresponding methods only.
        /// </summary>
        ReadOnlyObservableCollection<IFormatDataItem> Data { get; }

        void AddDataCollectionChangedHandler(NotifyCollectionChangedEventHandler eventHandler);

        void RemoveDataCollectionChangedHandler(NotifyCollectionChangedEventHandler eventHandler);

        /// <summary>
        /// Checks if this processor supports specified format
        /// </summary>
        /// <param name="format"></param>
        /// <returns>If this processor supports specified format</returns>
        bool SupportsFormat(string format);

        /// <summary>
        /// Tries to read file at specified path, parse and fill data
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>If file was successfuly parsed</returns>
        bool ReadFromFile(string filePath);

        void SetData(IEnumerable<IFormatDataItem> initialData, bool cloneInitialDataItems);

        void AddDataItem(IFormatDataItem dataItem, bool cloneInputDataItem);

        void AddNewDataItem(int day, int month, int year, string brandName, int price);

        void AddNewDataItem(DateTime date, string brandName, int price);

        void AddNewDataItem(string date, string brandName, int price);

        bool RemoveDataItem(IFormatDataItem dataItem);

        /// <summary>
        /// Removes all data items
        /// </summary>
        void ClearData();

        /// <returns>Complete formatted structure bytes representation</returns>
        object GetData();

        /// <summary>
        /// Tries to write formatted file with data items representation at a specified path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>If file was successfully filled with formatted data</returns>
        bool SaveToFile(string filePath);

        /// <summary>
        /// Tries to write formatted file with data items representation at a specified path
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="replace">If existing file is to be replaced</param>
        /// <returns>If file was successfully filled with formatted data></returns>
        bool SaveToFile(string filePath, bool replace);

        /// <summary>
        /// Get ready-to-work processor instance of the same format
        /// </summary>
        /// <returns></returns>
        IFormatProcessor GetFormatProcessor();

        /// <summary>
        /// Data items collection indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IFormatDataItem this[int index] { get; }
    }
}