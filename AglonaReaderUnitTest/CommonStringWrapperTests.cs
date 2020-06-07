using System.Text;
using AglonaReader.StringUtils;
using NUnit.Framework;

namespace AglonaReaderUnitTest
{
    [TestFixture]
    public class CommonStringWrapperTests
    {
        [Test]
        public void StringBuilderSubstring()
        {
            var sb = new StringBuilder("boar");
            Assert.AreEqual("oa", sb.Wrap().Substring(1, 2));
        }
        
        [Test]
        public void StringSubstring()
        {
            var s = "boar";
            Assert.AreEqual("oa", s.Wrap().Substring(1, 2));
        }
    }
}