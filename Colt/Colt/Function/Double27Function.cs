using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Function
{
    /// <summary>
    /// Abstract class that represents a function object: a function that takes 27 arguments and returns a single value.
    /// </summary>
    public abstract class Double27Function
    {
        /// <summary>
        /// Applies a function to 27 arguments.
        /// </summary>
        /// <param name="a000">the first argument passed to the function.</param>
        /// <param name="a001">the second argument passed to the function.</param>
        /// <param name="a002">the third argument passed to the function.</param>
        /// <param name="a010">the fourth argument passed to the function.</param>
        /// <param name="a011">the fifth argument passed to the function.</param>
        /// <param name="a012">the sixth argument passed to the function.</param>
        /// <param name="a020">the seventh argument passed to the function.</param>
        /// <param name="a021">the eighth argument passed to the function.</param>
        /// <param name="a022">the nineth argument passed to the function.</param>
        /// <param name="a100">the tenth argument passed to the function.</param>
        /// <param name="a101">the eleventh argument passed to the function.</param>
        /// <param name="a102">the twwelveth argument passed to the function.</param>
        /// <param name="a110">the thirteenth argument passed to the function.</param>
        /// <param name="a111">the fourteenth argument passed to the function.</param>
        /// <param name="a112">the fifteenth argument passed to the function.</param>
        /// <param name="a120">the sixteenth argument passed to the function.</param>
        /// <param name="a121">the seventeenth argument passed to the function.</param>
        /// <param name="a122">the eighteenth argument passed to the function.</param>
        /// <param name="a200">the nineteenth argument passed to the function.</param>
        /// <param name="a201">the twentyth argument passed to the function.</param>
        /// <param name="a202">the twenty-first argument passed to the function.</param>
        /// <param name="a210">the twenty-second argument passed to the function.</param>
        /// <param name="a211">the twenty-third argument passed to the function.</param>
        /// <param name="a212">the twenty-fourth argument passed to the function.</param>
        /// <param name="a220">the twenty-fifth argument passed to the function.</param>
        /// <param name="a221">the twenty-sixth argument passed to the function.</param>
        /// <param name="a222">the twenty-seventh argument passed to the function.</param>
        /// <returns>the result of the function.</returns>
        public delegate Double Apply(double a000, double a001, double a002,
                                        double a010, double a011, double a012,
                                        double a020, double a021, double a022,

                                        double a100, double a101, double a102,
                                        double a110, double a111, double a112,
                                        double a120, double a121, double a122,

                                        double a200, double a201, double a202,
                                        double a210, double a211, double a212,
                                        double a220, double a221, double a222
                                    );
    }
}
