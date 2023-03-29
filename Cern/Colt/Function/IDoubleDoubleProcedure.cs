// Copyright � 1999 CERN - European Organization for Nuclear Research.
// Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
// is hereby granted without fee, provided that the above copyright notice appear in all copies and 
// that both that copyright notice and this permission notice appear in supporting documentation. 
// CERN makes no representations about the suitability of this software for any purpose. 
// It is provided "as is" without expressed or implied warranty.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Function
{
    /// <summary>
    /// Interface that represents a function object: a function that takes 
    /// two arguments and returns a single value.
    /// </summary>
    public interface IDoubleDoubleProcedure
    {
        /// <summary>
        /// Applies a procedure to one arguments.
        /// <summary>
        /// <param name="x">  the first argument passed to the function.</param>
        /// <param name="y">  the second argument passed to the function.</param>
        /// <returns>the result of the function.</returns>
        Boolean Apply(double x, double y);

        /// <summary>
        /// delegate to evaluate the argument
        /// </summary>
        /// <param name="argument1></param>
        /// <returns></returns>
        DoubleDoubleProcedureDelegate Eval { get; set; }
    }
}
