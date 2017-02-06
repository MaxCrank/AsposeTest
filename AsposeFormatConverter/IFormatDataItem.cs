using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormatConversion.Interfaces
{
    public interface IFormatDataItem
    {
        string Date { get; }
        string BrandName { get; }
        int Price { get; }

        void SetDate(string date);
        void SetDate(DateTime date);
        void SetDate(int day, int month, int year);
        void SetDay(int day);
        void SetMonth(int month);
        void SetYear(int year);
        void SetBrandName(string brandName);
        void SetPrice(int price);
    }
}
