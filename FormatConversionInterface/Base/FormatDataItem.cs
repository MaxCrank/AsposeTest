using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AsposeFormatConverter.Base
{
    /// <summary>
    /// Fixed data item structure for all file formats. Used to decouple data item structure handling from specific format and ease editing/conversion. 
    /// </summary>
    internal class FormatDataItem : NotifyPropertyChangedBase, IFormatDataItem
    {
        private string _brandName = string.Empty;
        private DateTime _date = new DateTime(1970, 1, 1);
        private int _price = 0;

        /// <summary>
        /// Logical name which defines the purpose of the data
        /// </summary>
        public string Name => "Car";

        /// <summary>
        /// BrandName length is 2 bytes (ushort) max positive value
        /// </summary>
        public string BrandName => _brandName;

        /// <summary>
        /// Date is in dd.mm.yyyy format
        /// </summary>
        public string Date => _date.ToString(FormatConversionSettings.DateFormat);

        public int Day => _date.Day;

        public int Month => _date.Month;

        public int Year => _date.Year;

        /// <summary>
        /// Value is always positive
        /// </summary>
        public int Price => _price;

        public FormatDataItem()
        {

        }

        public FormatDataItem(FormatDataItem initialDataItem)
        {
            Debug.Assert(initialDataItem != null, "Initial data item for constructor is null");
            SetDate(initialDataItem.Date);
            SetBrandName(initialDataItem.BrandName);
            SetPrice(initialDataItem.Price);
        }

        public void SetBrandName(string brandName)
        {
            Debug.Assert(!string.IsNullOrEmpty(brandName), "Brand name should not be empty");
            _brandName = brandName.Substring(0, Math.Min(brandName.Length, ushort.MaxValue));
            OnPropertyChanged(nameof(BrandName));
        }

        public void SetDate(DateTime date)
        {
            Debug.Assert(DateIsValid(date.Year, date.Month, date.Day), "Date is invalid");
            _date = date;
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(Year));
            OnPropertyChanged(nameof(Month));
            OnPropertyChanged(nameof(Day));
        }

        public void SetDate(string date)
        {
            Debug.Assert(!string.IsNullOrEmpty("Date string is null or empty"));
            DateTime parsedDate;
            Debug.Assert(DateTime.TryParseExact(date, FormatConversionSettings.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate)
                || DateTime.TryParse(date, out parsedDate), $"Date should be in valid format");
            _date = parsedDate;
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(Year));
            OnPropertyChanged(nameof(Month));
            OnPropertyChanged(nameof(Day));
        }

        public void SetDate(int year, int month, int day)
        {
            Debug.Assert(DateIsValid(year, month, day), "Date is invalid");
            _date = new DateTime(year, month, day);
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(Year));
            OnPropertyChanged(nameof(Month));
            OnPropertyChanged(nameof(Day));
        }

        public void SetDay(int day)
        {
            Debug.Assert(DateIsValid(Year, Month, day), "Date is invalid");
            _date = new DateTime(Year, Month, day);
            OnPropertyChanged(nameof(Day));
            OnPropertyChanged(nameof(Date));
        }

        public void SetMonth(int month)
        {
            Debug.Assert(DateIsValid(Year, month, Day), "Date is invalid");
            _date = new DateTime(Year, month, Day);
            OnPropertyChanged(nameof(Month));
            OnPropertyChanged(nameof(Date));
        }

        public void SetYear(int year)
        {
            Debug.Assert(DateIsValid(year, Month, Day), "Date is invalid");
            _date = new DateTime(year, Month, Day);
            OnPropertyChanged(nameof(Year));
            OnPropertyChanged(nameof(Date));
        }

        private bool DateIsValid(int year, int month, int day)
        {
            return year > 0 && year <= 9999 && month > 0 && month <= 12 && day > 0 && DateTime.DaysInMonth(year, month) >= day;
        }

        public void SetPrice(int price)
        {
            Debug.Assert(price >= 0, "Price should be positive");
            _price = price;
            OnPropertyChanged(nameof(Price));
        }

        public object Clone()
        {
            return new FormatDataItem(this);
        }

        public override string ToString()
        {
            return $"Date: {Date}; BrandName: {BrandName}; Price {Price}";
        }
    }
}
