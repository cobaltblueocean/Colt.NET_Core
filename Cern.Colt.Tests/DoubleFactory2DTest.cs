// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleFactory2DTest.cs" company="Università Politecnica delle Marche">
//   Mauro Mazzieri, 2010.
//   Adapted from Java source code of Colt, copyright © 1999 CERN - European Organization for Nuclear Research.
// </copyright>
// <summary>
//   Unit test for <see cref="DoubleFactory2D"/>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
    using Cern.Colt.Matrix;
    using Cern.Colt.Matrix.LinearAlgebra;
    using Cern.Colt.Matrix.Implementation;
    using NUnit.Framework;
    using NUnit.Framework.Legacy;

namespace Cern.Colt.Tests
{

    /// <summary>
    /// Unit test for <see cref="DoubleFactory2D"/>.
    /// </summary>
    [TestFixture]
    public class DoubleFactory2DTest
    {
        /// <summary>
        /// Demonstrates usage of <see cref="DoubleFactory2D"/> class.
        /// </summary>
        [Test]
        public void Demo1()
        {
            foreach (var f in new[] { DoubleFactory2D.Dense, DoubleFactory2D.Sparse })
            {
                var parts1 = new[]
                    {
                        new[] { null, f.Make(2, 2, 1), null }, new[] { f.Make(4, 4, 2), null, f.Make(4, 3, 3) },
                        new[] { null, f.Make(2, 2, 4), null }
                    };
                var c1 = f.Compose(parts1);
                ClassicAssert.AreEqual(0, c1[0, 0]);
                ClassicAssert.AreEqual(0, c1[0, 3]);
                ClassicAssert.AreEqual(0, c1[1, 0]);
                ClassicAssert.AreEqual(0, c1[1, 3]);
                ClassicAssert.AreEqual(1, c1[0, 4]);
                ClassicAssert.AreEqual(1, c1[0, 5]);
                ClassicAssert.AreEqual(1, c1[1, 4]);
                ClassicAssert.AreEqual(1, c1[1, 5]);
                ClassicAssert.AreEqual(0, c1[0, 6]);
                ClassicAssert.AreEqual(0, c1[0, 8]);
                ClassicAssert.AreEqual(0, c1[1, 6]);
                ClassicAssert.AreEqual(0, c1[1, 8]);
                ClassicAssert.AreEqual(2, c1[2, 0]);
                ClassicAssert.AreEqual(2, c1[2, 3]);
                ClassicAssert.AreEqual(2, c1[5, 0]);
                ClassicAssert.AreEqual(2, c1[5, 3]);
                ClassicAssert.AreEqual(0, c1[2, 4]);
                ClassicAssert.AreEqual(0, c1[2, 5]);
                ClassicAssert.AreEqual(0, c1[5, 4]);
                ClassicAssert.AreEqual(0, c1[5, 5]);
                ClassicAssert.AreEqual(3, c1[2, 6]);
                ClassicAssert.AreEqual(3, c1[2, 8]);
                ClassicAssert.AreEqual(3, c1[5, 6]);
                ClassicAssert.AreEqual(3, c1[5, 8]);
                ClassicAssert.AreEqual(0, c1[6, 0]);
                ClassicAssert.AreEqual(0, c1[6, 3]);
                ClassicAssert.AreEqual(0, c1[7, 0]);
                ClassicAssert.AreEqual(0, c1[7, 3]);
                ClassicAssert.AreEqual(4, c1[6, 4]);
                ClassicAssert.AreEqual(4, c1[6, 5]);
                ClassicAssert.AreEqual(4, c1[7, 4]);
                ClassicAssert.AreEqual(4, c1[7, 5]);
                ClassicAssert.AreEqual(0, c1[6, 6]);
                ClassicAssert.AreEqual(0, c1[6, 8]);
                ClassicAssert.AreEqual(0, c1[7, 6]);
                ClassicAssert.AreEqual(0, c1[7, 8]);

                var parts3 = new[]
                    {
                        new[] { f.Identity(3), null, }, new[] { null, f.Identity(3).ViewColumnFlip() },
                        new[] { f.Identity(3).ViewRowFlip(), null }
                    };
                var c3 = f.Compose(parts3);

                ClassicAssert.AreEqual(1, c3[0, 0]);
                ClassicAssert.AreEqual(0, c3[0, 2]);
                ClassicAssert.AreEqual(0, c3[1, 4]);
                ClassicAssert.AreEqual(0, c3[4, 1]);
                ClassicAssert.AreEqual(0, c3[3, 3]);
                ClassicAssert.AreEqual(1, c3[3, 5]);
                ClassicAssert.AreEqual(1, c3[5, 3]);
                ClassicAssert.AreEqual(0, c3[5, 5]);
                ClassicAssert.AreEqual(0, c3[6, 0]);
                ClassicAssert.AreEqual(1, c3[6, 2]);
                ClassicAssert.AreEqual(1, c3[8, 0]);
                ClassicAssert.AreEqual(0, c3[8, 2]);
                ClassicAssert.AreEqual(0, c3[6, 3]);

                var a = f.Ascending(2, 2);
                IDoubleMatrix2D b = f.Descending(2, 2);
                const IDoubleMatrix2D N = null;

                var parts4 = new[] { new[] { a, N, a, N }, new[] { N, a, N, b } };
                var c4 = f.Compose(parts4);
                ClassicAssert.AreEqual(1, c4[0, 0]);
                ClassicAssert.AreEqual(2, c4[0, 1]);
                ClassicAssert.AreEqual(3, c4[1, 0]);
                ClassicAssert.AreEqual(4, c4[1, 1]);
                ClassicAssert.AreEqual(0, c4[3, 7]);
                ClassicAssert.AreEqual(1, c4[3, 6]);
                ClassicAssert.AreEqual(2, c4[2, 7]);
                ClassicAssert.AreEqual(3, c4[2, 6]);
            }
        }

