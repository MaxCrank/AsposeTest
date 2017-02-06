using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsposeFormatConverter.Base;
using System.IO;

namespace FormatConversionTester
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CommonFormatConverter converter = new CommonFormatConverter();
            using (var binConverter = converter.CreateFormatProcessor(ConvertedFormat.BIN))
            {
                binConverter.AddNewDataItem(10, 2, 1000, "Car", 100500);
                binConverter.SaveToFile("Car.bin");
                converter.Convert(binConverter, "Car.xml", ConvertedFormat.XML);
            }
            using (var xmlConverter = converter.CreateFormatProcessor(ConvertedFormat.XML))
            {
                xmlConverter.ReadFromFile("Car.xml");
                xmlConverter.AddNewDataItem(15, 5, 2000, "BlaCar", 90);
                xmlConverter.SaveToFile("BlaCar.xml");
                converter.Convert("BlaCar.xml", "BlaCar.bin" , ConvertedFormat.BIN);
            }
            using (var binConverter = converter.CreateFormatProcessor(ConvertedFormat.BIN))
            {
                if (binConverter.ReadFromFile("BlaCar.bin"))
                {
                    binConverter.AddNewDataItem(5, 12, 2005, "BlaBlaCar", 999);
                    converter.Convert(binConverter, "BlaBlaCar.xml", ConvertedFormat.XML);
                }
            }
        }
    }
}
