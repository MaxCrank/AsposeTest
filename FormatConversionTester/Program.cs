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
            string filePath = "CarTest.xml";
            ConvertedFormat format;
            if (converter.TryGetSupportedFormatFromPath(filePath, out format))
            {
                using (var formatProcessor = converter.CreateFormatProcessor(format))
                {
                    if (formatProcessor.ReadFromFile(filePath))
                    {
                        foreach (var dataItem in formatProcessor.Data)
                        {
                            Console.WriteLine(dataItem.ToString());
                        }
                    }
                    formatProcessor.SaveToFile("CarTestNew.xml");
                }
                Console.ReadLine();
            }
        }
    }
}
