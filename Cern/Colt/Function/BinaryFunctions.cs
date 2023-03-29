// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryFunctions.cs" company="CERN">
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
using Cern.Jet.Math;

namespace Cern.Colt.Function
{
    /// <summary>
    /// Compares its two arguments for order.  Returns a negative integer,
    /// zero, or a positive integer as the first argument is less than, equal
    /// to, or greater than the second.
    /// </summary>
    /// <param name="o1">
    /// The first argument.
    /// </param>
    /// <param name="o2">
    /// The second argument.
    /// </param>
    /// <returns>
    /// The comparison result.
    /// </returns>
    public delegate int ByteComparatorDelegate(byte o1, byte o2);

    /// <summary>
    /// Compares its two arguments for order.  Returns a negative integer,
    /// zero, or a positive integer as the first argument is less than, equal
    /// to, or greater than the second.
    /// </summary>
    /// <param name="o1">
    /// The first argument.
    /// </param>
    /// <param name="o2">
    /// The second argument.
    /// </param>
    /// <returns>
    /// The comparison result.
    /// </returns>
    public delegate int CharComparatorDelegate(char o1, char o2);

    /// <summary>
    /// A function that takes arguments and returns a single value.
    /// </summary>
    /// <returns>
    /// The result of the function.
    /// </returns>
    public delegate double Double9FunctionDelegate(
        double a00, double a01, double a02, double a10, double a11, double a12, double a20, double a21, double a22);


    /// <summary>
    /// A function that takes arguments and returns a single value.
    /// </summary>
    /// <returns>
    /// The result of the function.
    /// </returns>
    public delegate double Double27FunctionDelegate(double a000, double a001, double a002, double a010, double a011, double a012, double a020, double a021, double a022,
                                            double a100, double a101, double a102, double a110, double a111, double a112, double a120, double a121, double a122,
                                            double a200, double a201, double a202, double a210, double a211, double a212, double a220, double a221, double a222);

    /// <summary>
    /// A comparison function which imposes a <i>total ordering</i> on some collection of elements.
    /// Returns a negative integer, zero, or a positive integer as the first argument is less than, equal to, or greater than the second.
    /// </summary>
    /// <param name="a">
    /// The first argument.
    /// </param>
    /// <param name="b">
    /// The second argument.
    /// </param>
    /// <returns>
    /// A negative integer, zero, or a positive integer as the first argument is less than, equal to, or greater than the second. 
    /// </returns>
    public delegate int DoubleComparatorDelegate(double a, double b);

    /// <summary>
    /// A function that takes two arguments and returns a single value.
    /// </summary>
    /// <param name="x">
    /// The first argument passed to the function.
    /// </param>
    /// <param name="y">
    /// The second argument passed to the function.
    /// </param>
    /// <returns>
    /// The result of the function.
    /// </returns>
    public delegate double DoubleDoubleFunctionDelegate(double x, double y);

    /// <summary>
    /// Applies a procedure to an argument.
    /// Optionally can return a boolean flag to inform the object calling the procedure.
    /// </summary>
    /// <param name="x">
    /// The first argument passed to the function.
    /// </param>
    /// <param name="y">
    /// The second argument passed to the function.
    /// </param>
    /// <returns>
    /// A flag to inform the object calling the procedure.
    /// </returns>
    public delegate bool DoubleDoubleProcedureDelegate(double x, double y);

    /// <summary>
    /// A function that takes two arguments and returns a single value.
    /// </summary>
    /// <param name="x">
    /// The first argument passed to the function.
    /// </param>
    /// <param name="y">
    /// The second argument passed to the function.
    /// </param>
    /// <returns>
    /// The result of the function.
    /// </returns>
    public delegate int IntIntFunctionDelegate(int x, int y);


    /// <summary>
    /// Applies a procedure to an argument.
    /// Optionally can return a boolean flag to inform the object calling the procedure.
    /// </summary>
    /// <param name="x">
    /// The first argument passed to the function.
    /// </param>
    /// <param name="y">
    /// The second argument passed to the function.
    /// </param>
    /// <returns>
    /// A flag to inform the object calling the procedure.
    /// </returns>
    public delegate bool IntIntProcedureDelegate(int x, int y);

    /// <summary>
    /// Applies a procedure to an argument.
    /// Optionally can return a boolean flag to inform the object calling the procedure.
    /// </summary>
    /// <param name="x">
    /// The first argument passed to the function.
    /// </param>
    /// <param name="y">
    /// The second argument passed to the function.
    /// </param>
    /// <param name="z">
    /// The third argument passed to the function.
    /// </param>
    /// <returns>
    /// A flag to inform the object calling the procedure.
    /// </returns>
    public delegate bool IntIntIntProcedureDelegate(int x, int y, int z);

