using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsposeFormatConverter.Base;
using NUnit.Framework;
using System.Collections.Specialized;
using System.IO;
using System.Collections;

namespace AsposeFormatConverter.Tests
{
    [TestFixture()]
    public class FormatProcessBaseTest
    {
        [TestCaseSource(typeof(SignificantFormatsTestData), nameof(SignificantFormatsTestData.TestCases))]
        public void Create(ConvertedFormat format)
        {
            FormatProcessorBase.ClearFormatProcessorsCache();
            var converter = new CommonFormatConverter();
            var processor = converter.CreateFormatProcessor(format) as FormatProcessorBase;
            Assert.IsNotNull(processor);
            Assert.IsNotNull(processor.Data);
            Assert.AreNotEqual(ConvertedFormat.UNKNOWN, processor.Format);
            Assert.DoesNotThrow(() => processor.GetFormatProcessor());
        }

        [TestCaseSource(typeof(SignificantFormatsTestData), nameof(SignificantFormatsTestData.TestCases))]
        public void ManipulateCollectionHandlers(ConvertedFormat format)
        {
            FormatProcessorBase.ClearFormatProcessorsCache();
            var converter = new CommonFormatConverter();
            var processor = converter.CreateFormatProcessor(format) as FormatProcessorBase;
            Assert.Throws<ArgumentNullException>(() => processor.AddDataCollectionChangedHandler(null));
            NotifyCollectionChangedEventHandler handler1 = (sender, args) => { };
            NotifyCollectionChangedEventHandler handler2 = (sender, args) => { };
            Assert.DoesNotThrow(() => processor.AddDataCollectionChangedHandler(handler1));
            Assert.Throws<InvalidOperationException>(() => processor.AddDataCollectionChangedHandler(handler1));
            Assert.Throws<ArgumentNullException>(() => processor.RemoveDataCollectionChangedHandler(null));
            Assert.Throws<InvalidOperationException>(() => processor.RemoveDataCollectionChangedHandler(handler2));
            Assert.DoesNotThrow(() => processor.AddDataCollectionChangedHandler(handler2));
            Assert.DoesNotThrow(() => processor.RemoveDataCollectionChangedHandler(handler2));
            Assert.DoesNotThrow(() => processor.RemoveDataCollectionChangedHandler(handler1));
        }

        [TestCaseSource(typeof(SignificantFormatsTestData), nameof(SignificantFormatsTestData.TestCases))]
        public void AddRemoveDataItem(ConvertedFormat format)
        {
            FormatProcessorBase.ClearFormatProcessorsCache();
            var converter = new CommonFormatConverter();
            var processor = converter.CreateFormatProcessor(format) as FormatProcessorBase;
            IFormatDataItem dataItem = new FormatDataItem();
            Assert.Throws<ArgumentNullException>(() => processor.AddDataItem(null));
            Assert.DoesNotThrow(() => processor.AddDataItem(dataItem, false));
            Assert.Throws<InvalidOperationException>(() => processor.AddDataItem(dataItem, false));
            Assert.IsFalse(processor.RemoveDataItem(null));
            Assert.IsFalse(processor.RemoveDataItem(dataItem.Clone() as IFormatDataItem));
            Assert.IsTrue(processor.RemoveDataItem(dataItem));
        }

        [TestCaseSource(typeof(SignificantFormatsTestData), nameof(SignificantFormatsTestData.TestCases))]
        public void SupportsFormat(ConvertedFormat format)
        {
            FormatProcessorBase.ClearFormatProcessorsCache();
            var converter = new CommonFormatConverter();
            var processor = converter.CreateFormatProcessor(format) as FormatProcessorBase;
            Assert.IsTrue(processor.Format == format);
            Assert.IsTrue(processor.SupportsFormat(format.ToString()));
            Assert.IsTrue(processor.SupportsFormat("." + format.ToString()));
            foreach (ConvertedFormat frmt in SignificantFormatsTestData.TestCases)
            {
                if (frmt != format)
                {
                    var strFormat = frmt.ToString();
                    Assert.IsFalse(processor.SupportsFormat(strFormat));
                    Assert.IsFalse(processor.SupportsFormat("." + strFormat));
                }
            }
        }

        [TestCaseSource(typeof(SignificantFormatsTestData), nameof(SignificantFormatsTestData.TestCases))]
        public void Dispose(ConvertedFormat format)
        {
            FormatProcessorBase.ClearFormatProcessorsCache();
            var converter = new CommonFormatConverter();
            var processor = converter.CreateFormatProcessor(format) as FormatProcessorBase;
            processor.AddDataCollectionChangedHandler((sender, args) => { });
            processor.AddDataCollectionChangedHandler((sender, args) => { });
            processor.AddNewDataItem(new DateTime(), string.Empty, 100);
            processor.AddNewDataItem(new DateTime(2010, 02, 10), "VS", 200);
            Assert.DoesNotThrow(() => processor.Dispose());
        }

        [TestCaseSource(typeof(SignificantFormatsTestData), nameof(SignificantFormatsTestData.TestCases))]
        public void ReadWriteFiles(ConvertedFormat format)
        {
            FormatProcessorBase.ClearFormatProcessorsCache();
            string filePath1 = "file1";
            string filePath2 = "file2";
            if (File.Exists(filePath1))
            {
                File.Delete(filePath1);
            }
            if (File.Exists(filePath2))
            {
                File.Delete(filePath2);
            }
            var converter = new CommonFormatConverter();
            var processor = converter.CreateFormatProcessor(format) as FormatProcessorBase;
            Assert.Throws<ArgumentException>(() => processor.ReadFromFile(filePath1));
            Assert.Throws<ArgumentException>(() => processor.ReadFromFile(null));
            processor.AddNewDataItem(new DateTime(), string.Empty, 100);
            processor.AddNewDataItem(new DateTime(2010, 02, 10), "VS", 200);
            Assert.DoesNotThrow(() => processor.SaveToFile(filePath1));
            Assert.IsTrue(processor.SaveToFile(filePath1));
            Assert.DoesNotThrow(() => processor.ReadFromFile(filePath1));
            Assert.IsTrue(processor.ReadFromFile(filePath1));
            Assert.DoesNotThrow(() => processor.SaveToFile(filePath2));
            Assert.IsTrue(processor.SaveToFile(filePath2));
            Assert.DoesNotThrow(() => processor.ReadFromFile(filePath2));
            Assert.IsTrue(processor.ReadFromFile(filePath2));
            if (File.Exists(filePath1))
            {
                File.Delete(filePath1);
            }
            if (File.Exists(filePath2))
            {
                File.Delete(filePath2);
            }
        }
    }
}
