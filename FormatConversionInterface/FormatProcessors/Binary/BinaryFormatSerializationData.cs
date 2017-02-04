using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AsposeFormatConverter.Base;

namespace AsposeFormatConverter.FormatProcessors.Binary
{
    internal class BinaryFormatSerializationData
    {
        public short Header => 0x2526;

        public uint RecordsCount { get; }

        public List<BinaryFormatSerializationDataItem> DataItems { get; }

        public BinaryFormatSerializationData(IFormatProcessor formatProcessor)
        {
            Debug.Assert(formatProcessor != null, $"Can't init {nameof(BinaryFormatSerializationData)} with null");
            RecordsCount = (uint)formatProcessor.Data.Count;
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
            return binaryData.ToArray();
        }

        internal class BinaryFormatSerializationDataItem
        {
            public string Date { get; }

            public ushort BrandNameLength => (ushort)(BrandName?.Length ?? 0);

            public string BrandName { get; }

            public int Price { get; }

            public BinaryFormatSerializationDataItem(IFormatDataItem formatDataItem)
            {
                Debug.Assert(formatDataItem != null, "Can't init binary BinaryFormatSerializationDataItem constructor with null");
                Date = formatDataItem.Date;
                BrandName = formatDataItem.BrandName;
                Price = formatDataItem.Price;
            }

            public byte[] GetBinaryData()
            {
                List<byte> binaryData = new List<byte>();
                binaryData.AddRange(Encoding.UTF8.GetBytes(Date));
                binaryData.AddRange(BitConverter.GetBytes(BrandNameLength));
                binaryData.AddRange(Encoding.Unicode.GetBytes(BrandName));
                binaryData.AddRange(BitConverter.GetBytes(Price));
                return binaryData.ToArray();
            }
        }
    }
}
