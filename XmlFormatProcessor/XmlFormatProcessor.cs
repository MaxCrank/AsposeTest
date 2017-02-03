using FormatConversion.Base;
using FormatConversion.XmlFormatProcessor.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace FormatConversion.XmlFormatProcessor
{
    public sealed class XmlFormatProcessor : FormatProcessorBase
    {
        private readonly Encoding _defaultEncoding = Encoding.UTF8;

        static XmlSchema _xmlSchema;

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
            catch (Exception ex)
            {
                _xmlSchema = null;
                throw ex;
            }
            return _xmlSchema != null;
        }

        private void AddDataItemNode(IFormatDataItem dataItem, XmlDocument xmlDoc, XmlElement rootNode)
        {
            if (dataItem != null && xmlDoc != null && rootNode != null)
            {
                var carNode = xmlDoc.CreateElement("Car");
                var dateNode = xmlDoc.CreateElement("Date");
                dateNode.Value = dataItem.Date;
                carNode.AppendChild(dateNode);
                var brandNameNode = xmlDoc.CreateElement("BrandName");
                brandNameNode.Value = dataItem.BrandName;
                carNode.AppendChild(brandNameNode);
                var priceNode = xmlDoc.CreateElement("Price");
                priceNode.Value = dataItem.Price.ToString();
                carNode.AppendChild(priceNode);
                rootNode.AppendChild(carNode);
            }
            else
            {
                throw new ArgumentNullException("Can't add data item node, one of the arguments is null");
            }
        }

        public override byte[] GetData()
        {
            byte[] result = null;
            if (ReadXmlSchema())
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(Resources.XmlFormatTemplate);
                xmlDoc.Schemas.Add(_xmlSchema);
                var carsRootNode = xmlDoc.GetElementById("Document");
                foreach(var dataItem in Data)
                {
                    AddDataItemNode(dataItem, xmlDoc, carsRootNode);
                }
                xmlDoc.Validate(null);
                var declaration = xmlDoc.FirstChild as XmlDeclaration;
                if (declaration != null && Encoding.GetEncoding(declaration.Encoding) == _defaultEncoding)
                {
                    result = _defaultEncoding.GetBytes(xmlDoc.OuterXml);
                }
                else
                {
                    Console.WriteLine("Can't create XML bytes data representation - XML template encoding differs from default");
                }
            }
            else
            {
                Console.WriteLine("Can't create XML bytes data representation - XML schema can't be read");
            }
            return result;
        }

        private IFormatDataItem ParseDataItem(XmlNode dataItemNode)
        {
            if (dataItemNode == null)
            {
                
            }
            else
            {

            }
            throw new NotImplementedException();
        }

        protected override bool ParseBytes(byte[] allBytes)
        {
            bool result = false;
            if (allBytes != null && allBytes.Length > 0)
            {
                if (ReadXmlSchema())
                {
                    string xml = _defaultEncoding.GetString(allBytes);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(Resources.XmlFormatTemplate);
                    xmlDoc.Schemas.Add(_xmlSchema);
                    xmlDoc.Validate(null);
                    var carsRootNode = xmlDoc.GetElementById("Document");
                    if (carsRootNode != null && carsRootNode.ChildNodes.Count > 0)
                        for (int x = 0; x < carsRootNode.ChildNodes.Count; x++)
                        {
                            //carsRootNode.ChildNodes.Item
                        }
                }
                else
                {
                    Console.WriteLine("Can't parse null-valued or empty byte array");
                }
            }
            return result;
        }
    }
}
