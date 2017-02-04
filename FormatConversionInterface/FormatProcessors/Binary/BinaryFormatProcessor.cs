using AsposeFormatConverter.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
        protected override bool ParseBytes(byte[] allBytes)
        {
            throw new NotImplementedException();
        }
    }
}
