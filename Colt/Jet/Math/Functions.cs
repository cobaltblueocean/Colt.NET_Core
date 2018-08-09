using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Function;

namespace Cern.Jet.Math
{
    /// <summary>
    /// Function objects to be passed to generic methodsd Contains the functions of <see cref="System.Math"/> as function objects, as well as a few more basic functions.
    /// Function objects conveniently allow to express arbitrary functions in a generic mannerd Essentially, a function object is an object that can perform a function on some arguments.
    /// It is a minimal delegate that takes the arguments, computes something and returns some result valued Function objects are comparable to function pointers in C used for call-backs.
    /// 
    /// Unary functions are of type <see cref="Cern.Colt.Function.DoubleFunction"/>, binary functions of type <see cref="Cern.Colt.Function.DoubleDoubleFunction"/>.
    /// All can be retrieved via public static variables named after the functiond 
    /// Unary predicates are of type <see cref="Cern.Colt.Function.DoubleProcedure"/>, binary predicates of type <see cref="Cern.Colt.Function.DoubleDoubleProcedure"/>.
    /// All can be retrieved via public static variables named IsXXXX.
    /// 
    /// Binary functions and predicates also exist as unary functions with the second argument being fixed to a constantd These are generated and retrieved via factory methods (again with the same name as the function).
    /// 
    /// More general, any binary function can be made an unary functions by fixing either the first or the second argument.
    /// See methods <see cref="Functions.DoubleFunctions.BindArg1(DoubleDoubleFunction, double)"/> and <see cref="Functions.DoubleFunctions.BindArg2(DoubleDoubleFunction, double)"/>d  The order of arguments can be swapped so that the first argument becomes the second and vice-versa.
    /// See method <see cref="Functions.DoubleDoubleFunctions.SwapArgs(DoubleDoubleFunction)"/>.
    /// 
    /// Even more general, functions can be chained (composed, assembled)d Assume we have two unary functions <i>g</i> and <i>h</i>.The unary function <i>g(h(a))</i> applying both in sequence can be generated via <see cref="Functions.DoubleFunctions.Chain(DoubleFunction, DoubleFunction)"/>d 
    /// 
    /// Arbitrarily complex functions can be composed from these building blocksd For example,
    /// sin(a) + cos<sup>2</sup>(b) can be specified as follows:
    /// 
    /// chain(plus,sin,chain(square,cos));
    /// 
    /// or, of course, as 
    /// 
    /// new DoubleDoubleFunction() {
    /// &nbsp;&nbsp;&nbsp;return new DoubleDoubleFunction((a, b) => { { return System.Math.Sin(a) + System.Math.Pow(System.Math.Cos(b), 2); }
    /// }
    /// </summary>
    public class Functions
    {
        public static Functions functions = new Functions();

        #region Constructors

        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected Functions() { }
        #endregion

        public class DoubleFunctions
        {
            #region Constructors
            /// <summary>
            /// Makes this class non instantiable, but still let's others inherit from it.
            /// </summary>
            protected DoubleFunctions() { }

            /// <summary>
            /// Constructs a function that returns (from &lt= a && a &lt= to) ? 1 : 0.
            /// </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <returns>(from &lt= a && a &lt= to) ? 1 : 0</returns>
            public static DoubleFunction Between(double from, double to)
            {
                return new DoubleFunction((a) => { return (from <= a && a <= to) ? 1 : 0; });
            }

            /// <summary>
            /// Constructs a unary function from a binary function with the first operand (argument) fixed to the given constant <i>c</i>.
            /// The second operand is variable (free).
            /// </summary>
            /// <param name="function">a binary function taking operands in the form <i>function.apply(c,var)</i>.</param>
            /// <param name="c"></param>
            /// <returns>the unary function <i>function(c,var)</i>.</returns>
            public static DoubleFunction BindArg1(DoubleDoubleFunction function, double c)
            {
                return new DoubleFunction((var) => { return function(c, var); });
            }

            /// <summary>
            /// Constructs a unary function from a binary function with the second operand (argument) fixed to the given constant <i>c</i>.
            /// The first operand is variable (free).
            /// </summary>
            /// <param name="function">a binary function taking operands in the form <i>function.apply(var,c)</i>.</param>
            /// <param name="c"></param>
            /// <returns>the unary function <i>function(var,c)</i>.</returns>
            public static DoubleFunction BindArg2(DoubleDoubleFunction function, double c)
            {
                return new DoubleFunction((var) => { return function(var, c); });
            }

