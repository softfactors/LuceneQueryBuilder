using Microsoft.VisualStudio.TestTools.UnitTesting;

using static LuceneQueryBuilder.Util;

namespace Test
{
    [TestClass]
    public class UtilTest
    {
        [TestMethod]
        public void EscapeTermProperlyEscapes()
        {
            foreach (var c in CharsToEscape)
            {
                Assert.AreEqual($"\\{c}", EscapeTerm(c));
            }

            Assert.AreEqual(@"foo \\ bar \+ baz", EscapeTerm(@"foo \ bar + baz"));
        }
    }
}
