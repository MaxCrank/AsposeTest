using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FormatConversion.Base
{
    /// <summary>
    /// Fixed data item structure interface for all file formats. Used to decouple data item structure handling from specific format and ease editing/conversion. 
    /// </summary>
    public interface IFormatDataItem : ICloneable, INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Logical name which defines the purpose of the data
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Date should be in valid dd.mm.yyyy format
        /// </summary>
        string Date { get; }

        int Day { get; }

        int Month { get; }

        int Year { get; }

        /// <summary>
        /// BrandName length should not be more than 2 bytes (ushort) max positive value
        /// </summary>
        string BrandName { get; }

        /// <summary>
        /// Price should be >= 0
        /// </summary>
        int Price { get; }

        void SetDate(string date);

        void SetDate(DateTime date);

        void SetDate(int year, int month, int day);

        void SetDay(int day);

        void SetMonth(int month);

        void SetYear(int year);

        void SetBrandName(string brandName);

        void SetPrice(int price);
    }
}
