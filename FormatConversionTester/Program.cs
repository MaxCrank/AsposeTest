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
        //simple usage
        private static void Main(string[] args)
        {
            //create format converter
            CommonFormatConverter converter = new CommonFormatConverter();
            //populate bin format processor, add some data, save to file and convert
            using (var binProcessor = converter.CreateFormatProcessor(ConvertedFormat.BIN))
            {
                binProcessor.AddNewDataItem(10, 2, 1000, "Car", 100500);
                binProcessor[0].SetMonth(3);
                binProcessor.SaveToFile("Car.bin");
                converter.ConvertProcessor(binProcessor, "Car.xml", ConvertedFormat.XML);
                foreach (var dataItem in binProcessor)
                {
                    Console.WriteLine(dataItem.ToString());
                }
            }
            //populate xml format processor, add new data, modify existing data, save to file  and convert
            using (var xmlProcessor = converter.CreateFormatProcessor(ConvertedFormat.XML))
            {
                xmlProcessor.ReadFromFile("Car.xml");
                xmlProcessor.AddNewDataItem(15, 5, 2000, "BlaCar", 90);
                xmlProcessor[1].SetDay(20);
                xmlProcessor.SaveToFile("BlaCar.xml");
                converter.Convert("BlaCar.xml", "BlaCar.bin" , ConvertedFormat.BIN);
                foreach (var dataItem in xmlProcessor)
                {
                    Console.WriteLine(dataItem.ToString());
                }
            }
            //populate bin format processor, add new data, modify existing data, save to file  and convert
            using (var binProcessor = converter.CreateFormatProcessor(ConvertedFormat.BIN))
            {
                if (binProcessor.ReadFromFile("BlaCar.bin"))
                {
                    binProcessor.AddNewDataItem(5, 12, 2005, "BlaBlaCar", 999);
                    binProcessor[2].SetPrice(888);
                    converter.ConvertProcessor(binProcessor, "BlaBlaCar.xml", ConvertedFormat.XML);
                    foreach (var dataItem in binProcessor)
                    {
                        Console.WriteLine(dataItem.ToString());
                    }
                }
            }
            //validate data
            //press enter key to exit program
            Console.ReadLine();
            //removing test files
            File.Delete("Car.bin");
            File.Delete("Car.xml");
            File.Delete("BlaCar.bin");
            File.Delete("BlaCar.xml");
            File.Delete("BlaBlaCar.xml");
        }
    }
}