            /// <summary>
            /// Constructs the function <i>f( g(a), h(b) )</i>.
            /// </summary>
            /// <param name="g">a binary function.</param>
            /// <param name="h">a binary function.</param>
            /// <returns>the binary function <i>f( g(a), h(b) )</i>.</returns>
            public static DoubleFunction Chain(DoubleFunction g, DoubleFunction h)
            {
                return new DoubleFunction((a) => { return g(h(a)); });
            }

            /// <summary>
            /// Constructs a function that returns <i>a < b ? -1 : a > b ? 1 : 0</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="b">the value to be compared.</param>
            /// <returns>the binary function</returns>
            public static DoubleFunction Compare(Double b)
            {
                return new DoubleFunction((a) => { return a < b ? -1 : a > b ? 1 : 0; });
            }

            /// <summary>
            /// Constructs a function that returns the constant <i>c</i>.
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public static DoubleFunction Constant(Double c)
            {
                return new DoubleFunction((a) => { return c; });
            }

            /// <summary>
            /// Constructs a function that returns <i>a / b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static DoubleFunction Div(Double b)
            {
                return new DoubleFunction((a) => { return (1 / b); });
            }

            /// <summary>
            /// Constructs a function that returns <i>a == b ? 1 : 0</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static DoubleFunction Equals(Double b)
            {
                return new DoubleFunction((a) => { return a == b ? 1 : 0; });
            }

            /// <summary>
            /// Constructs a function that returns <i>a > b ? 1 : 0</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static DoubleFunction Greater(Double b)
            {
                return new DoubleFunction((a) => { return a > b ? 1 : 0; });
            }

            /// <summary>
            /// Constructs a function that returns <i>System.Math.IEEERemainder(a,b)</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static DoubleFunction IEEEremainder(Double b)
            {
                return new DoubleFunction((a) => { return System.Math.IEEERemainder(a, b); });
            }

            /// <summary>
            /// Constructs a function that returns <i>from<=a && a<=to</i>.
            /// <i>a</i> is a variable, <i>from</i> and <i>to</i> are fixed.
            /// </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <returns></returns>
            public static DoubleProcedure IsBetween(double from, double to)
            {
                return new DoubleProcedure((a) => { return from <= a && a <= to; });
            }

            /// <summary>
            /// Constructs a function that returns <i>a == b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static DoubleProcedure IsEqual(double b)
            {
                return new DoubleProcedure((a) => { return a==b; });
            }

            /// <summary>
            /// Constructs a function that returns <i>a > b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static DoubleProcedure IsGreater(double b)
            {
                return new DoubleProcedure((a) => { return a > b; });
            }

            /// <summary>
            /// Constructs a function that returns <i>a < b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static DoubleProcedure IsLess(double b)
            {
                return new DoubleProcedure((a) => { return a < b; });
            }

            /// <summary>
            /// Constructs a function that returns <i>a < b ? 1 : 0</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static DoubleFunction Less(double b)
            {
                return new DoubleFunction((a) => { return a < b ? 1 : 0; });
            }

            /// <summary>
            /// Constructs a function that returns <i>System.Math.Log(a) / System.Math.Log(b)</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static DoubleFunction Lg(double b)
            {
                return new DoubleFunction((a) => {
                    double logInv = 1 / System.Math.Log(b);
                    return System.Math.Log(a) * logInv;
                });
            }

            /// <summary>
            /// Constructs a function that returns <i>System.Math.Max(a,b)</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static DoubleFunction Max(double b)
            {
                return new DoubleFunction((a) => { return System.Math.Max(a, b); });
            }

            /// <summary>
            /// Constructs a function that returns <i>System.Math.Min(a,b)</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static DoubleFunction Min(double b)
            {
                return new DoubleFunction((a) => { return System.Math.Min(a, b); });
            }

            /// <summary>
            /// Constructs a function that returns <i>a - b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static DoubleFunction Minus(double b)
            {
                return new DoubleFunction((a) => { return a - b; });
            }

