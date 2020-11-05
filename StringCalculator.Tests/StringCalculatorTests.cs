using NUnit.Framework;
using System;

namespace StringCalculator.Tests
{
    [TestFixture]
    public class StringCalculatorTests
    {
        [TestCase("", 0)]
        [TestCase("1", 1)]
        [TestCase("1,2", 3)]
        [TestCase("1\n,2", 3)]
        [TestCase("\n1,2", 3)]
        [TestCase("1\n\n\n,2\n\n", 3)]
        [TestCase("1,2,1001", 3)]
        [TestCase("999, 1000, 1001", 999)]
        public void TestAdd_SimpleInputs(string input, int expectedResult)
        {
            var result = StringCalculator.Add(input);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase("//;\n1;2", 3)]
        [TestCase("//*;\n1;2*3*4;5", 15)]
        [TestCase("//-\n1-2", 3)]
        [TestCase("//*;\n\n\n\n\n\n\n\n\n\n\n\n1;2", 3)]
        public void TestAdd_InputsWithHeader(string input, int expectedResult)
        {
            var result = StringCalculator.Add(input);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase(null)]
        [TestCase("1,\n")]
        public void TestAdd_Throws_ArgNull(string input)
        {
            Assert.Throws<ArgumentNullException>(() => StringCalculator.Add(input));
        }

        [TestCase("////////;\n1;2", "Invalid delimiter header specified: '/'")]
        [TestCase("////////;//\n1;2", "Invalid delimiter header specified: '/'")]
        public void TestAdd_Throws_ArgOutOfRange_InvalidDelims(string input, string expectedResult)
        {
            Assert.That(() => StringCalculator.Add(input),
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Message.Contains(expectedResult));
        }

        [TestCase("-1", "Negatives not allowed: -1")]
        [TestCase("1,2,-5,4,-8,-3,-3", "Negatives not allowed: -5,-8,-3,-3")]
        public void TestAdd_Throws_ArgOutOfRange_NegativeAudit(string input, string expectedResult)
        {
            Assert.That(() => StringCalculator.Add(input),
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Message.Contains(expectedResult));
        }
    }
}