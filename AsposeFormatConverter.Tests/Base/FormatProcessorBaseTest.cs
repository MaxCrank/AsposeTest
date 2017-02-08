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
    public class FormatProcessorBaseTest
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
        public void Cache(ConvertedFormat format)
        {
            FormatProcessorBase.ClearFormatProcessorsCache();
            var converter = new CommonFormatConverter();
            var processor1 = converter.CreateFormatProcessor(format) as FormatProcessorBase;
            var handler = new NotifyCollectionChangedEventHandler((sender, args) => { });
            var dataItem = new FormatDataItem();
            processor1.Dispose();
            string filePath = "file";
            Assert.IsTrue(processor1.IsCached);
            Assert.Throws<InvalidOperationException>(() => processor1.Dispose());
            Assert.Throws<InvalidOperationException>(() => { var someVar = processor1.Data; });
            Assert.Throws<InvalidOperationException>(() => { processor1.AddDataCollectionChangedHandler(handler); });
            Assert.Throws<InvalidOperationException>(() => { processor1.RemoveDataCollectionChangedHandler(handler); });
            Assert.Throws<InvalidOperationException>(() => { processor1.AddDataItem(dataItem); });
            Assert.Throws<InvalidOperationException>(() => { processor1.RemoveDataItem(dataItem); });
            Assert.Throws<InvalidOperationException>(() => { processor1.ClearData(); });
            Assert.Throws<InvalidOperationException>(() => { processor1.GetEnumerator(); });
            Assert.Throws<InvalidOperationException>(() => { processor1.ReadFromFile(filePath); });
            Assert.Throws<InvalidOperationException>(() => { processor1.SaveToFile(filePath); });
            Assert.Throws<InvalidOperationException>(() => { processor1.SetData(null); });
            Assert.DoesNotThrow(() => { processor1.SupportsFormat(format.ToString()); });
            var processor2 = converter.CreateFormatProcessor(format);
            Assert.AreSame(processor1, processor2);
            Assert.False(processor1.IsCached);
            Assert.DoesNotThrow(() => { var someVar = processor1.Data; });
            Assert.DoesNotThrow(() => { processor1.AddDataCollectionChangedHandler(handler); });
            Assert.DoesNotThrow(() => { processor1.RemoveDataCollectionChangedHandler(handler); });
            Assert.DoesNotThrow(() => { processor1.AddDataItem(dataItem); });
            Assert.DoesNotThrow(() => { processor1.RemoveDataItem(dataItem); });
            Assert.DoesNotThrow(() => { processor1.ClearData(); });
            Assert.DoesNotThrow(() => { processor1.GetEnumerator(); });
            Assert.DoesNotThrow(() => { processor1.SaveToFile(filePath); });
            Assert.DoesNotThrow(() => { processor1.ReadFromFile(filePath); });
            Assert.DoesNotThrow(() => { processor1.SetData(new[]{ dataItem }); });
            File.Delete(filePath);
            processor1.Dispose();
            Assert.IsTrue(processor1.IsCached);
            processor2 = processor1.GetFormatProcessor();
            Assert.AreSame(processor1, processor2);
            Assert.False(processor1.IsCached);
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
            File.Delete(filePath1);
            File.Delete(filePath2);
        }
    }
}
