using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F1 = Cern.Jet.Math.Functions.DoubleFunctions;
using F2 = Cern.Jet.Math.Functions.DoubleDoubleFunctions;

namespace Cern.Colt.Matrix.DoubleAlgorithms
{
    /// <summary>
/// Deprecated; Basic element-by-element transformations on {@link cern.colt.matrix.DoubleMatrix1D} and {@link cern.colt.matrix.DoubleMatrix2D}.
/// All transformations modify the first argument matrix to hold the result of the transformation.
/// Use idioms like <i>result = mult(matrix.copy(),5)</i> to leave source matrices unaffected.
/// <p>
/// If your favourite transformation is not provided by this class, consider using method <i>assign</i> in combination with prefabricated function objects of {@link cern.jet.math.Functions},
/// using idioms like 
/// <table>
/// <td class="PRE"> 
/// <pre>
/// cern.jet.math.Functions F = cern.jet.math.Functions.functions; // alias
/// matrix.Assign(F.square);
/// matrix.Assign(F.sqrt);
/// matrix.Assign(F.sin);
/// matrix.Assign(F.log);
/// matrix.Assign(F.log(b));
/// matrix.Assign(otherMatrix, F.min);
/// matrix.Assign(otherMatrix, F.max);
/// </pre>
/// </td>
/// </table>
/// Here are some <a href="../doc-files/functionObjects.html">other examples</a>.
/// <p>
/// Implementation: Performance optimized for medium to very large matrices.
/// In fact, there is now nomore a performance advantage in using this class; The assign (transform) methods directly defined on matrices are now just as fast.
/// Thus, this class will soon be removed altogether.
///
/// @deprecated
/// @author wolfgang.hoschek@cern.ch
/// @version 1.0, 09/24/99
    /// </summary>
    public class Transform
    {
        /// <summary>
        /// Little trick to allow for "aliasing", that is, renaming this class.
        /// Normally you would write
        /// <pre>
        /// Transform.mult(myMatrix,2);
        /// Transform.plus(myMatrix,5);
        /// </pre>
        /// Since this class has only static methods, but no instance methods
        /// you can also shorten the name "DoubleTransform" to a name that better suits you, for example "Trans".
        /// <pre>
        /// Transform T = Transform.transform; // kind of "alias"
        /// T.mult(myMatrix,2);
        /// T.plus(myMatrix,5);
        /// </pre>
        /// </summary>
        public static Transform transform = new Transform();

        //private static cern.jet.math.Functions F = cern.jet.math.Functions.functions; // alias

        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected Transform() { }

        /// <summary>
        /// A[i] = System.Math.Abs(A[i])
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix1D Abs(DoubleMatrix1D A)
        {
            return A.Assign(F1.Abs);
        }

        /// <summary>
        /// A[row,col] = System.Math.Abs(A[row,col])
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Abs(DoubleMatrix2D A)
        {
            return A.Assign(F1.Abs);
        }
        
        /// <summary>
        /// A = A / s <=> A[i] = A[i] / s
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix1D Div(DoubleMatrix1D A, double s)
        {
            return A.Assign(F1.Div(s));
        }

        /// <summary>
        /// A = A / B <=> A[i] = A[i] / B[i]
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix1D Div(DoubleMatrix1D A, DoubleMatrix1D B)
        {
            return A.Assign(B, F2.Div);
        }

        /// <summary>
        /// A = A / s <=> A[row,col] = A[row,col] / s
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Div(DoubleMatrix2D A, double s)
        {
            return A.Assign(F1.Div(s));
        }

        /// <summary>
        /// A = A / B <=> A[row,col] = A[row,col] / B[row,col]
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Div(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            return A.Assign(B, F2.Div);
        }

        /// <summary>
        /// A[row,col] = A[row,col] == s ? 1 : 0; ignores tolerance.
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Equals(DoubleMatrix2D A, double s)
        {
            return A.Assign(F1.Equals(s));
        }

        /// <summary>
        /// A[row,col] = A[row,col] == B[row,col] ? 1 : 0; ignores tolerance.
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Equals(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            return A.Assign(B, F2.Equals);
        }

        /// <summary>
        /// A[row,col] = A[row,col] > s ? 1 : 0
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Greater(DoubleMatrix2D A, double s)
        {
            return A.Assign(F1.Greater(s));
        }

        /// <summary>
        /// A[row,col] = A[row,col] > B[row,col] ? 1 : 0
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Greater(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            return A.Assign(B, F2.Greater);
        }

        /// <summary>
        /// A[row,col] = A[row,col] < s ? 1 : 0
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Less(DoubleMatrix2D A, double s)
        {
            return A.Assign(F1.Less(s));
        }

        /// <summary>
        /// A[row,col] = A[row,col] < B[row,col] ? 1 : 0
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Less(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            return A.Assign(B, F2.Less);
        }

        /// <summary>
        /// A = A - s <=> A[i] = A[i] - s
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix1D Minus(DoubleMatrix1D A, double s)
        {
            return A.Assign(F1.Minus(s));
        }

