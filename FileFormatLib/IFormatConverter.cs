using FormatConversion.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormatConversion.Converter
{
    /// <summary>
    /// Interface for a common format converter
    /// </summary>
    public interface IFormatConverter
    {
        /// <summary>
        /// Used for loading all format processors. Should be invoked before usage.
        /// </summary>
        void InitFormatProcessors();
        /// <summary>
        /// Checks if specified format is supported by any of format processors and returns a new instance of the corresponding one
        /// </summary>
        /// <param name="format"></param>
        /// <returns>New format processor instance if specified format is supported, otherwise null</returns>
        IFormatProcessor GetFormatProcessor(string format);
        /// <summary>
        /// Tries to convert an input file without explicit format specification
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="outputFormat"></param>
        /// <returns>If conversion was successful</returns>
        bool Convert(string inputFilePath, string outputFilePath, string outputFormat);
        /// <summary>
        /// Tries to convert an input file with explicit format specification
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="inputFormat"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="outputFormat"></param>
        /// <returns>If conversion was successful</returns>
        bool Convert(string inputFilePath, string inputFormat, string outputFilePath, string outputFormat);
        /// <summary>
        /// Tries to convert data from provided format processor
        /// </summary>
        /// <param name="inputFormatProcessor"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="outputFormat"></param>
        /// <returns>If conversion was successful</returns>
        bool Convert(IFormatProcessor inputFormatProcessor, string outputFilePath, string outputFormat);
        /// <summary>
        /// Tries to get supported file format from provided file path and its' extension
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="format"></param>
        /// <returns>If supported format was revealed</returns>
        bool TryGetSupportedFormatFromPath(string filePath, out string format);

    }
}
