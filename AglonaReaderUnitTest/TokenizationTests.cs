using System.Linq;
using AglonaReader.StringUtils;
using AglonaReader.Tokenization;
using NUnit.Framework;

namespace AglonaReaderUnitTest
{
    [TestFixture]
    public class TokenizationTests
    {
        [Test]
        public void Null()
        {
            var tokens = Token.Tokenize(null).ToList();
            Assert.That(tokens, Is.Empty);
        }
        
        
        [Test]
        public void Empty()
        {
            var tokens = Token.Tokenize("".Wrap()).ToList();
            Assert.That(tokens, Is.Empty);
        }
        
        
        [Test]
        public void SingleBlank()
        {
            var tokens = Token.Tokenize(" \t ".Wrap()).ToList();
            Assert.That(tokens.Count, Is.EqualTo(1));
            AssertBlank(tokens[0], 0, " \t ");
        }
        
        
        [Test]
        public void SingleWord()
        {
            var tokens = Token.Tokenize("ab".Wrap()).ToList();
            Assert.That(tokens.Count, Is.EqualTo(1));
            AssertWord(tokens[0], 0, "ab");
        }
        
        
        [Test]
        public void Sequence()
        {
            var tokens = Token.Tokenize("be  cool".Wrap()).ToList();
            
            Assert.AreEqual(3, tokens.Count);

            AssertWord(tokens[0], 0, "be");
            AssertBlank(tokens[1], 2, "  ");
            AssertWord(tokens[2], 4, "cool");
        }

        [Test]
        public void Natural()
        {
            var p = Token.NaturalDividerPosition("a, b".Wrap(), 0, true);
            Assert.That(p, Is.EqualTo(3));
        }
        
        [Test]
        public void EmDash()
        {
            var p = Token.NaturalDividerPosition("a—b".Wrap(), 0, true);
            Assert.That(p, Is.EqualTo(-1));
        }
        
        [Test]
        public void EmDashSep1()
        {
            var p = Token.NaturalDividerPosition("a— b".Wrap(), 0, true);
            Assert.That(p, Is.EqualTo(3));
        }
        
        [Test]
        public void EmDashSep2()
        {
            var p = Token.NaturalDividerPosition("a — b".Wrap(), 0, true);
            Assert.That(p, Is.EqualTo(4));
        }
        
        [Test]
        public void NaturalParenthesis()
        {
            var p = Token.NaturalDividerPosition("a (b".Wrap(), 0, true);
            Assert.That(p, Is.EqualTo(2));
        }
        
        [Test]
        public void NaturalNothing()
        {
            var p = Token.NaturalDividerPosition("a b".Wrap(), 0, true);
            Assert.That(p, Is.EqualTo(-1));
        }
        
        [Test]
        public void NaturalBeginning()
        {
            var p = Token.NaturalDividerPosition("(a) (b)".Wrap(), 0, true);
            Assert.That(p, Is.EqualTo(4));
        }
        
        private static void AssertBlank(Token token, int start, string content)
        {
            Assert.That(token, Is.InstanceOf(typeof(BlankSpace)));
            AssertStartAndContent(token, start, content);
        }
        
        
        private static void AssertWord(Token token, int start, string content)
        {
            Assert.That(token, Is.InstanceOf(typeof(Word)));
            AssertStartAndContent(token, start, content);
        }

        
        private static void AssertStartAndContent(Token token, int start, string content)
        {
            Assert.That(start, Is.EqualTo(token.Start));
            Assert.That(content, Is.EqualTo(token.Content));
        }
    }
}