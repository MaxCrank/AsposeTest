using AsposeFormatConverter.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsposeFormatConverter.Tests
{
    public class SignificantFormatsTestData
    {
        public static IEnumerable<ConvertedFormat> TestCases
        {
            get
            {
                foreach (ConvertedFormat format in Enum.GetValues(typeof(ConvertedFormat)))
                {
                    if (format != ConvertedFormat.UNKNOWN)
                    {
                        yield return format;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
    }
}
