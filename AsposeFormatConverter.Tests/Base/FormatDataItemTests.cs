using AsposeFormatConverter.Base;
using NUnit.Framework;
using System;

namespace AsposeFormatConverter.Tests
{
    [TestFixture()]
    public class FormatDataItemTests
    {
        [Test()]
        public void CreateFormatData()
        {
            Assert.Throws<ArgumentNullException>(() => new FormatDataItem(null));
            var formatDataItem = new FormatDataItem();
            Assert.IsNotNull(formatDataItem);
            Assert.IsNotNull(new FormatDataItem(formatDataItem));
            Assert.IsTrue(formatDataItem is IFormatDataItem);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("10102000")]
        [TestCase("30.02.2000")]
        [TestCase("29.02.200")]
        public void ManipulateFormatDataDateStringFail(string date)
        {
            var formatDataItem = new FormatDataItem();
            Assert.Throws<ArgumentException>(() => formatDataItem.SetDate(date));
        }

        [TestCase("28.02.2001")]
        [TestCase("29.02.2000")]
        public void ManipulateFormatDataDateStringSuccess(string date)
        {
            var formatDataItem = new FormatDataItem();
            Assert.DoesNotThrow(() => formatDataItem.SetDate(date));
        }

        [TestCase(0, 0, 0)]
        [TestCase(0, 0, 1)]
        [TestCase(0, 1, 0)]
        [TestCase(1, 0, 0)]
        [TestCase(0, 1, 1)]
        [TestCase(1, 1, 0)]
        [TestCase(1, 0, 1)]
        [TestCase(0, 0, -1)]
        [TestCase(0, -1, 0)]
        [TestCase(-1, 0, 0)]
        [TestCase(0, -1, -1)]
        [TestCase(-1, -1, 0)]
        [TestCase(-1, 0, -1)]
        [TestCase(-1, -1, -1)]
        [TestCase(29, 02, 2001)]
        [TestCase(30, 02, 2000)]
        public void ManipulateFormatDataDateIntFail(int day, int month, int year)
        {
            Assert.Throws<ArgumentException>(() => new FormatDataItem().SetDate(day, month, year));
        }

        [TestCase(28, 02, 2001)]
        [TestCase(29, 02, 2000)]
        public void ManipulateFormatDataDateIntSuccess(int day, int month, int year)
        {
            Assert.DoesNotThrow(() => new FormatDataItem().SetDate(day, month, year));
        }

        [TestCase(29, 02, 2000, 30)]
        [TestCase(29, 02, 2000, 0)]
        [TestCase(29, 02, 2000, -1)]
        public void ManipulateFormatDataDayFail(int day, int month, int year, int newDay)
        {
            var dataItem = new FormatDataItem();
            dataItem.SetDate(day, month, year);
            Assert.Throws<ArgumentException>(() => dataItem.SetDay(newDay));
        }

        [TestCase(28, 02, 2000, 29)]
        [TestCase(29, 02, 2000, 1)]
        [TestCase(30, 05, 2010, 20)]
        public void ManipulateFormatDataDaySuccess(int day, int month, int year, int newDay)
        {
            var dataItem = new FormatDataItem();
            dataItem.SetDate(day, month, year);
            Assert.DoesNotThrow(() => dataItem.SetDay(newDay));
        }

        [TestCase(30, 03, 2000, 02)]
        [TestCase(29, 02, 2000, 0)]
        [TestCase(29, 02, 2000, -1)]
        [TestCase(29, 02, 2000, 13)]
        public void ManipulateFormatDataMonthFail(int day, int month, int year, int newMonth)
        {
            var dataItem = new FormatDataItem();
            dataItem.SetDate(day, month, year);
            Assert.Throws<ArgumentException>(() => dataItem.SetMonth(newMonth));
        }

        [TestCase(28, 02, 2000, 05)]
        [TestCase(29, 02, 2000, 01)]
        [TestCase(30, 05, 2010, 12)]
        public void ManipulateFormatDataMonthSuccess(int day, int month, int year, int newMonth)
        {
            var dataItem = new FormatDataItem();
            dataItem.SetDate(day, month, year);
            Assert.DoesNotThrow(() => dataItem.SetMonth(newMonth));
        }

        [TestCase(29, 02, 2000, 0)]
        [TestCase(29, 02, 2000, -1)]
        [TestCase(29, 02, 2000, 2001)]
        [TestCase(29, 02, 2000, 10000)]
        public void ManipulateFormatDataYearFail(int day, int month, int year, int newYear)
        {
            var dataItem = new FormatDataItem();
            dataItem.SetDate(day, month, year);
            Assert.Throws<ArgumentException>(() => dataItem.SetYear(newYear));
        }

        [TestCase(28, 02, 2000, 01)]
        [TestCase(29, 02, 2000, 2004)]
        [TestCase(30, 05, 2010, 9999)]
        public void ManipulateFormatYearSuccess(int day, int month, int year, int newYear)
        {
            var dataItem = new FormatDataItem();
            dataItem.SetDate(day, month, year);
            Assert.DoesNotThrow(() => dataItem.SetYear(newYear));
        }

        [Test()]
        public void CloneTest()
        {
            var clone = new FormatDataItem().Clone();
            Assert.IsNotNull(clone);
            Assert.IsTrue(clone is IFormatDataItem);
        }

        [TestCase(-1)]
        public void ManipulateFormatDataPriceFail(int price)
        {
            var dataItem = new FormatDataItem();
            Assert.Throws<ArgumentOutOfRangeException>(() => dataItem.SetPrice(price));
        }

        [TestCase(0)]
        [TestCase(int.MaxValue)]
        public void ManipulateFormatDataPriceSuccess(int price)
        {
            var dataItem = new FormatDataItem();
            Assert.DoesNotThrow(() => dataItem.SetPrice(price));
        }

        [TestCase(null)]
        public void ManipulateFormatDataBrandNameFail(string brandName)
        {
            var dataItem = new FormatDataItem();
            Assert.Throws<ArgumentNullException>(() => dataItem.SetBrandName(brandName));
        }

        [TestCase("")]
        [TestCase("BRNDNM")]
        public void ManipulateFormatDataBrandNameSuccess(string brandName)
        {
            var dataItem = new FormatDataItem();
            Assert.DoesNotThrow(() => dataItem.SetBrandName(brandName));
        }
    }
}
