using System;
using System.Collections.Generic;
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
        public string Name
        {
            get
            {
                return "Car";
            }
        }

        /// <summary>
        /// BrandName length is 2 bytes (ushort) max positive value
        /// </summary>
        public virtual string BrandName
        {
            get
            {
                return _brandName;
            }
        }

        /// <summary>
        /// Date is in dd.mm.yyyy format
        /// </summary>
        public string Date
        {
            get
            {
                return _date.ToString(FormatConversionSettings.DateFormat);
            }
        }

        public int Day
        {
            get
            {
                return _date.Day;
            }
        }

        public int Month
        {
            get
            {
                return _date.Month;
            }
        }

        public int Year
        {
            get
            {
                return _date.Year;
            }
        }

        /// <summary>
        /// Value is always positive
        /// </summary>
        public int Price
        {
            get
            {
                return _price;
            }
        }

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
            if (string.IsNullOrEmpty(brandName))
            {
                Console.WriteLine("Brand name should not be empty");
            }
            else
            {
                _brandName = brandName.Substring(0, Math.Min(brandName.Length, ushort.MaxValue));
                OnPropertyChanged(nameof(BrandName));
            }
        }

        public void SetDate(DateTime date)
        {
            if (DateIsValid(date.Year, date.Month, date.Day))
            {
                _date = date;
                OnPropertyChanged(nameof(Date));
                OnPropertyChanged(nameof(Year));
                OnPropertyChanged(nameof(Month));
                OnPropertyChanged(nameof(Day));
            }
            else
            {
                Console.WriteLine("Date is invalid");
            }
        }

        public void SetDate(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                Console.WriteLine("Date string is null or empty");
            }
            else
            {
                DateTime parsedDate;
                if (DateTime.TryParseExact(date, FormatConversionSettings.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                {
                    _date = parsedDate;
                    OnPropertyChanged(nameof(Date));
                    OnPropertyChanged(nameof(Year));
                    OnPropertyChanged(nameof(Month));
                    OnPropertyChanged(nameof(Day));
                }
                else
                {
                    Console.WriteLine($"Date should be in valid {FormatConversionSettings.DateFormat} format");
                }
            }
        }

        public void SetDate(int year, int month, int day)
        {
            if (DateIsValid(year, month, day))
            {
                _date = new DateTime(year, month, day);
                OnPropertyChanged(nameof(Date));
                OnPropertyChanged(nameof(Year));
                OnPropertyChanged(nameof(Month));
                OnPropertyChanged(nameof(Day));
            }
            else
            {
                Console.WriteLine("Date is invalid");
            }
        }

        public void SetDay(int day)
        {
            if (DateIsValid(Year, Month, day))
            {
                _date = new DateTime(Year, Month, day);
                OnPropertyChanged(nameof(Day));
                OnPropertyChanged(nameof(Date));
            }
            else
            {
                Console.WriteLine("Date is invalid");
            }
        }

        public void SetMonth(int month)
        {
            if (DateIsValid(Year, month, Day))
            {
                _date = new DateTime(Year, month, Day);
                OnPropertyChanged(nameof(Month));
                OnPropertyChanged(nameof(Date));
            }
            else
            {
                Console.WriteLine("Date is invalid");
            }
        }

        public void SetYear(int year)
        {
            if (DateIsValid(year, Month, Day))
            {
                _date = new DateTime(year, Month, Day);
                OnPropertyChanged(nameof(Year));
                OnPropertyChanged(nameof(Date));
            }
            else
            {
                Console.WriteLine("Date is invalid");
            }
        }

        bool DateIsValid(int year, int month, int day)
        {
            return year > 0 && year <= 9999 && month > 0 && month <= 12 && day > 0 && DateTime.DaysInMonth(year, month) >= day;
        }

        public void SetPrice(int price)
        {
            if (price < 0)
            {
                Console.WriteLine("Price shouldn't be negative");
            }
            else
            {
                _price = price;
                OnPropertyChanged(nameof(Price));
            }
        }

        public virtual object Clone()
        {
            return new FormatDataItem(this);
        }

        public override string ToString()
        {
            return $"Date: {Date}; BrandName: {BrandName}; Price {Price}";
        }
    }
}
