using AsposeFormatConverter.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("AsposeFormatConverter.Tests")]
namespace AsposeFormatConverter.FormatProcessors.XML
{
    internal class XmlFormatDataItem : FormatDataItem
    {
        public XmlFormatDataItem(XmlFormatSerializationData.XmlFormatSerializationDataItem xmlSerializedDataItem)
        {
            Debug.Assert(xmlSerializedDataItem != null, $"Can't init {nameof(XmlFormatDataItem)} ctor with null");
            SetDate(xmlSerializedDataItem.Date);
            SetBrandName(xmlSerializedDataItem.BrandName);
            SetPrice(xmlSerializedDataItem.Price);
        }
    }
}
