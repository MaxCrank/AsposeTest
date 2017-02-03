using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace FormatConversion.Base
{
    /// <summary>
    /// Interface for a common format processor. Used to decouple specific format handling implementations from the main library.
    /// </summary>
    public interface IFormatProcessor : IDisposable
    {
        /// <summary>
        /// Returns format string representation
        /// </summary>
        string Format { get; }

        /// <summary>
        /// Readonly collection of parsed data items. Modification is allowed with corresponding methods only.
        /// </summary>
        ReadOnlyObservableCollection<IFormatDataItem> Data { get; }

        bool AddDataCollectionChangedHandler(NotifyCollectionChangedEventHandler eventHandler);

        bool RemoveDataCollectionChangedHandler(NotifyCollectionChangedEventHandler eventHandler);

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

        bool AddDataItem(IFormatDataItem dataItem, bool cloneInputDataItem);

        bool RemoveDataItem(IFormatDataItem dataItem);

        /// <summary>
        /// Removes all data items
        /// </summary>
        void ClearData();

        /// <returns>Complete formatted structure bytes representation</returns>
        byte[] GetData();

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
        /// <param name="saveBackup">If existing file is to be saved with backup extension before replacement</param>
        /// <returnsIf file was successfully filled with formatted data></returns>
        bool SaveToFile(string filePath, bool replace, bool saveBackup);

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