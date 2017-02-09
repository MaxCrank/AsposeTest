using NUnit.Framework;
using AsposeFormatConverter.Base;
using System;
using System.IO;

namespace AsposeFormatConverter.Tests
{
    [SetUpFixture()]
    public class NUnitSetUp
    {
        public NUnitSetUp()
        {

        }

        [OneTimeSetUp()]
        public void SetUpTests()
        {
            var dir = Path.GetDirectoryName(new Uri(typeof(CommonFormatConverter).Assembly.CodeBase).LocalPath);
            Directory.SetCurrentDirectory(dir);
        }

        [OneTimeTearDown()]
        public void FinalizeTests()
        {
            
        }
    }
}
