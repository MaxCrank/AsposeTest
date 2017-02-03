using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AsposeFormatConverter.Base
{
    /// <summary>
    /// Common format converter. Can be referenced with various format processors to generate flexible library.
    /// </summary>
    public class CommonFormatConverter : ICommonFormatConverter
    {
        private readonly Regex _fullPathFormatRegex = new Regex(FormatConversionSettings.FormatFullPathRegex);

        private static List<IFormatProcessor> _formatProcessors = new List<IFormatProcessor>();

        private static bool _formatProcessorsInitialized = false;

        static CommonFormatConverter()
        {
            InitFormatProcessors();
        }

        /// <summary>
        /// Used for loading all format processors. Should be invoked before usage.
        /// </summary>
        private static void InitFormatProcessors()
        {
            Debug.Assert(!_formatProcessorsInitialized, "Format processors are already initialized");
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.GetInterface(typeof(IFormatProcessor).FullName) != null))
                {
                    var processor = Activator.CreateInstance(type) as IFormatProcessor;
                    Debug.Assert(processor != null, "One or more format processor activations failed on init - type check is probably not complete");
                    Debug.Assert(processor.Format != ConvertedFormat.UNKNOWN, "Fatal error: initialized processor formats should not be unknown");
                    _formatProcessors.Add(processor);
                }
            }
            _formatProcessorsInitialized = true;
        }

        /// <summary>
        /// Checks if specified format is supported by any of format processors and returns a new instance of the corresponding one
        /// </summary>
        /// <param name="format"></param>
        /// <returns>New format processor instance if specified format is supported, otherwise null</returns>
        public IFormatProcessor CreateFormatProcessor(ConvertedFormat format)
        {
            var processor = _formatProcessors.Find(p => p.Format == format);
            if (processor == null)
            {
                Console.WriteLine($"Not supported format: {format}");
            }
            return processor?.GetFormatProcessor();
        }

        /// <summary>
        /// Tries to convert data from provided format processor
        /// </summary>
        /// <param name="inputFormatProcessor"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="outputFormat"></param>
        /// <returns>If conversion was successful</returns>
        public bool Convert(IFormatProcessor inputFormatProcessor, string outputFilePath, ConvertedFormat outputFormat)
        {
            Debug.Assert(inputFormatProcessor != null, "Can't convert data from null-valued input format processor");
            Debug.Assert(!string.IsNullOrEmpty(outputFilePath), "Can't save data at null or empty file path");
            bool result = false;
            var outputProcessor = CreateFormatProcessor(outputFormat);
            if (outputProcessor != null)
            {
                outputProcessor.SetData(inputFormatProcessor.Data.AsEnumerable(), true);
                result = outputProcessor.SaveToFile(outputFilePath);
                outputProcessor.Dispose();
            }
            return result;
        }

        /// <summary>
        /// Tries to convert an input file with explicit format specification
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="outputFormat"></param>
        /// <returns>If conversion was successful</returns>
        public bool Convert(string inputFilePath, string outputFilePath, ConvertedFormat outputFormat)
        {
            Debug.Assert(!string.IsNullOrEmpty(inputFilePath), "Can't read file from null or empty path");
            Debug.Assert(!string.IsNullOrEmpty(outputFilePath), "Can't write file to null or empty path");
            Debug.Assert(File.Exists(inputFilePath), "Can't read from path leading to non-existing file");
            Debug.Assert(outputFormat != ConvertedFormat.UNKNOWN, "Converted format should not be unknown");
            bool result = false;
            ConvertedFormat inputFormat;
            if (TryGetSupportedFormatFromPath(inputFilePath, out inputFormat))
            {
                var outputFormatProcessor = CreateFormatProcessor(outputFormat);
                if (outputFormatProcessor != null)
                {
                    var inputProcessor = CreateFormatProcessor(inputFormat);
                    if (inputProcessor.ReadFromFile(inputFilePath))
                    {
                        outputFormatProcessor.SetData(inputProcessor.Data, false);
                        result = outputFormatProcessor.SaveToFile(outputFilePath);
                        outputFormatProcessor.Dispose();
                        inputProcessor.Dispose();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Tries to convert an input file without explicit format specification
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="inputFormat"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="outputFormat"></param>
        /// <returns>If conversion was successful</returns>
        public bool Convert(string inputFilePath, ConvertedFormat inputFormat, string outputFilePath,
            ConvertedFormat outputFormat)
        {
            Debug.Assert(!string.IsNullOrEmpty(inputFilePath), "Can't read file from null or empty path");
            Debug.Assert(!string.IsNullOrEmpty(outputFilePath), "Can't write file to null or empty path");
            Debug.Assert(File.Exists(inputFilePath), "Can't read from path leading to non-existing file");
            Debug.Assert(inputFormat != ConvertedFormat.UNKNOWN && outputFormat != ConvertedFormat.UNKNOWN,
                "Converted format should not be unknown");
            bool result = false;
            var inputFormatProcessor = CreateFormatProcessor(inputFormat);
            if (inputFormatProcessor != null && inputFormatProcessor.ReadFromFile(inputFilePath))
            {
                var outputFormatProcessor = CreateFormatProcessor(outputFormat);
                if (outputFormatProcessor != null)
                {
                    outputFormatProcessor.SetData(inputFormatProcessor.Data, true);
                    result = outputFormatProcessor.SaveToFile(outputFilePath);
                    outputFormatProcessor.Dispose();
                    inputFormatProcessor.Dispose();
                }
            }
            return result;
        }

        /// <summary>
        /// Tries to get supported file format from provided file path by file's extension
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="format"></param>
        /// <returns>If supported format was revealed</returns>
        public bool TryGetSupportedFormatFromPath(string filePath, out ConvertedFormat format)
        {
            Debug.Assert(!string.IsNullOrEmpty(filePath), "Extension can't be retrieved from null or empty file path");
            Debug.Assert(File.Exists(filePath), "Extension can't be retrieved from path leading to non-existing file");
            bool result = false;
            format = ConvertedFormat.UNKNOWN;
            if (_fullPathFormatRegex.IsMatch(filePath))
            {
                string extension = _fullPathFormatRegex.Match(filePath).Value;
                var supportedFormatProcessor = _formatProcessors.Find(p => p.SupportsFormat(extension));
                if (supportedFormatProcessor != null)
                {
                    format = supportedFormatProcessor.Format;
                    result = format != ConvertedFormat.UNKNOWN;
                }
            }
            return result;
        }
    }
}
