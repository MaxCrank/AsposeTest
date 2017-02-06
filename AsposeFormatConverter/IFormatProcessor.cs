using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormatConversion.Interfaces
{
    /// <summary>
    /// Interface for a common format processor
    /// </summary>
    public interface IFormatProcessor
    {
        /// <summary>
        /// Returns format string representation
        /// </summary>
        string Format { get; }
        /// <summary>
        /// Collection of parsed data items
        /// </summary>
        IEnumerable<IFormatDataItem> Data { get; }
        /// <summary>
        /// Tries to read file at specified path, parse and fill data
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>If file was successfuly parsed</returns>
        bool ReadFile(string filePath);
        void SetData(IEnumerable<IFormatDataItem> initialData);
        void AddDataItem(IFormatDataItem dataItem);
        void RemoveDataItem(IFormatDataItem dataItem);
        /// <summary>
        /// Tries to parse object to data item
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parsedDataItem"></param>
        /// <returns>If parsing was successful</returns>
        bool TryParse(object input, out IFormatDataItem parsedDataItem);
        /// <summary>
        /// Removes all data items
        /// </summary>
        void ClearData();
        /// <summary>
        /// Tries to write formatted file with data items representation at a specified path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>If file was successfully filled with formatted data</returns>
        bool WriteFile(string filePath);
        /// <summary>
        /// Returns new instance of the same format processor
        /// </summary>
        /// <returns></returns>
        IFormatProcessor CreateNew();
        /// <summary>
        /// Checks if this processor supports specified format
        /// </summary>
        /// <param name="format"></param>
        /// <returns>If this processor supports specified format</returns>
        bool SupportsFormat(string format);

        IFormatDataItem this[int index] { get; }
    }
}