using NUnit.Framework;
using System;

namespace SampleTest.Tests
{
    [TestFixture(1)]
    [TestFixture(2)]
    [TestFixture(3)]
    public class TestsFixture
    {
        private int value;

        public TestsFixture(int value)
        {
            this.value = value;
        }
        [Test]
        public void TestValue()
        {
            Console.WriteLine(value);
            Assert.NotZero(value);
        }
    }

    [TestFixtureSource(nameof(FixtureArgs))]
    public class TestsFixtureSource
    {
        public static object[] FixtureArgs = { 1, 2, 3 };

        private int value;

        public TestsFixtureSource(int value)
        {
            this.value = value;
        }
        [Test]
        public void TestValue()
        {
            Console.WriteLine(value);
            Assert.NotZero(value);
        }
    }

    [TestFixtureSource(typeof(TestsFixtureSource), nameof(TestsFixtureSource.FixtureArgs))]
    public class TestsFixtureSourceAnother
    {
        private int value;

        public TestsFixtureSourceAnother(int value)
        {
            this.value = value;
        }
        [Test]
        public void TestValue()
        {
            Console.WriteLine(value);
            Assert.NotZero(value);
        }
    }

    [TestFixture("hello", "hello", "goodbye")]
    [TestFixture("zip", "zip")]
    [TestFixture(42, 42, 99)]
    [TestFixture('a', 'a', 'b')]
    //[TestFixture('A', 'A')]
    public class TestsFixtureParameterized
    {
        private readonly string _eq1;
        private readonly string _eq2;
        private readonly string _neq;
        private readonly int _type;

        public TestsFixtureParameterized(string eq1, string eq2, string neq)
        {
            _eq1 = eq1;
            _eq2 = eq2;
            _neq = neq;
            _type = 1;
        }

        public TestsFixtureParameterized(string eq1, string eq2)
            : this(eq1, eq2, null) { _type = 2; }

        public TestsFixtureParameterized(int eq1, int eq2, int neq)
        {
            _eq1 = eq1.ToString();
            _eq2 = eq2.ToString();
            _neq = neq.ToString();
            _type = 3;
        }

        // params is not supported in the ricaun.NUnit
        // Can use params arguments (but not yet optional arguments)
        public TestsFixtureParameterized(params char[] eqArguments)
        {
            _eq1 = eqArguments[0].ToString();
            _eq2 = eqArguments[1].ToString();
            if (eqArguments.Length > 2)
                _neq = eqArguments[2].ToString();
            else
                _neq = null;
            _type = 4;
        }

        [Test]
        public void Test()
        {
            Console.WriteLine(_type);
        }

        [Test]
        public void TestEquality()
        {
            Assert.That(_eq2, Is.EqualTo(_eq1));
            Assert.That(_eq2.GetHashCode(), Is.EqualTo(_eq1.GetHashCode()));
        }

        [Test]
        public void TestInequality()
        {
            Assert.That(_neq, Is.Not.EqualTo(_eq1));
            if (_neq != null)
            {
                Assert.That(_neq.GetHashCode(), Is.Not.EqualTo(_eq1.GetHashCode()));
            }
        }
    }
}