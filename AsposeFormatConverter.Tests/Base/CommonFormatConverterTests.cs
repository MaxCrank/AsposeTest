using NUnit.Framework;
using AsposeFormatConverter.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsposeFormatConverter.Tests
{
    [TestFixture()]
    public class CommonFormatConverterTests
    {
        [Test()]
        public void CreateCommonFormatConverter()
        {
            FormatProcessorBase.ClearFormatProcessorsCache();
            CommonFormatConverter converter = new CommonFormatConverter();
            Assert.IsNotNull(converter);
        }

        [TestCaseSource(typeof(SignificantFormatsTestData), nameof(SignificantFormatsTestData.TestCases))]
        public void CreateKnownFormatProcessor(ConvertedFormat format)
        {
            FormatProcessorBase.ClearFormatProcessorsCache();
            var converter = new CommonFormatConverter();
            Assert.IsNotNull(converter);
            var firstProcessor = converter.CreateFormatProcessor(format);
            Assert.IsNotNull(firstProcessor);
            var secondProcessor = firstProcessor.GetFormatProcessor();
            Assert.IsNotNull(secondProcessor);
            Assert.AreNotSame(firstProcessor, secondProcessor);
            secondProcessor.Dispose();
            Assert.AreSame(secondProcessor.GetFormatProcessor(), secondProcessor);
        }

        [TestCase(ConvertedFormat.UNKNOWN)]
        public void CreateUnknownFormatProcessor(ConvertedFormat format)
        {
            FormatProcessorBase.ClearFormatProcessorsCache();
            var converter = new CommonFormatConverter();
            Assert.IsNotNull(converter);
            Assert.Throws<ArgumentException>(() => converter.CreateFormatProcessor(format));
        }

        [TestCase(ConvertedFormat.BIN, ConvertedFormat.BIN)]
        [TestCase(ConvertedFormat.XML, ConvertedFormat.XML)]
        [TestCase(ConvertedFormat.BIN, ConvertedFormat.XML)]
        [TestCase(ConvertedFormat.XML, ConvertedFormat.BIN)]
        public void ConvertKnownFormatArgumentExceptionTest(ConvertedFormat inputFormat, ConvertedFormat outputFormat)
        {
            FormatProcessorBase.ClearFormatProcessorsCache();

            var converter = new CommonFormatConverter();
            Assert.IsNotNull(converter);
            Assert.Throws<ArgumentNullException>(() => converter.ConvertProcessor(null, "outfile", outputFormat));
            Assert.Throws<ArgumentException>(() => converter.ConvertProcessor(converter.CreateFormatProcessor(inputFormat), 
                null, outputFormat));
            Assert.Throws<ArgumentException>(() => converter.ConvertProcessor(converter.CreateFormatProcessor(inputFormat),
                string.Empty, outputFormat));
        }

        [TestCase(ConvertedFormat.BIN, ConvertedFormat.BIN)]
        [TestCase(ConvertedFormat.XML, ConvertedFormat.XML)]
        [TestCase(ConvertedFormat.BIN, ConvertedFormat.XML)]
        [TestCase(ConvertedFormat.XML, ConvertedFormat.BIN)]
        public void ConvertValidKnownFormatsSuccessTest(ConvertedFormat inputFormat, ConvertedFormat outputFormat)
        {
            FormatProcessorBase.ClearFormatProcessorsCache();
            string filePath1 = "file1";
            string filePath2 = "file2";
            var converter = new CommonFormatConverter();
            Assert.IsNotNull(converter);
            using (var processor = converter.CreateFormatProcessor(inputFormat))
            {
                Assert.IsNotNull(processor);
                Assert.IsTrue(converter.ConvertProcessor(processor, filePath1, outputFormat));
                processor.Dispose();
                Assert.DoesNotThrow(() => converter.ConvertProcessor(processor, filePath1, outputFormat));
            }
            Assert.IsTrue(converter.Convert(filePath1, outputFormat, filePath2, inputFormat));
            Assert.DoesNotThrow(() => converter.Convert(filePath1, outputFormat, filePath2, inputFormat));
            Assert.IsTrue(converter.Convert(filePath2, inputFormat, filePath1, outputFormat));
            Assert.DoesNotThrow(() => converter.Convert(filePath2, inputFormat, filePath1, outputFormat));
            Assert.IsTrue(File.Exists(filePath1));
            Assert.IsTrue(File.Exists(filePath2));
            File.Delete(filePath1);
            File.Delete(filePath2);
        }

        [Test()]
        public void TryGetSupportedFormatFromPathFalse()
        {
            FormatProcessorBase.ClearFormatProcessorsCache();
            string filePath = "file";
            var converter = new CommonFormatConverter();
            Assert.IsNotNull(converter);
            ConvertedFormat format;
            if (!File.Exists(filePath) || string.IsNullOrEmpty(filePath))
            {
                Assert.Throws<ArgumentException>(() => converter.TryGetSupportedFormatFromPath("file", out format));
            }
            else
            {
                Assert.IsFalse(converter.TryGetSupportedFormatFromPath("file", out format));
            }
        }

        [TestCaseSource(typeof(SignificantFormatsTestData), nameof(SignificantFormatsTestData.TestCases))]
        public void TryGetSupportedFormatFromPathTrue(ConvertedFormat format)
        {
            FormatProcessorBase.ClearFormatProcessorsCache();
            var converter = new CommonFormatConverter();
            Assert.IsNotNull(converter);
            using (var processor = converter.CreateFormatProcessor(format))
            {
                Assert.IsNotNull(processor);
                Assert.IsTrue(converter.ConvertProcessor(processor, $"file.{format}", format));
            }
            ConvertedFormat supportedFormat;
            Assert.IsTrue(converter.TryGetSupportedFormatFromPath($"file.{format}", out supportedFormat));
            Assert.AreEqual(supportedFormat, format);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("3/3\\1?")]
        public void TryGetSupportedFormatFromPathException(string filePath)
        {
            FormatProcessorBase.ClearFormatProcessorsCache();
            var converter = new CommonFormatConverter();
            Assert.IsNotNull(converter);
            ConvertedFormat format;
            Assert.Throws<ArgumentException>(() => converter.TryGetSupportedFormatFromPath(filePath, out format));
        }
    }
}