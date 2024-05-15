using Cern.Colt.Matrix;
using Cern.Colt.Matrix.LinearAlgebra;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Cern.Colt.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.ContextMask)]
    public class NormInfinityTest
    {
        [Test]
        public void TestMain()
        {
            IDoubleMatrix1D x1 = DoubleFactory1D.Dense.Make(new double[] { 1.0, 2.0 });
            IDoubleMatrix1D x2 = DoubleFactory1D.Dense.Make(new double[] { 1.0, -2.0 });
            IDoubleMatrix1D x3 = DoubleFactory1D.Dense.Make(new double[] { -1.0, -2.0 });
            IDoubleMatrix1D x4 = DoubleFactory1D.Dense.Make(new double[] { 4.0, 5.0 });

            ClassicAssert.AreEqual(2, Algebra.NormInfinity(x1));
            ClassicAssert.AreEqual(2, Algebra.NormInfinity(x2));
            ClassicAssert.AreEqual(2, Algebra.NormInfinity(x3));
            ClassicAssert.AreEqual(5, Algebra.NormInfinity(x4));
        }
    }
}
