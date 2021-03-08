using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cern.Colt.Tests
{
    public class NumericTest
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AlmostEqualsTest()
        {
            double fb = 0.55;
            double fc = 0.56;
            double eps = 0.001;

            Assert.IsTrue(fb.AlmostEquals(fc, 0.02));
            Assert.IsFalse(fb.AlmostEquals(fc, 0.01));
            Assert.IsFalse(fb.AlmostEquals(fc, eps));
            Assert.IsFalse(fb.AlmostEquals(fc));

            Assert.Pass();
        }

    }
}
