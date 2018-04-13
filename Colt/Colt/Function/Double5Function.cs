using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Function
{
    /// <summary>
    /// Abstract class that represents a function object: a function that takes 
    /// 5 arguments and returns a single value.
    /// </summary>
    public abstract class Double5Function
    {
        /// <summary>
        /// Applies a function to two arguments.
        /// </summary>
        /// <param name="a">the first argument passed to the function.</param>
        /// <param name="b">the second argument passed to the function.</param>
        /// <param name="c">the third argument passed to the function.</param>
        /// <param name="d">the fourth argument passed to the function.</param>
        /// <param name="e">the fifth argument passed to the function.</param>
        /// <returns>the result of the function.</returns>
        public delegate Double Apply(double a, double b, double c, double d, double e);
    }
}