            /// <summary>
            /// Constructs a function that returns <i>a % b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static DoubleFunction Mod(double b)
            {
                return new DoubleFunction((a) => { return a % b; });
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static DoubleFunction Mult(double b)
            {
                return new DoubleFunction((a) => { return a * b; });
            }

            /// <summary>
            /// Constructs a function that returns <i>a * b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static DoubleFunction Plus(double b)
            {
                return new DoubleFunction((a) => { return a + b; });
            }

            /// <summary>
            /// Constructs a function that returns <i>System.Math.Pow(a,b)</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="b"></param>
            /// <returns></returns>
            public static DoubleFunction Pow(double b)
            {
                return new DoubleFunction((a) => { return System.Math.Pow(a, b); });
            }

            /// <summary>
            /// Constructs a function that returns a new uniform random number in the open unit interval <code>(0.0,1.0)</code> (excluding 0.0 and 1.0).
            /// Currently the engine is <see cref="Cern.Jet.Random.Engine.MersenneTwister"/>
            /// and is seeded with the current time.
            /// 
            /// Note that any random engine derived from <see cref="Cern.Jet.Random.Engine.RandomEngine"/> and any random distribution derived from <see cref="Cern.Jet.Random.AbstractDistribution"/> are function objects, because they implement the proper interfaces.
            /// Thus, if you are not happy with the default, just pass your favourite random generator to function evaluating methods.
            /// </summary>
            /// <returns></returns>
            public static DoubleFunction Random()
            {
                return new DoubleFunction((a) => { return new Cern.Jet.Random.Engine.MersenneTwister(DateTime.Now).Raw(); });
            }

            /// <summary>
            /// Constructs a function that returns the number rounded to the given precision; <i>System.Math.Round(a / precision) * precision</i>.
            /// </summary>
            /// <param name="precision"></param>
            /// <returns></returns>
            public static DoubleFunction Round(double precision)
            {
                return new DoubleFunction((a) => { return System.Math.Round(a / precision) * precision; });
            }

            #endregion

            #region Unary functions
            /// <summary>
            /// Function that returns <i>System.Math.Abs(a)</i>.
            /// </summary>
            public static DoubleFunction Abs = new DoubleFunction((a) => { return System.Math.Abs(a); });

            /// <summary>
            /// Function that returns <i>System.Math.Acos(a)</i>.
            /// </summary>
            public static DoubleFunction Acos = new DoubleFunction((a) => { return System.Math.Acos(a); });

            /// <summary>
            /// Function that returns <i>System.Math.Asin(a)</i>.
            /// </summary>
            public static DoubleFunction Asin = new DoubleFunction((a) => { return System.Math.Asin(a); });

            /// <summary>
            /// Function that returns <i>System.Math.Atan(a)</i>.
            /// </summary>
            public static DoubleFunction Atan = new DoubleFunction((a) => { return System.Math.Atan(a); });

            /// <summary>
            /// Function that returns <i>System.Math.Ceiling(a)</i>.
            /// </summary>
            public static DoubleFunction Ceil = new DoubleFunction((a) => { return System.Math.Ceiling(a); });

            /// <summary>
            /// Function that returns <i>System.Math.Cos(a)</i>.
            /// </summary>
            public static DoubleFunction Cos = new DoubleFunction((a) => { return System.Math.Cos(a); });

            /// <summary>
            /// Function that returns <i>System.Math.Exp(a)</i>.
            /// </summary>
            public static DoubleFunction Exp = new DoubleFunction((a) => { return System.Math.Exp(a); });

            /// <summary>
            /// Function that returns <i>System.Math.Floor(a)</i>.
            /// </summary>
            public static DoubleFunction Floor = new DoubleFunction((a) => { return System.Math.Floor(a); });

            /// <summary>
            /// Function that returns its argument.
            /// </summary>
            public static DoubleFunction Identity = new DoubleFunction((a) => { return a; });

            /// <summary>
            /// Function that returns <i>1.0 / a</i>.
            /// </summary>
            public static DoubleFunction Inv = new DoubleFunction((a) => { return 1.0 / a; });

            /// <summary>
            /// Function that returns <i>System.Math.Log(a)</i>.
            /// </summary>
            public static DoubleFunction Log = new DoubleFunction((a) => { return System.Math.Log(a); });

            /// <summary>
            /// Function that returns <i>System.Math.Log(a) / System.Math.Log(2)</i>.
            /// </summary>
            public static DoubleFunction Log2 = new DoubleFunction((a) => { return System.Math.Log(a) * 1.4426950408889634; });

            /// <summary>
            /// Function that returns <i>-a</i>.
            /// </summary>
            public static DoubleFunction Neg = new DoubleFunction((a) => { return -a; });

            /// <summary>
            /// Function that returns <i>System.Math.Round(a)</i>.
            /// </summary>
            public static DoubleFunction Rint = new DoubleFunction((a) => { return System.Math.Round(a); });

            /// <summary>
            /// Function that returns <i>a < 0 ? -1 : a > 0 ? 1 : 0</i>.
            /// </summary>
            public static DoubleFunction Sign = new DoubleFunction((a) => { return System.Math.Sign(a); });

            /// <summary>
            /// Function that returns <i>System.Math.Sin(a)</i>.
            /// </summary>
            public static DoubleFunction Sin = new DoubleFunction((a) => { return System.Math.Sin(a); });

            /// <summary>
            /// Function that returns <i>System.Math.Sqrt(a)</i>.
            /// </summary>
            public static DoubleFunction Sqrt = new DoubleFunction((a) => { return System.Math.Sqrt(a); });

            /// <summary>
            /// Function that returns <i>a * a</i>.
            /// </summary>
            public static DoubleFunction Square = new DoubleFunction((a) => { return a * a; });

            /// <summary>
            /// Function that returns <i>System.Math.Tan(a)</i>.
            /// </summary>
            public static DoubleFunction Tan = new DoubleFunction((a) => { return System.Math.Tan(a); });

            #endregion

        }