        /// <summary>
        /// A = A - B <=> A[i] = A[i] - B[i]
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix1D Minus(DoubleMatrix1D A, DoubleMatrix1D B)
        {
            return A.Assign(B, F2.Minus);
        }

        /// <summary>
        /// A = A - s <=> A[row,col] = A[row,col] - s
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Minus(DoubleMatrix2D A, double s)
        {
            return A.Assign(F1.Minus(s));
        }

        /// <summary>
        /// A = A - B <=> A[row,col] = A[row,col] - B[row,col]
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Minus(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            return A.Assign(B, F2.Minus);
        }

        /// <summary>
        /// A = A - B*s <=> A[i] = A[i] - B[i]*s
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix1D MinusMult(DoubleMatrix1D A, DoubleMatrix1D B, double s)
        {
            return A.Assign(B, F2.MinusMult(s));
        }

        /// <summary>
        /// A = A - B*s <=> A[row,col] = A[row,col] - B[row,col]*s
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D MinusMult(DoubleMatrix2D A, DoubleMatrix2D B, double s)
        {
            return A.Assign(B, F2.MinusMult(s));
        }

        /// <summary>
        /// >A = A * s <=> A[i] = A[i] * s
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix1D Mult(DoubleMatrix1D A, double s)
        {
            return A.Assign(F1.Mult(s));
        }

        /// <summary>
        /// A = A * B <=> A[i] = A[i] * B[i]
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix1D Mult(DoubleMatrix1D A, DoubleMatrix1D B)
        {
            return A.Assign(B, F2.Mult);
        }

        /// <summary>
        /// A = A * s <=> A[row,col] = A[row,col] * s
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Mult(DoubleMatrix2D A, double s)
        {
            return A.Assign(F1.Mult(s));
        }

        /// <summary>
        /// A = A * B <=> A[row,col] = A[row,col] * B[row,col]
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Mult(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            return A.Assign(B, F2.Mult);
        }

        /// <summary>
        /// A = -A <=> A[i] = -A[i] for all cells.
        /// </summary>
        /// <param name="A"></param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix1D Negate(DoubleMatrix1D A)
        {
            return A.Assign(F1.Mult(-1));
        }

        /// <summary>
        /// A = -A <=> A[row,col] = -A[row,col]
        /// </summary>
        /// <param name="A"></param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Negate(DoubleMatrix2D A)
        {
            return A.Assign(F1.Mult(-1));
        }

        /// <summary>
        /// A = A + s <=> A[i] = A[i] + s
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix1D Plus(DoubleMatrix1D A, double s)
        {
            return A.Assign(F1.Plus(s));
        }

        /// <summary>
        /// A = A + B <=> A[i] = A[i] + B[i]
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix1D Plus(DoubleMatrix1D A, DoubleMatrix1D B)
        {
            return A.Assign(B, F2.Plus);
        }

        /// <summary>
        /// A = A + s <=> A[row,col] = A[row,col] + s
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Plus(DoubleMatrix2D A, double s)
        {
            return A.Assign(F1.Plus(s));
        }

        /// <summary>
        /// A = A + B <=> A[row,col] = A[row,col] + B[row,col]
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Plus(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            return A.Assign(B, F2.Plus);
        }

        /// <summary>
        /// A = A + B*s<=> A[i] = A[i] + B[i]*s
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix1D PlusMult(DoubleMatrix1D A, DoubleMatrix1D B, double s)
        {
            return A.Assign(B, F2.PlusMult(s));
        }

        /// <summary>
        /// A = A + B*s <=> A[row,col] = A[row,col] + B[row,col]*s
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D PlusMult(DoubleMatrix2D A, DoubleMatrix2D B, double s)
        {
            return A.Assign(B, F2.PlusMult(s));
        }

        /// <summary>
        /// A = A<sup>s</sup> <=> A[i] = System.Math.Pow(A[i], s)
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix1D Pow(DoubleMatrix1D A, double s)
        {
            return A.Assign(F1.Pow(s));
        }

        /// <summary>
        /// A = A<sup>B</sup> <=> A[i] = System.Math.Pow(A[i], B[i])
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix1D Pow(DoubleMatrix1D A, DoubleMatrix1D B)
        {
            return A.Assign(B, F2.Pow);
        }

        /// <summary>
        /// A = A<sup>s</sup> &lt;=> A[row,col] = System.Math.Pow(A[row,col], s)
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="s">the scalar; can have any value.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Pow(DoubleMatrix2D A, double s)
        {
            return A.Assign(F1.Pow(s));
        }

        /// <summary>
        /// A = A<sup>B</sup> &lt;=> A[row,col] = System.Math.Pow(A[row,col], B[row,col])
        /// </summary>
        /// <param name="A">the matrix to modify.</param>
        /// <param name="B">the matrix to stay unaffected.</param>
        /// <returns><i>A</i> (for convenience only).</returns>
        public static DoubleMatrix2D Pow(DoubleMatrix2D A, DoubleMatrix2D B)
        {
            return A.Assign(B, F2.Pow);
        }
    }
}
