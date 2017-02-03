using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsposeFormatConverter.Base;
using System.IO;

namespace FormatConversionTester
{
    class Program
    {
        static void Main(string[] args)
        {
            CommonFormatConverter converter = null;
            try
            {
                converter = new CommonFormatConverter();
            }
            catch(Exception ex)
            {
                if (!(ex is FileNotFoundException))
                {
                    throw ex;
                }
            }
            string filePath = "CarTest.xml";
            string format;
            if (converter.TryGetSupportedFormatFromPath(filePath, out format))
            {
                var formatProcessor = converter.CreateFormatProcessor(format);
                if (formatProcessor.ReadFromFile(filePath))
                {
                    foreach(var dataItem in formatProcessor.Data)
                    {
                        Console.WriteLine(dataItem.ToString());
                    }
                }
                formatProcessor.SaveToFile("CarTestNew.xml");
            }

        }
    }
}
