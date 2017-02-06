using AsposeFormatConverter.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AsposeFormatConverter.Properties;

namespace AsposeFormatConverter.FormatProcessors.Binary
{
    internal sealed class BinaryFormatProcessor : FormatProcessorBase
    {
        /// <inheritdoc />
        public override ConvertedFormat Format => ConvertedFormat.BIN;

        /// <inheritdoc />
        public override object GetData()
        {
            var binaryData = new BinaryFormatSerializationData(this);
            Debug.Assert(binaryData != null, $"{nameof(BinaryFormatSerializationData)} generation failed");
            return binaryData.GetBytes();
        }

        /// <inheritdoc />
        protected override bool ParseBytes(IEnumerable<byte> allBytes)
        {
            bool result = false;
            BinaryFormatSerializationData binaryFormatSerializationData;
            if (BinaryFormatSerializationData.TryParseBytes(allBytes, out binaryFormatSerializationData))
            {
                ClearData();
                foreach (var binaryDataItem in binaryFormatSerializationData.DataItems)
                {
                    AddDataItem(new BinaryFormatDataItem(binaryDataItem), false);
                }
                result = true;
            }
            return result;
        }
    }
}
