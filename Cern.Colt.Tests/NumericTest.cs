using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cern.Colt.Tests
{
    [Parallelizable(ParallelScope.ContextMask)]
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

            ClassicAssert.IsTrue(fb.AlmostEquals(fc, 0.02));
            ClassicAssert.IsFalse(fb.AlmostEquals(fc, 0.01));
            ClassicAssert.IsFalse(fb.AlmostEquals(fc, eps));
            ClassicAssert.IsFalse(fb.AlmostEquals(fc));

            ClassicAssert.Pass();
        }

    }
}
