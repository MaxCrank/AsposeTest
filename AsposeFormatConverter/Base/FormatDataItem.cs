using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("AsposeFormatConverter.Tests")]
namespace AsposeFormatConverter.Base
{
    /// <summary>
    /// Fixed data item structure for all file formats. Used to decouple data item structure handling from specific format and ease editing/conversion. 
    /// </summary>
    internal class FormatDataItem : NotifyPropertyChangedBase, IFormatDataItem
    {
        private string _brandName = string.Empty;
        private DateTime _date = new DateTime();
        private int _price = 0;

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
            if (initialDataItem == null)
            {
                throw new ArgumentNullException("Initial data item for constructor is null");
            }
            SetDate(initialDataItem.Date);
            SetBrandName(initialDataItem.BrandName);
            SetPrice(initialDataItem.Price);
        }

        public void SetBrandName(string brandName)
        {
            if (brandName == null)
            {
                throw new ArgumentNullException("Brand name should not be null");
            }
            _brandName = brandName.Substring(0, Math.Min(brandName.Length, short.MaxValue));
            OnPropertyChanged(nameof(BrandName));
        }

        public void SetDate(DateTime date)
        {
            _date = date;
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(Year));
            OnPropertyChanged(nameof(Month));
            OnPropertyChanged(nameof(Day));
        }

        public void SetDate(string date)
        {
            DateTime parsedDate;
            bool dateWasParsed = DateTime.TryParseExact(date, FormatConversionSettings.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate)
                || DateTime.TryParse(date, out parsedDate);
            if (!dateWasParsed)
            {
                throw new ArgumentException($"Date {date} should be in valid format with valid value");
            }
            _date = parsedDate;
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(Year));
            OnPropertyChanged(nameof(Month));
            OnPropertyChanged(nameof(Day));
        }

        public void SetDate(int day, int month, int year)
        {
            if (!DateIsValid(day, month, year))
            {
                throw new ArgumentException($"Date is invalid: {day}.{month}.{year}");
            }
            _date = new DateTime(year, month, day);
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(Year));
            OnPropertyChanged(nameof(Month));
            OnPropertyChanged(nameof(Day));
        }

        public void SetDay(int day)
        {
            if (!DateIsValid(day, Month, Year))
            {
                throw new ArgumentException($"Date {day}.{Month}.{Year} is invalid");
            }
            _date = new DateTime(Year, Month, day);
            OnPropertyChanged(nameof(Day));
            OnPropertyChanged(nameof(Date));
        }

        public void SetMonth(int month)
        {
            if (!DateIsValid(Day, month, Year))
            {
                throw new ArgumentException($"Date {Day}.{month}.{Year} is invalid");
            }
            _date = new DateTime(Year, month, Day);
            OnPropertyChanged(nameof(Month));
            OnPropertyChanged(nameof(Date));
        }

        public void SetYear(int year)
        {
            if (!DateIsValid(Day, Month, year))
            {
                throw new ArgumentException($"Date {Day}.{Month}.{year} is invalid");
            }
            _date = new DateTime(year, Month, Day);
            OnPropertyChanged(nameof(Year));
            OnPropertyChanged(nameof(Date));
        }

        private bool DateIsValid(int day, int month, int year)
        {
            return year > 0 && year <= 9999 && month > 0 && month <= 12 && day > 0 && DateTime.DaysInMonth(year, month) >= day;
        }

        public void SetPrice(int price)
        {
            if (price < 0)
            {
                throw new ArgumentOutOfRangeException("Price should be positive");
            }
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
