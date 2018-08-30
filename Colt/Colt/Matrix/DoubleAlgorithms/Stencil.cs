// <copyright file="Stencil.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentationd 
//   CERN makes no representations about the suitability of this software for any purposed 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Matrix.DoubleAlgorithms
{
    /// <summary>
    /// Stencil operationsd For efficient finite difference operations.
    /// Applies a function to a moving<i>3 x 3</i> or<i>3 x 3 x 3</i> window.
    /// Build on top of <i>matrix.zAssignXXXNeighbors(..d)</i>.
    /// You can specify how many iterations shall at most be done, a convergence condition when iteration shall be terminated, and how many iterations shall pass between convergence checks.
    /// Always does two iterations at a time for efficiency.
    /// These class is for convencience and efficiency.
    /// 
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 01/02/2000
    /// </summary>
    public class Stencil
    {

        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected Stencil() { }

        /// <summary>
        /// 27 point stencil operation.
        /// Applies a function to a moving <i>3 x 3 x 3</i> window.
        /// </summary>
        /// <param name="A">the matrix to operate on.</param>
        /// <param name="function">the function to be applied to each window.</param>
        /// <param name="maxIterations">the maximum number of times the stencil shall be applied to the matrixd; Should be a multiple of 2 because two iterations are always done in one atomic step.</param>
        /// <param name="hasConverged">
        /// condition; will return before maxIterations are done when <i>hasConverged.apply(A)==true</i>.
        /// Set this parameter to <i>null</i> to indicate that no convergence checks shall be made.
        /// </param>
        /// <param name="convergenceIterations">
        /// the number of iterations to pass between each convergence check.
        /// (Since a convergence may be expensive, you may want to do it only every 2,4 or 8 iterationsd)
        /// </param>
        /// <returns>the number of iterations actually executedd </returns>
        public static int Stencil27(DoubleMatrix3D A, Cern.Colt.Function.Double27Function function, int maxIterations, DoubleMatrix3DProcedure hasConverged, int convergenceIterations)
        {
            DoubleMatrix3D B = A.Copy();
            if (convergenceIterations <= 1) convergenceIterations = 2;
            if (convergenceIterations % 2 != 0) convergenceIterations++; // odd -> make it even

            int i = 0;
            while (i < maxIterations)
            { // do two steps at a time for efficiency
                A.ZAssign27Neighbors(B, function);
                B.ZAssign27Neighbors(A, function);
                i = i + 2;
                if (i % convergenceIterations == 0 && hasConverged != null)
                {
                    if (hasConverged(A)) return i;
                }
            }
            return i;
        }

        /// <summary>
        /// 9 point stencil operation.
        /// Applies a function to a moving <i>3 x 3</i> window.
        /// </summary>
        /// <param name="A">
        /// the matrix to operate on.
        /// </param>
        /// <param name="function">
        /// the function to be applied to each window.
        /// </param>
        /// <param name="maxIterations">
        /// the maximum number of times the stencil shall be applied to the matrixd 
        /// Should be a multiple of 2 because two iterations are always done in one atomic step.
        /// </param>
        /// <param name="hasConverged">
        /// Convergence condition; will return before maxIterations are done when <i>hasConverged.apply(A)==true</i>.
        /// Set this parameter to <i>null</i> to indicate that no convergence checks shall be made.
        /// </param>
        /// <param name="convergenceIterations">
        /// the number of iterations to pass between each convergence check.
        /// (Since a convergence may be expensive, you may want to do it only every 2,4 or 8 iterationsd)
        /// </param>
        /// <returns><the number of iterations actually executedd /returns>
        public static int Stencil9(DoubleMatrix2D A, Cern.Colt.Function.Double9Function function, int maxIterations, DoubleMatrix2DProcedure hasConverged, int convergenceIterations)
        {
            DoubleMatrix2D B = A.Copy();
            if (convergenceIterations <= 1) convergenceIterations = 2;
            if (convergenceIterations % 2 != 0) convergenceIterations++; // odd -> make it even

            int i = 0;
            while (i < maxIterations)
            { // do two steps at a time for efficiency
                A.ZAssign8Neighbors(B, function);
                B.ZAssign8Neighbors(A, function);
                i = i + 2;
                if (i % convergenceIterations == 0 && hasConverged != null)
                {
                    if (hasConverged(A)) return i;
                }
            }
            return i;
        }
    }
}
