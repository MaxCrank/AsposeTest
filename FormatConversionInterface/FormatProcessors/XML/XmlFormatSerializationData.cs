using AsposeFormatConverter.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace AsposeFormatConverter.FormatProcessors.XML
{
    [Serializable]
    [XmlRoot(ElementName = "Document", Namespace = "", IsNullable = false)]
    [XmlInclude(typeof(XmlFormatSerializationDataItem))]
    public class XmlFormatSerializationData
    {
        [XmlElement("Car")]
        public List<XmlFormatSerializationDataItem> XmlFormatSerializationDataItems { get; set; }

        public XmlFormatSerializationData()
        {
            
        }

        public XmlFormatSerializationData(IFormatProcessor formatProcessor)
        {
            Debug.Assert(formatProcessor != null, $"Can't init {nameof(XmlFormatSerializationData)} with null");
            XmlFormatSerializationDataItems = new List<XmlFormatSerializationDataItem>();
            foreach (var dataItem in formatProcessor.Data)
            {
                XmlFormatSerializationDataItems.Add(new XmlFormatSerializationDataItem(dataItem));
            }
        }

        [Serializable]
        public class XmlFormatSerializationDataItem
        {
            [XmlElement("Date")]
            public string Date { get; set; }

            [XmlElement("BrandName")]
            public string BrandName { get; set; }

            [XmlElement("Price")]
            public int Price { get; set; }

            public XmlFormatSerializationDataItem()
            {

            }

            public XmlFormatSerializationDataItem(IFormatDataItem formatDataItem)
            {
                Debug.Assert(formatDataItem != null, $"Can't init {nameof(XmlFormatSerializationDataItem)} ctor with null");
                Date = formatDataItem.Date;
                BrandName = formatDataItem.BrandName;
                Price = formatDataItem.Price;
            }
        }
    }
}
