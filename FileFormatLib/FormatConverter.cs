using FormatConversion.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FormatConversion.Converter
{
    /// <summary>
    /// Common format converter. Can be referenced with various format processors to generate flexible library.
    /// </summary>
    public class FormatConverter : IFormatConverter
    {
        List<IFormatProcessor> _formatProcessors = new List<IFormatProcessor>();

        /// <summary>
        /// Used for loading all format processors. Should be invoked before usage.
        /// </summary>
        public void InitFormatProcessors()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.GetInterface(typeof(IFormatProcessor).FullName) != null))
                {
                    _formatProcessors.Add(Activator.CreateInstance(type) as IFormatProcessor);
                }
            }
        }
        /// <summary>
        /// Checks if specified format is supported by any of format processors and returns a new instance of the corresponding one
        /// </summary>
        /// <param name="format"></param>
        /// <returns>New format processor instance if specified format is supported, otherwise null</returns>
        public IFormatProcessor GetFormatProcessor(string format)
        {
            IFormatProcessor processor = null;
            if (string.IsNullOrEmpty(format))
            {
                Console.WriteLine("Specified format is null or empty");
            }
            else
            {
                processor = _formatProcessors.Find(p => p.SupportsFormat(format));
                if (processor == null)
                {
                    Console.WriteLine("Not supported format: " + format);
                }
            }
            return processor?.CreateNew();
        }
        /// <summary>
        /// Tries to convert data from provided format processor
        /// </summary>
        /// <param name="inputFormatProcessor"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="outputFormat"></param>
        /// <returns>If conversion was successful</returns>
        public bool Convert(IFormatProcessor inputFormatProcessor, string outputFilePath, string outputFormat)
        {
            bool result = false;
            if (inputFormatProcessor == null)
            {
                Console.WriteLine("Input format processor is null");
            }
            else if (string.IsNullOrEmpty(outputFilePath))
            {
                Console.WriteLine("Specified output file path is null or empty");
            }
            else if (string.IsNullOrEmpty(outputFormat))
            {
                Console.WriteLine("Specified output format is null or empty");
            }
            else
            {
                var outputProcessor = GetFormatProcessor(outputFormat);
                if (outputProcessor != null)
                {
                    outputProcessor.SetData(inputFormatProcessor.Data);
                    result = outputProcessor.WriteFile(outputFilePath);
                }
            }
            return result;
        }
        /// <summary>
        /// Tries to convert an input file with explicit format specification
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="inputFormat"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="outputFormat"></param>
        /// <returns>If conversion was successful</returns>
        public bool Convert(string inputFilePath, string outputFilePath, string outputFormat)
        {
            bool result = false;
            string inputFormat;
            string finalOutputPath = outputFilePath;
            if (TryGetSupportedFormatFromPath(inputFilePath, out inputFormat))
            {
                var outputFormatProcessor = GetFormatProcessor(outputFormat);
                if (outputFormatProcessor != null)
                {
                    var inputProcessor = GetFormatProcessor(inputFormat);
                    if (inputProcessor.ReadFile(inputFilePath))
                    {
                        outputFormatProcessor.SetData(inputProcessor.Data);
                        result = outputFormatProcessor.WriteFile(outputFilePath);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Tries to convert an input file without explicit format specification
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        /// <param name="outputFormat"></param>
        /// <returns>If conversion was successful</returns>
        public bool Convert(string inputFilePath, string inputFormat, string outputFilePath, string outputFormat)
        {
            bool result = false;
            if (string.IsNullOrEmpty(inputFilePath))
            {
                Console.WriteLine("Specified input file path is null or empty");
            }
            else if (!File.Exists(inputFilePath))
            {
                Console.WriteLine("File " + inputFilePath + " does not exist");
            }
            else
            {
                var inputFormatProcessor = GetFormatProcessor(inputFormat);
                if (inputFormatProcessor != null && inputFormatProcessor.ReadFile(inputFilePath))
                {
                    var outputFormatProcessor = GetFormatProcessor(outputFormat);
                    if (outputFormatProcessor != null)
                    {
                        outputFormatProcessor.SetData(inputFormatProcessor.Data);
                        result = outputFormatProcessor.WriteFile(outputFilePath);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Tries to get supported file format from provided file path and its' extension
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="format"></param>
        /// <returns>If supported format was revealed</returns>
        public bool TryGetSupportedFormatFromPath(string filePath, out string format)
        {
            format = string.Empty;
            bool result = false;
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("Specified file path is null or empty");
            }
            else if (!File.Exists(filePath))
            {
                Console.WriteLine("File " + filePath + " does not exist");
            }
            else if (filePath.Contains(".") && filePath[0] != '.')
            {
                string extension = filePath.Split('.').Last();
                var supportedFormatProcessor = _formatProcessors.Find(p => p.SupportsFormat(extension));
                if (supportedFormatProcessor != null)
                {
                    result = true;
                    format = supportedFormatProcessor.Format;
                }
            }
            return result;
        }
    }
}
