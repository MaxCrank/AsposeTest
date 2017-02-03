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
    [XmlInclude(typeof(XmlFormatSerializedData.Car))]
    public class XmlFormatSerializedData
    {
        [XmlElement("Car")]
        public Car[] Cars;

        public XmlFormatSerializedData()
        {

        }

        [Serializable]
        public class Car
        {
            [XmlElement("Date")]
            public string Date { get; set; }

            [XmlElement("BrandName")]
            public string BrandName { get; set; }

            [XmlElement("Price")]
            public int Price { get; set; }

            public Car()
            {

            }

            public Car(IFormatDataItem formatDataItem)
            {
                Debug.Assert(formatDataItem != null, "Can't init Car ctor with null");
                Date = formatDataItem.Date;
                BrandName = formatDataItem.BrandName;
                Price = formatDataItem.Price;
            }
        }
    }
}
