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
    /// a single argument and returns a single value.
    /// <summary>
    public interface IIntFunction
    {
        IntFunctionDelegate Eval { set; }

        /// <summary>
        /// Applies a function to an argument.
        /// <summary>
        /// <param name="argument">  argument passed to the function.</param>
        /// <returns>the result of the function.</returns>
        int Apply(int argument);
    }
}

