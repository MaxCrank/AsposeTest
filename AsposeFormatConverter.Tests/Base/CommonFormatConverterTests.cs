using NUnit.Framework;
using AsposeFormatConverter.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsposeFormatConverter.Base.Tests
{
    [TestFixture()]
    public class CommonFormatConverterTests
    {
        [Test()]
        public void CreateCommonFormatConverter()
        { 
            CommonFormatConverter converter = new CommonFormatConverter();
            Assert.AreNotEqual(converter, null);
        }

        [Test()]
        [TestCase(ConvertedFormat.BIN)]
        [TestCase(ConvertedFormat.XML)]
        public void CreateKnownFormatProcessor(ConvertedFormat format)
        {
            var converter = new CommonFormatConverter();
            Assert.AreNotEqual(converter, null);
            var processor = converter.CreateFormatProcessor(format);
            Assert.AreNotEqual(processor, null);
            Assert.AreEqual(processor.Format, format);
        }

        [Test()]
        [TestCase(ConvertedFormat.UNKNOWN)]
        public void CreateUnknownFormatProcessor(ConvertedFormat format)
        {
            var converter = new CommonFormatConverter();
            Assert.AreNotEqual(converter, null);
            Assert.Throws<ArgumentException>(() => converter.CreateFormatProcessor(format));
        }

        [Test()]
        [TestCase(ConvertedFormat.BIN, ConvertedFormat.BIN)]
        [TestCase(ConvertedFormat.XML, ConvertedFormat.XML)]
        [TestCase(ConvertedFormat.BIN, ConvertedFormat.XML)]
        [TestCase(ConvertedFormat.XML, ConvertedFormat.BIN)]
        public void ConvertKnownFormatArgumentExceptionTest(ConvertedFormat inputFormat, ConvertedFormat outputFormat)
        {
            var converter = new CommonFormatConverter();
            Assert.AreNotEqual(converter, null);
            Assert.Throws<ArgumentNullException>(() => converter.ConvertProcessor(null, "outfile", outputFormat));
            Assert.Throws<ArgumentException>(() => converter.ConvertProcessor(converter.CreateFormatProcessor(inputFormat), 
                null, outputFormat));
            Assert.Throws<ArgumentException>(() => converter.ConvertProcessor(converter.CreateFormatProcessor(inputFormat),
                string.Empty, outputFormat));
        }

        [Test()]
        [TestCase(ConvertedFormat.BIN, ConvertedFormat.BIN)]
        [TestCase(ConvertedFormat.XML, ConvertedFormat.XML)]
        [TestCase(ConvertedFormat.BIN, ConvertedFormat.XML)]
        [TestCase(ConvertedFormat.XML, ConvertedFormat.BIN)]
        public void ConvertValidKnownFormatsSuccessTest(ConvertedFormat inputFormat, ConvertedFormat outputFormat)
        {
            string filePath1 = "file1";
            string filePath2 = "file2";
            var converter = new CommonFormatConverter();
            Assert.AreNotEqual(converter, null);
            using (var processor = converter.CreateFormatProcessor(inputFormat))
            {
                Assert.AreNotEqual(processor, null);
                Assert.AreEqual(true, converter.ConvertProcessor(processor, filePath1, outputFormat));
                processor.Dispose();
                Assert.DoesNotThrow(() => converter.ConvertProcessor(processor, filePath1, outputFormat));
            }
            Assert.AreEqual(true, converter.Convert(filePath1, outputFormat, filePath2, inputFormat));
            Assert.DoesNotThrow(() => converter.Convert(filePath1, outputFormat, filePath2, inputFormat));
            Assert.AreEqual(true, converter.Convert(filePath2, inputFormat, filePath1, outputFormat));
            Assert.DoesNotThrow(() => converter.Convert(filePath2, inputFormat, filePath1, outputFormat));
            Assert.AreEqual(true, File.Exists(filePath1));
            Assert.AreEqual(true, File.Exists(filePath2));
            File.Delete(filePath1);
            File.Delete(filePath2);
        }

        [Test()]
        public void TryGetSupportedFormatFromPathFalse()
        {
            string filePath = "file";
            var converter = new CommonFormatConverter();
            Assert.AreNotEqual(converter, null);
            ConvertedFormat format;
            if (!File.Exists(filePath) || string.IsNullOrEmpty(filePath))
            {
                Assert.Throws<ArgumentException>(() => converter.TryGetSupportedFormatFromPath("file", out format));
            }
            else
            {
                Assert.AreEqual(false, converter.TryGetSupportedFormatFromPath("file", out format));
            }
        }

        [Test()]
        [TestCase(ConvertedFormat.BIN)]
        [TestCase(ConvertedFormat.XML)]
        public void TryGetSupportedFormatFromPathTrue(ConvertedFormat format)
        {
            var converter = new CommonFormatConverter();
            Assert.AreNotEqual(converter, null);
            using (var processor = converter.CreateFormatProcessor(format))
            {
                Assert.AreNotEqual(processor, null);
                Assert.AreEqual(true, converter.ConvertProcessor(processor, $"file.{format}", format));
            }
            ConvertedFormat supportedFormat;
            Assert.AreEqual(true, converter.TryGetSupportedFormatFromPath($"file.{format}", out supportedFormat));
            Assert.AreEqual(supportedFormat, format);
        }

        [Test()]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("3/3\\1?")]
        public void TryGetSupportedFormatFromPathException(string filePath)
        {
            
            var converter = new CommonFormatConverter();
            Assert.AreNotEqual(converter, null);
            ConvertedFormat format;
            Assert.Throws<ArgumentException>(() => converter.TryGetSupportedFormatFromPath(filePath, out format));
        }
    }
}