    /// <summary>
    /// Compares its two arguments for order.  Returns a negative integer,
    /// zero, or a positive integer as the first argument is less than, equal
    /// to, or greater than the second.
    /// </summary>
    /// <param name="o1">
    /// The first argument.
    /// </param>
    /// <param name="o2">
    /// The second argument.
    /// </param>
    /// <returns>
    /// The comparison result.
    /// </returns>
    public delegate int FloatComparatorDelegate(float o1, float o2);

    /// <summary>
    /// Compares its two arguments for order.  Returns a negative integer,
    /// zero, or a positive integer as the first argument is less than, equal
    /// to, or greater than the second.
    /// </summary>
    /// <param name="o1">
    /// The first argument.
    /// </param>
    /// <param name="o2">
    /// The second argument.
    /// </param>
    /// <returns>
    /// The comparison result.
    /// </returns>
    public delegate int IntComparatorDelegate(int o1, int o2);

    /// <summary>
    /// A function that takes three arguments.
    /// </summary>
    /// <param name="first">
    /// The first argument passed to the function.
    /// </param>
    /// <param name="second">
    /// The second argument passed to the function.
    /// </param>
    /// <param name="third">
    /// The third argument passed to the function.
    /// </param>
    /// <returns>
    /// The result of the function.
    /// </returns>
    public delegate double IntIntDoubleFunctionDelegate(int first, int second, double third);

    /// <summary>
    /// A function that takes three arguments.
    /// </summary>
    /// <param name="first">
    /// The first argument passed to the function.
    /// </param>
    /// <param name="second">
    /// The second argument passed to the function.
    /// </param>
    /// <param name="third">
    /// The third argument passed to the function.
    /// </param>
    /// <returns>
    /// The result of the function.
    /// </returns>
    public delegate int IntIntIntFunctionDelegate(int first, int second, int third);

    /// <summary>
    /// Compares its two arguments for order.  Returns a negative integer,
    /// zero, or a positive integer as the first argument is less than, equal
    /// to, or greater than the second.
    /// </summary>
    /// <param name="o1">
    /// The first argument.
    /// </param>
    /// <param name="o2">
    /// The second argument.
    /// </param>
    /// <returns>
    /// The comparison result.
    /// </returns>
    public delegate int LongComparatorDelegate(long o1, long o2);

    /// <summary>
    /// Compares its two arguments for order.  Returns a negative integer,
    /// zero, or a positive integer as the first argument is less than, equal
    /// to, or greater than the second.
    /// </summary>
    /// <typeparam name="C">
    /// The type of the arguments.
    /// </typeparam>
    /// <param name="o1">
    /// The first argument.
    /// </param>
    /// <param name="o2">
    /// The second argument.
    /// </param>
    /// <returns>
    /// The comparison result.
    /// </returns>
    public delegate int ObjectComparatorDelegate<C>(C o1, C o2);

    /// <summary>
    /// A function that takes two arguments and returns a single value.
    /// </summary>
    /// <param name="x">
    /// The first argument passed to the function.
    /// </param>
    /// <param name="y">
    /// The second argument passed to the function.
    /// </param>
    /// <returns>
    /// The result of the function.
    /// </returns>
    /// <typeparam name="C">
    /// The type of the arguments and of the return value.
    /// </typeparam>
    public delegate C ObjectObjectFunctionDelegate<C>(C x, C y);

    /// <summary>
    /// Compares its two arguments for order.  Returns a negative integer,
    /// zero, or a positive integer as the first argument is less than, equal
    /// to, or greater than the second.
    /// </summary>
    /// <param name="o1">
    /// The first argument.
    /// </param>
    /// <param name="o2">
    /// The second argument.
    /// </param>
    /// <returns>
    /// The comparison result.
    /// </returns>
    public delegate int ShortComparatorDelegate(short o1, short o2);

    /// <summary>
    /// Function to be passed to generic methods.
    /// </summary>
    public static class BinaryFunctions
    {
        /// <summary>
        /// Function that returns <tt>a + b</tt>.
        /// </summary>
        public static readonly IDoubleDoubleFunction Plus = new DoubleDoubleFunction()
        {
            Eval = (a, b) => a + b
        };

        /// <summary>
        /// Function that returns <tt>a - b</tt>.
        /// </summary>
        public static readonly IDoubleDoubleFunction Minus = new DoubleDoubleFunction()
        {
            Eval = (a, b) => a - b
        };

        /// <summary>
        /// Function that returns <tt>a * b</tt>.
        /// </summary>
        public static readonly IDoubleDoubleFunction Mult = new DoubleDoubleFunction()
        {
            Eval = (a, b) => a * b
        };
    }
}