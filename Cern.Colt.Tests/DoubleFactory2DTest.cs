// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleFactory2DTest.cs" company="Università Politecnica delle Marche">
//   Mauro Mazzieri, 2010.
//   Adapted from Java source code of Colt, copyright © 1999 CERN - European Organization for Nuclear Research.
// </copyright>
// <summary>
//   Unit test for <see cref="DoubleFactory2D"/>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Tests
{
    using Cern.Colt.Matrix;
    using Cern.Colt.Matrix.LinearAlgebra;

    using NUnit.Framework;
    using Cern.Colt.Matrix.Implementation;

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
                Assert.AreEqual(0, c1[0, 0]);
                Assert.AreEqual(0, c1[0, 3]);
                Assert.AreEqual(0, c1[1, 0]);
                Assert.AreEqual(0, c1[1, 3]);
                Assert.AreEqual(1, c1[0, 4]);
                Assert.AreEqual(1, c1[0, 5]);
                Assert.AreEqual(1, c1[1, 4]);
                Assert.AreEqual(1, c1[1, 5]);
                Assert.AreEqual(0, c1[0, 6]);
                Assert.AreEqual(0, c1[0, 8]);
                Assert.AreEqual(0, c1[1, 6]);
                Assert.AreEqual(0, c1[1, 8]);
                Assert.AreEqual(2, c1[2, 0]);
                Assert.AreEqual(2, c1[2, 3]);
                Assert.AreEqual(2, c1[5, 0]);
                Assert.AreEqual(2, c1[5, 3]);
                Assert.AreEqual(0, c1[2, 4]);
                Assert.AreEqual(0, c1[2, 5]);
                Assert.AreEqual(0, c1[5, 4]);
                Assert.AreEqual(0, c1[5, 5]);
                Assert.AreEqual(3, c1[2, 6]);
                Assert.AreEqual(3, c1[2, 8]);
                Assert.AreEqual(3, c1[5, 6]);
                Assert.AreEqual(3, c1[5, 8]);
                Assert.AreEqual(0, c1[6, 0]);
                Assert.AreEqual(0, c1[6, 3]);
                Assert.AreEqual(0, c1[7, 0]);
                Assert.AreEqual(0, c1[7, 3]);
                Assert.AreEqual(4, c1[6, 4]);
                Assert.AreEqual(4, c1[6, 5]);
                Assert.AreEqual(4, c1[7, 4]);
                Assert.AreEqual(4, c1[7, 5]);
                Assert.AreEqual(0, c1[6, 6]);
                Assert.AreEqual(0, c1[6, 8]);
                Assert.AreEqual(0, c1[7, 6]);
                Assert.AreEqual(0, c1[7, 8]);

                var parts3 = new[]
                    {
                        new[] { f.Identity(3), null, }, new[] { null, f.Identity(3).ViewColumnFlip() },
                        new[] { f.Identity(3).ViewRowFlip(), null }
                    };
                var c3 = f.Compose(parts3);

                Assert.AreEqual(1, c3[0, 0]);
                Assert.AreEqual(0, c3[0, 2]);
                Assert.AreEqual(0, c3[1, 4]);
                Assert.AreEqual(0, c3[4, 1]);
                Assert.AreEqual(0, c3[3, 3]);
                Assert.AreEqual(1, c3[3, 5]);
                Assert.AreEqual(1, c3[5, 3]);
                Assert.AreEqual(0, c3[5, 5]);
                Assert.AreEqual(0, c3[6, 0]);
                Assert.AreEqual(1, c3[6, 2]);
                Assert.AreEqual(1, c3[8, 0]);
                Assert.AreEqual(0, c3[8, 2]);
                Assert.AreEqual(0, c3[6, 3]);

                var a = f.Ascending(2, 2);
                DoubleMatrix2D b = f.Descending(2, 2);
                const DoubleMatrix2D N = null;

                var parts4 = new[] { new[] { a, N, a, N }, new[] { N, a, N, b } };
                var c4 = f.Compose(parts4);
                Assert.AreEqual(1, c4[0, 0]);
                Assert.AreEqual(2, c4[0, 1]);
                Assert.AreEqual(3, c4[1, 0]);
                Assert.AreEqual(4, c4[1, 1]);
                Assert.AreEqual(0, c4[3, 7]);
                Assert.AreEqual(1, c4[3, 6]);
                Assert.AreEqual(2, c4[2, 7]);
                Assert.AreEqual(3, c4[2, 6]);
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
                const DoubleMatrix2D N = null;
                DoubleMatrix2D a = f.Make(2, 2, 1);
                DoubleMatrix2D b = f.Make(4, 4, 2);
                DoubleMatrix2D c = f.Make(4, 3, 3);
                DoubleMatrix2D d = f.Make(2, 2, 4);
                var parts1 = new[] { new[] { N, a, N }, new[] { b, N, c }, new[] { N, d, N } };
                DoubleMatrix2D matrix = f.Compose(parts1);
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
            Assert.AreEqual(5d, principal[0]);
            Assert.AreEqual(6d, principal[1]);
            Assert.AreEqual(1d, principal[2]);
            Assert.AreEqual(12d, Algebra.Trace(a));
        }
    }
}