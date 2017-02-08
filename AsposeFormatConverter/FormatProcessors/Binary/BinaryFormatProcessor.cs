﻿using AsposeFormatConverter.Base;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AsposeFormatConverter.Tests")]
namespace AsposeFormatConverter.FormatProcessors.Binary
{
    internal sealed class BinaryFormatProcessor : FormatProcessorBase
    {
        /// <inheritdoc />
        public override ConvertedFormat Format { get { return ConvertedFormat.BIN; } }

        /// <inheritdoc />
        public override object GetData()
        {
            CheckCacheException();
            var binaryData = new BinaryFormatSerializationData(this);
            if (binaryData == null)
            {
                throw new Exception("BinaryFormatSerializationData generation failed");
            }
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