        /// <summary>
        /// Demonstrates usage of <see cref="DoubleFactory2D"/> class.
        /// </summary>
        [Test]
        public void Demo2()
        {
            foreach (var f in new[] { DoubleFactory2D.Dense, DoubleFactory2D.Sparse })
            {
                const IDoubleMatrix2D N = null;
                IDoubleMatrix2D a = f.Make(2, 2, 1);
                IDoubleMatrix2D b = f.Make(4, 4, 2);
                IDoubleMatrix2D c = f.Make(4, 3, 3);
                IDoubleMatrix2D d = f.Make(2, 2, 4);
                var parts1 = new[] { new[] { N, a, N }, new[] { b, N, c }, new[] { N, d, N } };
                IDoubleMatrix2D matrix = f.Compose(parts1);
                var sm = matrix.ToString();

                a.Assign(9);
                b.Assign(9);
                c.Assign(9);
                d.Assign(9);
                f.Decompose(parts1, matrix);
                var sa = a.ToString();
                var sb = b.ToString();
                var sc = c.ToString();
                var sd = d.ToString();
            }
        }

        /// <summary>
        /// Test the principal and trace of a matrix.
        /// </summary>
        [Test]
        public void TestDiagonal()
        {
            var a = new DenseDoubleMatrix2D(new[]
                {
                    new[] { 5d, 2d, 4d },
                    new[] { -3d, 6d, 2d },
                    new[] { 3d, -3d, 1d }
                });
            var principal = DoubleFactory2D.Dense.Diagonal(a);
            ClassicAssert.AreEqual(5d, principal[0]);
            ClassicAssert.AreEqual(6d, principal[1]);
            ClassicAssert.AreEqual(1d, principal[2]);
            ClassicAssert.AreEqual(12d, Algebra.Trace(a));
        }
    }
}