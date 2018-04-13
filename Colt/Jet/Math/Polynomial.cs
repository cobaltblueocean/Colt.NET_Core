using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Jet.Math
{
    /// <summary>
    /// Polynomial functions.
    /// </summary>
    public class Polynomial : Constants
    {

        #region Local Variables

        #endregion

        #region Property

        #endregion

        #region Constructor

        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected Polynomial() { }

        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods

        /// <summary>
        /// Evaluates the given polynomial of degree <tt>N</tt> at <tt>x</tt>, assuming coefficient of N is 1.0.
        /// Otherwise same as <tt>polevl()</tt>.
        /// <pre>
        ///                     2          N
        /// y  =  C  + C x + C x  +...+ C x
        ///        0    1     2          N
        ///
        /// where C  = 1 and hence is omitted from the array.
        ///        N
        ///
        /// Coefficients are stored in reverse order:
        ///
        /// coef[0] = C  , ..., coef[N-1] = C  .
        ///            N-1                   0
        ///
        /// Calling arguments are otherwise the same as polevl().
        /// </pre>
        /// In the interest of speed, there are no checks for out of bounds arithmetic.        /// 
        /// </summary>
        /// <param name="x">argument to the polynomial.</param>
        /// <param name="coef">the coefficients of the polynomial.</param>
        /// <param name="N">the degree of the polynomial.</param>
        /// <returns></returns>
        public static double P1evl(double x, double[] coef, int N)
        {

            double ans;

            ans = x + coef[0];

            for (int i = 1; i < N; i++) { ans = ans * x + coef[i]; }

            return ans;
        }

        /// <summary>
        /// Evaluates the given polynomial of degree <tt>N</tt> at <tt>x</tt>.
        /// <pre>
        ///                     2          N
        /// y  =  C  + C x + C x  +...+ C x
        ///        0    1     2          N
        ///
        /// Coefficients are stored in reverse order:
        ///
        /// coef[0] = C  , ..., coef[N] = C  .
        ///            N                   0
        /// </pre>
        /// In the interest of speed, there are no checks for out of bounds arithmetic.
        /// </summary>
        /// <param name="x">argument to the polynomial.</param>
        /// <param name="coef">the coefficients of the polynomial.</param>
        /// <param name="N">the degree of the polynomial.</param>
        /// <returns></returns>
        public static double Polevl(double x, double[] coef, int N)
        {

            double ans;
            ans = coef[0];

            for (int i = 1; i <= N; i++) ans = ans * x + coef[i];

            return ans;
        }
        #endregion

        #region Local Private Methods

        #endregion

    }
}
