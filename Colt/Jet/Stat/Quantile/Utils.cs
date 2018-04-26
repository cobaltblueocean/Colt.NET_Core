using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Jet.Stat.Quantile
{
    /// <summary>
    /// Holds some utility methods shared by different quantile finding implementations.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected Utils()
        {
        }

        /// <summary>
        /// Similar to System.Math.Ceiling(value), but adjusts small numerical rounding errors +- epsilon.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long EpsilonCeiling(double value)
        {
            double epsilon = 0.0000001;
            return (long)System.Math.Ceiling(value - epsilon);
        }
    }
}
