using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using AsposeFormatConverter.Base;

namespace AsposeFormatConverter.FormatProcessors.Binary
{
    internal class BinaryFormatSerializationData
    {
        public static int MinSize => sizeof(short) + sizeof(int);

        public static short Header => 0x2526;

        public int RecordsCount { get; private set; }

        public List<BinaryFormatSerializationDataItem> DataItems { get; private set; }

        private BinaryFormatSerializationData()
        {
            
        }

        public BinaryFormatSerializationData(IFormatProcessor formatProcessor)
        {
            Debug.Assert(formatProcessor != null, $"Can't init {nameof(BinaryFormatSerializationData)} with null");
            RecordsCount = formatProcessor.Data.Count;
            DataItems = new List<BinaryFormatSerializationDataItem>();
            foreach (var dataitem in formatProcessor.Data)
            {
                DataItems.Add(new BinaryFormatSerializationDataItem(dataitem));
            }
        }

        public byte[] GetBytes()
        {
            List<byte> binaryData = new List<byte>();
            binaryData.AddRange(BitConverter.GetBytes(Header));
            binaryData.AddRange(BitConverter.GetBytes(RecordsCount));
            DataItems.ForEach(i => binaryData.AddRange(i.GetBinaryData()));
            Debug.Assert(binaryData.Count >= MinSize, $"{nameof(BinaryFormatSerializationData)} binary size is less than minimum");
            return binaryData.ToArray();
        }

        public static bool TryParseBytes(IEnumerable<byte> allBytes, out BinaryFormatSerializationData binaryFormatSerializationData)
        {
            bool result = false;
            binaryFormatSerializationData = null;
            if (allBytes != null)
            {
                var byteArray = allBytes.ToArray();
                if (byteArray.Length >= MinSize)
                {
                    BinaryFormatSerializationData resultData;
                    result = TryParseBytesManually(byteArray, out resultData);
                    if (result)
                    {
                        binaryFormatSerializationData = resultData;
                    }
                }
            }
            if (!result)
            {
                throw new FormatException("Binary validation failed: format is invalid");
            }
            return result;
        }

        private static bool TryParseBytesManually(byte[] allBytes, out BinaryFormatSerializationData binaryFormatSerializationData)
        {
            Debug.Assert(allBytes != null, "Internal binary parse bytes pre-check for null failed");
            bool result = false;
            BinaryFormatSerializationData tempData = new BinaryFormatSerializationData();
            if (Header == BitConverter.ToInt16(allBytes, 0))
            {
                tempData.RecordsCount = BitConverter.ToUInt16(allBytes, sizeof(ushort));
                tempData.DataItems = new List<BinaryFormatSerializationDataItem>();
                int parsedItemsSize = MinSize;
                if (tempData.RecordsCount > 0) for (int x = 0; x < tempData.RecordsCount; x++)
                {
                    int itemSize;
                    BinaryFormatSerializationDataItem serializationDataItem;
                    if (BinaryFormatSerializationDataItem.TryParseBinarySerializationDataItem(allBytes, parsedItemsSize, 
                        out serializationDataItem, out itemSize))
                    {
                        tempData.DataItems.Add(serializationDataItem);
                        parsedItemsSize += itemSize;
                        if (x == tempData.RecordsCount - 1)
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        tempData = null;
                        break;
                    }
                }
                else
                {
                    result = true;
                }
            }
            binaryFormatSerializationData = tempData;
            return result;
        }

        internal class BinaryFormatSerializationDataItem
        {
            public const int DateSize = 8;

            public static int MinSize => DateSize + sizeof(short) + sizeof(int);

            public short Day { get; private set; }

            public short Month { get; private set; }

            public int Year { get; private set; }

            public short BrandNameLength => (short)(BrandName?.Length ?? 0);

            public string BrandName { get; private set; }

            public int Price { get; private set; }

            private BinaryFormatSerializationDataItem()
            {

            }

            public BinaryFormatSerializationDataItem(IFormatDataItem formatDataItem)
            {
                Debug.Assert(formatDataItem != null, $"Can't init binary {nameof(BinaryFormatSerializationDataItem)} constructor with null");
                Day = (short)formatDataItem.Day;
                Month = (short)formatDataItem.Month;
                Year = formatDataItem.Year;
                BrandName = formatDataItem.BrandName;
                Price = formatDataItem.Year;
            }

            public byte[] GetBinaryData()
            {
                List<byte> binaryData = new List<byte>();
                binaryData.AddRange(BitConverter.GetBytes(Day));
                binaryData.AddRange(BitConverter.GetBytes(Month));
                binaryData.AddRange(BitConverter.GetBytes(Year));
                binaryData.AddRange(BitConverter.GetBytes(BrandNameLength));
                binaryData.AddRange(Encoding.Unicode.GetBytes(BrandName));
                binaryData.AddRange(BitConverter.GetBytes(Price));
                Debug.Assert(binaryData.Count >= MinSize, $"{nameof(BinaryFormatSerializationDataItem)} binary size is less than minimum");
                return binaryData.ToArray();
            }

            public static bool TryParseBinarySerializationDataItem(byte[] allBytes, int arrayPosition, 
                out BinaryFormatSerializationDataItem serializationDataitem, out int size)
            {
                bool result = false;
                size = 0;
                serializationDataitem = null;
                if (allBytes.Length >= arrayPosition + MinSize)
                {
                    var brandNameLength = BitConverter.ToInt16(allBytes, arrayPosition + DateSize);
                    var supposedSize = MinSize + brandNameLength * 2;
                    if (allBytes.Length >= arrayPosition + supposedSize)
                    {
                        size = supposedSize;
                        serializationDataitem = new BinaryFormatSerializationDataItem();
                        serializationDataitem.Day = BitConverter.ToInt16(allBytes, arrayPosition);
                        serializationDataitem.Month = BitConverter.ToInt16(allBytes, arrayPosition + sizeof(short));
                        serializationDataitem.Year = BitConverter.ToInt32(allBytes, arrayPosition + sizeof(short) * 2);
                        serializationDataitem.BrandName = Encoding.Unicode.GetString(allBytes, arrayPosition + DateSize + sizeof(short), brandNameLength * 2);
                        Debug.Assert(brandNameLength == serializationDataitem.BrandNameLength,
                            $"{nameof(BinaryFormatSerializationDataItem)} byte parser has bug related to {nameof(BrandName)} and/or {nameof(BrandNameLength)} length");
                        serializationDataitem.Price = BitConverter.ToInt32(allBytes, arrayPosition + (MinSize - sizeof(int)) + brandNameLength * 2);
                        result = true;
                    }
                }
                return result;
            }
        }
    }
}
