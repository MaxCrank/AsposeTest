using AsposeFormatConverter.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsposeFormatConverter.FormatProcessors.XML
{
    internal class XmlFormatDataItem : FormatDataItem
    {
        public XmlFormatDataItem(XmlFormatSerializedData.Car xmlSerializedDataItem)
        {
            if (xmlSerializedDataItem != null)
            {
                SetDate(xmlSerializedDataItem.Date);
                SetBrandName(xmlSerializedDataItem.BrandName);
                SetPrice(xmlSerializedDataItem.Price);
            }
        }
    }
}
