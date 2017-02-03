using AsposeFormatConverter.Base;
using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override bool ParseBytes(byte[] allBytes)
        {
            throw new NotImplementedException();
        }
    }
}
