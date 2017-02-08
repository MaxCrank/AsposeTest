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
    public interface IFormatProcessor : IDisposable, IEnumerable<IFormatDataItem>
    {
        /// <summary>
        /// Returns format string representation
        /// </summary>
        ConvertedFormat Format { get; }

        /// <summary>
        /// Readonly collection of parsed data items. Modification is allowed with corresponding methods only.
        /// </summary>
        IEnumerable<IFormatDataItem> Data { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is cached for reusage. If you call Dispose and try to use the instance further, 
        /// exception will be thrown. 
        /// </summary>
        bool IsCached { get; }

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

        void SetData(IEnumerable<IFormatDataItem> initialData);

        void SetData(IEnumerable<IFormatDataItem> initialData, bool cloneInitialDataItems);

        /// <summary>
        /// Adds the data item. By default, item is cloned before adding to prevent entwined dependencies.
        /// </summary>
        /// <param name="dataItem"></param>
        void AddDataItem(IFormatDataItem dataItem);

        /// <summary>
        /// Adds the data item. By default, item is cloned before adding to prevent entwined dependencies.
        /// </summary>
        /// <param name="dataItem"></param>
        /// <param name="cloneInputDataItem">If set to true (by default), item will be cloned before adding to prevent entwined dependencies.</param>
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
        /// <returns>If file was successfully filled with formatted data></returns>
        bool SaveToFile(string filePath);

        /// <summary>
        /// Tries to write formatted file with data items representation at a specified path
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="makeBackup">Specifies if backup of old file is to be made</param>
        /// <returns>If file was successfully filled with formatted data></returns>
        bool SaveToFile(string filePath, bool makeBackup);

        /// <summary>
        /// Tries to write formatted file with data items representation at a specified path
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="makeBackup">Specifies if backup of old file is to be made</param>
        /// <param name="replace">If existing file is to be replaced</param>
        /// <returns>If file was successfully filled with formatted data></returns>
        bool SaveToFile(string filePath, bool makeBackup, bool replace);

        /// <summary>
        /// Get ready-to-work processor instance of the same format. Disposed instances are being cached.
        /// </summary>
        /// <returns></returns>
        IFormatProcessor GetFormatProcessor();

        /// <summary>
        /// Data items collection indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IFormatDataItem this[int index] { get; }

        int DataItemsCount { get; }
    }
}