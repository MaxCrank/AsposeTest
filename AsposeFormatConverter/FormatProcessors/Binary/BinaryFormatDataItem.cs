using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AsposeFormatConverter.Base;

[assembly: InternalsVisibleTo("AsposeFormatConverter.Tests")]
namespace AsposeFormatConverter.FormatProcessors.Binary
{
    internal class BinaryFormatDataItem : FormatDataItem
    {
        public BinaryFormatDataItem(BinaryFormatSerializationData.BinaryFormatSerializationDataItem binaryFormatSerializationDataItem)
        {
            if (binaryFormatSerializationDataItem == null)
            {
                throw new ArgumentNullException($"Can't init {nameof(BinaryFormatDataItem)} ctor with null");
            }
            SetDate(binaryFormatSerializationDataItem.Day, binaryFormatSerializationDataItem.Month, binaryFormatSerializationDataItem.Year);
            SetBrandName(binaryFormatSerializationDataItem.BrandName);
            SetPrice(binaryFormatSerializationDataItem.Price);
        }
    }
}