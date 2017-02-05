using System.Diagnostics;
using AsposeFormatConverter.Base;

namespace AsposeFormatConverter.FormatProcessors.Binary
{
    internal class BinaryFormatDataItem : FormatDataItem
    {
        public BinaryFormatDataItem(BinaryFormatSerializationData.BinaryFormatSerializationDataItem binaryFormatSerializationDataItem)
        {
            Debug.Assert(binaryFormatSerializationDataItem != null, $"Can't init {nameof(BinaryFormatDataItem)} ctor with null");

        }
    }
}