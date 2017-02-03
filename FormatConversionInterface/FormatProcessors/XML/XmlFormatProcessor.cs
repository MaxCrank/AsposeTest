using AsposeFormatConverter.Base;
using AsposeFormatConverter.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AsposeFormatConverter.FormatProcessors.XML
{
    internal sealed class XmlFormatProcessor : FormatProcessorBase
    {
        private static readonly Encoding _defaultEncoding = Encoding.UTF8;
        private static readonly XmlSerializerNamespaces _emptyXmlNamespaces = new XmlSerializerNamespaces();
        private static XmlSchema _xmlSchema;

        private readonly XmlSerializer _xmlFormatSerializer = new XmlSerializer(typeof(XmlFormatSerializedData));
        private readonly XmlDocument _xmlFormatDocLoader = new XmlDocument();

        public override string Format
        {
            get
            {
                return "XML";
            }
        }

        static bool ReadXmlSchema()
        {
            if (_xmlSchema == null) try
            {
                using (StringReader schemaStringReader = new StringReader(Resources.XmlFormatSchema))
                using (XmlReader schemaXmlReader = XmlReader.Create(schemaStringReader))
                {
                    _xmlSchema = XmlSchema.Read(schemaXmlReader, (sender, args) =>
                    {
                        if (args.Severity == XmlSeverityType.Error)
                        {
                            _xmlSchema = null;
                            throw new XmlSchemaException(args.Exception?.Message ?? "Can't read XML schema");
                        }
                    });
                }

            }
            catch (Exception exception)
            {
                _xmlSchema = null;
                throw exception;
            }
            return _xmlSchema != null;
        }

        public XmlFormatProcessor() : base()
        {
            if (ReadXmlSchema())
                _xmlFormatDocLoader.Schemas.Add(_xmlSchema);
            _emptyXmlNamespaces.Add("", "");
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// Complete formatted structure bytes representation
        /// </returns>
        public override object GetData()
        {
            string outputString = string.Empty;
            XmlFormatSerializedData serializedFormatData = new XmlFormatSerializedData();
            serializedFormatData.Cars = new XmlFormatSerializedData.Car[Data.Count];
            for (int x = 0; x < Data.Count; x++)
            {
                serializedFormatData.Cars[x] = new XmlFormatSerializedData.Car(this[x]);
            }
            return serializedFormatData;
        }

        protected override bool WriteFormattedDataToStream(object data, Stream stream, string filePath)
        {
            bool result = false;
            var serializedData = data as XmlFormatSerializedData;
            if (serializedData != null) using (var xmlTextWriter = new XmlTextWriter(stream, _defaultEncoding))
                {
                    xmlTextWriter.Formatting = Formatting.Indented;
                    _xmlFormatSerializer.Serialize(xmlTextWriter, serializedData, _emptyXmlNamespaces);
                    stream.Close();
                    result = true;
                }
            else
            {
                Console.WriteLine("Can't write null-valued data to stream");
            }
            return result;
        }

        /// <summary>
        /// Parse complete formatted structure bytes representation
        /// </summary>
        /// <param name="allBytes"></param>
        /// <returns></returns>
        protected override bool ParseBytes(byte[] allBytes)
        {
            bool result = false;
            if (allBytes != null && allBytes.Length > 0)
            {
                string xml = _defaultEncoding.GetString(allBytes);
                int startIndex = xml.IndexOf('<');
                if (startIndex > 0)
                {
                    xml = xml.Substring(startIndex, xml.Length - startIndex);
                }
                _xmlFormatDocLoader.LoadXml(xml);
                _xmlFormatDocLoader.Validate(null);
                _xmlFormatDocLoader.RemoveAll();
                XmlFormatSerializedData document = null;
                using (var reader = new StringReader(xml))
                {
                    document = _xmlFormatSerializer.Deserialize(reader) as XmlFormatSerializedData;
                }
                if (document == null)
                {
                    Console.WriteLine("XML deserialization failed");
                }
                else
                {
                    foreach (var car in document.Cars)
                    {
                        AddDataItem(new XmlFormatDataItem(car), false);
                    }
                    result = true;
                }
            }
            else
            {
                Console.WriteLine("Can't parse null-valued or empty byte array to XML");
            }
            return result;
        }

        internal sealed class SpecificStringWriter : StringWriter
        {
            public override Encoding Encoding { get { return _defaultEncoding; } }
        }
    }
}