        public class DoubleDoubleFunctions
        {
            #region Constructors
            /// <summary>
            /// Makes this class non instantiable, but still let's others inherit from it.
            /// </summary>
            protected DoubleDoubleFunctions() { }

            /// <summary>
            /// Constructs the function <i>f( g(a), h(b) )</i>.
            /// </summary>
            /// <param name="f">a binary function.</param>
            /// <param name="g">a binary function.</param>
            /// <param name="h">a binary function.</param>
            /// <returns>the binary function <i>f( g(a), h(b) )</i>.</returns>
            public static DoubleDoubleFunction Chain(DoubleDoubleFunction f, DoubleFunction g, DoubleFunction h)
            {
                return new DoubleDoubleFunction((a, b) => { return f(g(a), h(b)); });
            }

            /// <summary>
            /// Constructs the function <i>g( h(a,b) )</i>.
            /// </summary>
            /// <param name="g">a unary function.</param>
            /// <param name="h">a binary function.</param>
            /// <returns>the unary function <i>g( h(a,b) )</i>.</returns>
            public static DoubleDoubleFunction Chain(DoubleFunction g, DoubleDoubleFunction h)
            {
                return new DoubleDoubleFunction((a, b) => 
                {
                    return g(h(a, b)); 
                });
            }

            /// <summary>
            /// Constructs the function <i>g( h(a) )</i>.
            /// </summary>
            /// <param name="g">a unary function.</param>
            /// <param name="h">a unary function.</param>
            /// <returns>the unary function <i>g( h(a) )</i>.</returns>
            public static DoubleFunction Chain(DoubleFunction g, DoubleFunction h)
            {
                return new DoubleFunction((a) =>
                {

                    return g(h(a));
                });
            }

            /// <summary>
            /// Constructs a function that returns <i>a - b*constant</i>.
            /// <i>a</i> and <i>b</i> are variables, <i>constant</i> is fixed.
            /// </summary>
            /// <param name="constant"></param>
            /// <returns>the binary function <i>f(a - b * constant)</i></returns>
            public static DoubleDoubleFunction MinusMult(double constant)
            {
                return new DoubleDoubleFunction((a, b) => { return a - b * constant; });
            }

            /// <summary>
            /// Constructs a function that returns <i>a % b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            /// <param name="constant"></param>
            /// <returns>the binary function <i>f(a + b * constant)</i></returns>
            public static DoubleDoubleFunction PlusMult(double constant)
            {
                return new DoubleDoubleFunction((a, b) => { return a + b * constant; });
            }

            /// <summary>
            /// Constructs a function that returns <i>function.apply(b,a)</i>, i.ed applies the function with the first operand as second operand and the second operand as first operand.
            /// </summary>
            /// <param name="function">a function taking operands in the form <i>function.apply(a,b)</i>.</param>
            /// <returns>the binary function <i>function(b, a)</i>.</returns>
            public static DoubleDoubleFunction SwapArgs(DoubleDoubleFunction function)
            {
                return new DoubleDoubleFunction((a, b) => { return function(b, a); });
            }

            #endregion

