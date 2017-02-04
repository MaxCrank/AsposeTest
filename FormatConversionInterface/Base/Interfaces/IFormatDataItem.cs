using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AsposeFormatConverter.Base
{
    /// <summary>
    /// Fixed data item structure interface for all file formats. Used to decouple data item structure handling from specific format and ease editing/conversion. 
    /// </summary>
    public interface IFormatDataItem : ICloneable, INotifyPropertyChanged, IDisposable
    {
        string Name { get; }

        string Date { get; }

        int Day { get; }

        int Month { get; }

        int Year { get; }

        string BrandName { get; }


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
