using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsposeFormatConverter.Base
{
    internal static class FormatConversionSettings
    { 
        public const int MaxBrandNameLength = ushort.MaxValue;
        public const string DateRegex = @"^\d{2}\.\d{2}.\d{4}$";
        public const string DateFormat = "dd.mm.yyyy";
        public const string FormatOnlyRegex = @"(^[.]?\w+$){1}";
        public const string FormatFullPathRegex = @"([.]{1}\w+$){1}";
        public const string TempFileExtension = ".tmp";
        public const string BackupFileExtension = ".bak";
    }
}