            #region Binary functions
            /// <summary>
            /// Function that returns <i>System.Math.Atan2(a,b)</i>.
            /// </summary>
            public static DoubleDoubleFunction Atan2 = new DoubleDoubleFunction((a, b) => { return System.Math.Atan2(a, b); });

            /// <summary>
            /// Function that returns <i>a < b ? -1 : a > b ? 1 : 0</i>.
            /// </summary>
            public static DoubleDoubleFunction Compare = new DoubleDoubleFunction((a, b) => { return a < b ? -1 : a > b ? 1 : 0; });

            /// <summary>
            /// Function that returns <i>a / b</i>.
            /// </summary>
            public static DoubleDoubleFunction Div = new DoubleDoubleFunction((a, b) => { return a / b; });

            /// <summary>
            /// Function that returns <i>a == b ? 1 : 0</i>.
            /// </summary>
            public static new DoubleDoubleFunction Equals = new DoubleDoubleFunction((a, b) => { return a == b ? 1 : 0; });

            /// <summary>
            /// Function that returns <i>a > b ? 1 : 0</i>.
            /// </summary>
            public static DoubleDoubleFunction Greater = new DoubleDoubleFunction((a, b) => { return a > b ? 1 : 0; });

            /// <summary>
            /// Function that returns <i>System.Math.IEEERemainder(a,b)</i>.
            /// </summary>
            public static DoubleDoubleFunction IEEEremainder = new DoubleDoubleFunction((a, b) => { return System.Math.IEEERemainder(a, b); });

            /// <summary>
            /// Function that returns <i>a == b</i>.
            /// </summary>
            public static DoubleDoubleProcedure IsEqual = new DoubleDoubleProcedure((a, b) => { return a == b; });

            /// <summary>
            /// Function that returns <i>a < b</i>.
            /// </summary>
            public static DoubleDoubleProcedure IsLess = new DoubleDoubleProcedure((a, b) => { return a < b; });

            /// <summary>
            /// Function that returns <i>a > b</i>.
            /// </summary>
            public static DoubleDoubleProcedure IsGreater = new DoubleDoubleProcedure((a, b) => { return a > b; });

            /// <summary>
            /// Function that returns <i>a < b ? 1 : 0</i>.
            /// </summary>
            public static DoubleDoubleFunction Less = new DoubleDoubleFunction((a, b) => { return a < b ? 1 : 0; });

            /// <summary>
            /// Function that returns <i>System.Math.Log(a) / System.Math.Log(b)</i>.
            /// </summary>
            public static DoubleDoubleFunction Lg = new DoubleDoubleFunction((a, b) => { return System.Math.Log(a) / System.Math.Log(b); });

            /// <summary>
            /// Function that returns <i>System.Math.Max(a,b)</i>.
            /// </summary>
            public static DoubleDoubleFunction Max = new DoubleDoubleFunction((a, b) => { return System.Math.Max(a, b); });

            /// <summary>
            /// Function that returns <i>System.Math.Min(a,b)</i>.
            /// </summary>
            public static DoubleDoubleFunction Min = new DoubleDoubleFunction((a, b) => { return System.Math.Min(a, b); });

            /// <summary>
            /// Function that returns <i>a - b</i>.
            /// </summary>
            public static DoubleDoubleFunction Minus = new DoubleDoubleFunction((a, b) => { return a - b; });

            /// <summary>
            /// Function that returns <i>a % b</i>.
            /// </summary>
            public static DoubleDoubleFunction Mod = new DoubleDoubleFunction((a, b) => { return a % b; });

            /// <summary>
            /// Function that returns <i>a * b</i>.
            /// </summary>
            public static DoubleDoubleFunction Mult = new DoubleDoubleFunction((a, b) => { return a * b; });

            /// <summary>
            /// Function that returns <i>a + b</i>.
            /// </summary>
            public static DoubleDoubleFunction Plus = new DoubleDoubleFunction((a, b) => { return a + b; });

            /// <summary>
            /// Function that returns <i>System.Math.Abs(a) + System.Math.Abs(b)</i>.
            /// </summary>
            public static DoubleDoubleFunction PlusAbs = new DoubleDoubleFunction((a, b) => { return System.Math.Abs(a) + System.Math.Abs(b); });

            /// <summary>
            /// Function that returns <i>System.Math.Pow(a,b)</i>.
            /// </summary>
            public static DoubleDoubleFunction Pow = new DoubleDoubleFunction((a, b) => { return System.Math.Pow(a, b); });
            #endregion

        }
    }
}
