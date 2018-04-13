// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnaryFunctions.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   A function that takes two arguments and returns a single value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Cern.Colt.Function
{
    /// <summary>
    /// A function that takes a single argument and returns a single value.
    /// </summary>
    /// <param name="argument">
    /// The argument passed to the function.
    /// </param>
    /// <returns>
    /// The result of the function.
    /// </returns>
    public delegate double DoubleFunction(double argument);

    /// <summary>
    /// Applies a procedure to an argument.
    /// Optionally can return a boolean flag to inform the object calling the procedure.
    /// </summary>
    /// <param name="element">
    /// The element passed to the procedure..
    /// </param>
    /// <returns>
    /// A flag to inform the object calling the procedure.
    /// </returns>
    public delegate bool DoubleProcedure(double element);

    /// <summary>
    /// A function that takes a single argument and returns a single value.
    /// </summary>
    /// <param name="argument">
    /// The argument passed to the function.
    /// </param>
    /// <returns>
    /// The result of the function.
    /// </returns>
    /// <typeparam name="C">
    /// The type of the argument and of the return value.
    /// </typeparam>
    public delegate C ObjectFunction<C>(C argument);

    /// <summary>
    /// Function to be passed to generic methods.
    /// </summary>
    public static class UnaryFunctions
    {
        /// <summary>
        /// Function that returns its argument.
        /// </summary>
        public static readonly DoubleFunction Identity = a => a;

        /// <summary>
        /// Function that returns <tt>-a</tt>.
        /// </summary>
        public static readonly DoubleFunction Neg = a => -a;

        /// <summary>
        /// A function that returns <tt>a + b</tt>.
        /// <tt>a</tt> is a variable, <tt>b</tt> is fixed.
        /// </summary>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// A function that returns <tt>a + b</tt>.
        /// </returns>
        public static DoubleFunction Plus(double b)
        {
            return a => a + b;
        }
        
        /// <summary>
        /// Constructs the function <tt>g( h(a) )</tt>.
        /// </summary>
        /// <param name="g">
        /// The unary function g.
        /// </param>
        /// <param name="h">
        /// The unary function h.
        /// </param>
        /// <returns>
        /// The unary function <tt>g( h(a) )</tt>.
        /// </returns>
        public static DoubleFunction Chain(DoubleFunction g, DoubleFunction h)
        {
            return a => g(h(a));
        }

        /// <summary>
        /// A function that returns <tt>a / b</tt>.
        /// <tt>a</tt> is a variable, <tt>b</tt> is fixed.
        /// </summary>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// A function that returns <tt>a / b</tt>.
        /// </returns>
        public static DoubleFunction Div(double b)
        {
            return a => a / b;
        }

        /// <summary>
        /// A function that returns <tt>a - b</tt>.
        /// <tt>a</tt> is a variable, <tt>b</tt> is fixed.
        /// </summary>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// A function that returns <tt>a - b</tt>.
        /// </returns>
        public static DoubleFunction Minus(double b)
        {
            return a => a - b;
        }

        /// <summary>
        /// A function that returns <tt>a * b</tt>.
        /// <tt>a</tt> is a variable, <tt>b</tt> is fixed.
        /// </summary>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// A function that returns <tt>a * b</tt>.
        /// </returns>
        public static DoubleFunction Mult(double b)
        {
            return a => a * b;
        }
    }
}