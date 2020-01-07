using Cern.Colt.Matrix;
using Cern.Colt.Matrix.LinearAlgebra;
using NUnit.Framework;

namespace Colt.Tests
{
    [TestFixture]
    public class NormInfinityTest
    {
        [Test]
        public void TestMain()
        {
            DoubleMatrix1D x1 = DoubleFactory1D.Dense.Make(new double[] { 1.0, 2.0 });
            DoubleMatrix1D x2 = DoubleFactory1D.Dense.Make(new double[] { 1.0, -2.0 });
            DoubleMatrix1D x3 = DoubleFactory1D.Dense.Make(new double[] { -1.0, -2.0 });
            DoubleMatrix1D x4 = DoubleFactory1D.Dense.Make(new double[] { 4.0, 5.0 });

            Assert.AreEqual(2, Algebra.NormInfinity(x1));
            Assert.AreEqual(2, Algebra.NormInfinity(x2));
            Assert.AreEqual(2, Algebra.NormInfinity(x3));
            Assert.AreEqual(5, Algebra.NormInfinity(x4));
        }
    }
}
