using AsposeFormatConverter.Base;
using AsposeFormatConverter.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private readonly XmlSerializer _xmlFormatSerializer = new XmlSerializer(typeof(XmlFormatSerializationData));
        private readonly XmlDocument _xmlFormatDocLoader = new XmlDocument();

        public override ConvertedFormat Format => ConvertedFormat.XML;

        private static void ReadXmlSchema()
        {
            if (_xmlSchema == null)
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
                Debug.Assert(_xmlSchema != null, "XML schema was not read");
            }
        }

        public XmlFormatProcessor()
        {
            ReadXmlSchema();
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
            return new XmlFormatSerializationData(this);
        }

        /// <inheritdoc />
        protected override void WriteFormattedDataToStream(object data, Stream stream)
        {
            Debug.Assert(data is XmlFormatSerializationData, "Can't write null or invalid data type to stream");
            Debug.Assert(stream != null, "Can't write data to null stream");
            var xmlTextWriter = new XmlTextWriter(stream, _defaultEncoding);
            xmlTextWriter.Formatting = Formatting.Indented;
            _xmlFormatSerializer.Serialize(xmlTextWriter, (XmlFormatSerializationData) data, _emptyXmlNamespaces);
            xmlTextWriter.Flush();
        }

        /// <summary>
        /// Parse complete formatted structure bytes representation
        /// </summary>
        /// <param name="allBytes"></param>
        /// <returns></returns>
        protected override bool ParseBytes(IEnumerable<byte> allBytes)
        {
            bool result = false;
            var byteArray = allBytes as byte[];
            string xml = _defaultEncoding.GetString(byteArray);
            int startIndex = xml.IndexOf('<');
            if (startIndex > 0)
            {
                xml = xml.Substring(startIndex, xml.Length - startIndex);
            }
            _xmlFormatDocLoader.LoadXml(xml);
            _xmlFormatDocLoader.Validate(null);
            _xmlFormatDocLoader.RemoveAll();
            XmlFormatSerializationData document = null;
            using (var reader = new StringReader(xml))
            {
                document = _xmlFormatSerializer.Deserialize(reader) as XmlFormatSerializationData;
            }
            if (document == null)
            {
                Console.WriteLine("XML deserialization failed");
            }
            else
            {
                ClearData();
                foreach (var car in document.XmlFormatSerializationDataItems)
                {
                    AddDataItem(new XmlFormatDataItem(car), false);
                }
                result = true;
            }
            return result;
        }

        internal sealed class SpecificStringWriter : StringWriter
        {
            public override Encoding Encoding => _defaultEncoding;
        }
    }
}
