using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Function;

namespace Cern.Jet.Math
{
    public class IntFunctions
    {

        #region Local Variables
        public static IntFunctions intFunctions = new IntFunctions();

        #endregion

        #region Property

        #endregion

        #region Constructor
        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected IntFunctions() { }

        #endregion

        #region Implement Methods

        #endregion

        public class UnaryFunctions
        {
            public static IntFunction Abs = new IntFunction((a) => { return (a < 0) ? -a : a; });

            /// <summary>
            /// Function that returns <i>a--</i>.
            /// </summary>
            public static IntFunction Dec = new IntFunction((a) => { return a--; });

            /// <summary>
            /// Function that returns <i>(int) Arithmetic.factorial(a)</i>.
            /// </summary>
            public static IntFunction Factorial = new IntFunction((a) => { return (int)Arithmetic.Factorial(a); });

            /// <summary>
            /// Function that returns its argument.
            /// </summary>
            public static IntFunction Identity = new IntFunction((a) => { return a; });

            /// <summary>
            /// Function that returns <i>a++</i>.
            /// </summary>
            public static IntFunction Inc = new IntFunction((a) => { return a++; });

            /// <summary>
            /// Function that returns <i>-a</i>.
            /// </summary>
            public static IntFunction Neg = new IntFunction((a) => { return -a; });

            /// <summary>
            /// Function that returns <i>~a</i>.
            /// </summary>
            public static IntFunction Not = new IntFunction((a) => { return ~a; });

            /// <summary>
            /// Function that returns <i>a < 0 ? -1 : a > 0 ? 1 : 0</i>.
            /// </summary>
            public static IntFunction Sign = new IntFunction((a) => { return a < 0 ? -1 : a > 0 ? 1 : 0; });

            /// <summary>
            /// Function that returns <i>a/// a</i>.
            /// </summary>
            public static IntFunction Square = new IntFunction((a) => { return a * a; });

            /// <summary>
            /// Constructs a function that returns <i>a & b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunction And(int b)
            {
                return new IntFunction((a) => { return a & b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>(from %lt;= a && a %lt;= to) ? 1 : 0</i>.
            /// <i>a</i> is a variable, <i>from</i> and <i>to</i> are fixed.
            /// </summary>
            public static IntFunction Between(int from, int to)
            {
                return new IntFunction((a) => { return (from <= a && a <= to) ? 1 : 0; });
            }

            /// <summary>
            /// Constructs a unary function from a binary function with the first operand (argument) fixed to the given constant <i>c</i>.
            /// The second operand is variable (free).
            /// </summary>
            /// <param name="function">a binary function taking operands in the form <i>function(c,var)</i>.</param>
            /// <param name="c"></param>
            /// <returns>the unary function <i>function(c,var)</i>.</returns>
            public static IntFunction BindArg1(IntIntFunction function, int c)
            {
                return new IntFunction((a) => { return function(c, a); });
            }
            /// <summary>
            /// Constructs a unary function from a binary function with the second operand (argument) fixed to the given constant <i>c</i>.
            /// The first operand is variable (free).
            /// </summary>
            /// <param name="function">a binary function taking operands in the form <i>function(var,c)</i>.</param>
            /// <param name="c"></param>
            /// <returns>the unary function <i>function(var,c)</i>.</returns>
            public static IntFunction BindArg2(IntIntFunction function, int c)
            {
                return new IntFunction((a) => { return function(a, c); });
            }
            /// <summary>
            /// Constructs the function <i>g( h(a) )</i>.
            /// </summary>
            /// <param name="g">a unary function.</param>
            /// <param name="h">a unary function.</param>
            /// <returns>the unary function <i>g( h(a) )</i>.</returns>
            public static IntFunction Chain(IntFunction g, IntFunction h)
            {
                return new IntFunction((a) => { return g(h(a)); });
            }
            /// <summary>
            /// Constructs a function that returns <i>a < b ? -1 : a > b ? 1 : 0</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunction Compare(int b)
            {
                return new IntFunction((a) => { return a < b ? -1 : a > b ? 1 : 0; });
            }
            /// <summary>
            /// Constructs a function that returns the constant <i>c</i>.
            /// </summary>
            public static IntFunction Constant(int c)
            {
                return new IntFunction((a) => { return c; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a / b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunction Div(int b)
            {
                return new IntFunction((a) => { return a / b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a == b ? 1 : 0</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunction Equals(int b)
            {
                return new IntFunction((a) => { return a == b ? 1 : 0; });
            }
            /// <summary>
            /// Constructs a function that returns <i>from<=a && a<=to</i>.
            /// <i>a</i> is a variable, <i>from</i> and <i>to</i> are fixed.
            /// </summary>
            public static IntProcedure IsBetween(int from, int to)
            {
                return new IntProcedure((a) => { return from <= a && a <= to; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a == b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntProcedure IsEqual(int b)
            {
                return new IntProcedure((a) => { return a == b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a > b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntProcedure IsGreater(int b)
            {
                return new IntProcedure((a) => { return a > b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a < b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntProcedure IsLess(int b)
            {
                return new IntProcedure((a) => { return a < b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>System.Math.Max(a,b)</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunction Max(int b)
            {
                return new IntFunction((a) => { return (a >= b) ? a : b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>System.Math.Min(a,b)</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunction Min(int b)
            {
                return new IntFunction((a) => { return (a <= b) ? a : b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a - b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunction Minus(int b)
            {
                return new IntFunction((a) => { return a - b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a % b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunction Mod(int b)
            {
                return new IntFunction((a) => { return a % b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a * b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunction Mult(int b)
            {
                return new IntFunction((a) =>
                {
                    return a * b;
                });
            }
            /// <summary>
            /// Constructs a function that returns <i>a | b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunction Or(int b)
            {
                return new IntFunction((a) => { return a | b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a + b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunction Plus(int b)
            {
                return new IntFunction((a) => { return a + b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>(int) System.Math.Pow(a,b)</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunction Pow(int b)
            {
                return new IntFunction((a) => { return (int)System.Math.Pow(a, b); });
            }
            /// <summary>
            /// Constructs a function that returns a 32 bit uniformly distributed random number in the closed interval <i>[int.MinValue,int.MaxValue]</i> (including <i>int.MinValue</i> and <i>int.MaxValue</i>).
            /// Currently the engine is <see cref="Cern.Jet.Random.Engine.MersenneTwister"/>
            /// and is seeded with the current time.
            /// <p>
            /// Note that any random engine derived from <see cref="Cern.Jet.Random.Engine.RandomEngine"/> and any random distribution derived from <see cref="Cern.Jet.Random.AbstractDistribution"/> are function objects, because they implement the proper interfaces.
            /// Thus, if you are not happy with the default, just pass your favourite random generator to function evaluating methods.
            /// </summary>
            public static IntFunction Random()
            {
                return new Cern.Jet.Random.Engine.MersenneTwister(new DateTime()).ApplyIntFunction();
            }
            /// <summary>
            /// Constructs a function that returns <i>a << b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunction ShiftLeft(int b)
            {
                return new IntFunction((a) => { return a << b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a >> b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunction ShiftRightSigned(int b)
            {
                return new IntFunction((a) => { return a >> b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a >>> b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunction ShiftRightUnsigned(int b)
            {
                return new IntFunction((a) => { return (int)UInt64.Parse(a.ToString()) >> b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>function(b,a)</i>, i.ed applies the function with the first operand as second operand and the second operand as first operand.
            /// </summary>
            /// <param name="function">a function taking operands in the form <i>function(a,b)</i>.</param>
            /// <param name="c"></param>
            /// <returns>the binary function <i>function(b,a)</i>.</returns>
            public static IntIntFunction SwapArgs(IntIntFunction function)
            {
                return new IntIntFunction((a, b) => { return function(b, a); });
            }
            /// <summary>
            /// Constructs a function that returns <i>a | b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunction Xor(int b)
            {
                return new IntFunction((a) => { return a ^ b; });
            }
        }

        public class BinaryFunctions
        {
            /// <summary>***************************
            /// <H3>Binary functions</H3>
            ///***************************/// </summary>

            /// <summary>
            /// Function that returns <i>a & b</i>.
            /// </summary>
            public static IntIntFunction And = new IntIntFunction((a, b) => { return a & b; });

            /// <summary>
            /// Function that returns <i>a < b ? -1 : a > b ? 1 : 0</i>.
            /// </summary>
            public static IntIntFunction Compare = new IntIntFunction((a, b) => { return a < b ? -1 : a > b ? 1 : 0; });

            /// <summary>
            /// Function that returns <i>a / b</i>.
            /// </summary>
            public static IntIntFunction Div = new IntIntFunction((a, b) => { return a / b; });

            /// <summary>
            /// Function that returns <i>a == b ? 1 : 0</i>.
            /// </summary>
            public static new IntIntFunction Equals = new IntIntFunction((a, b) => { return a == b ? 1 : 0; });

            /// <summary>
            /// Function that returns <i>a == b</i>.
            /// </summary>
            public static IntIntProcedure IsEqual = new IntIntProcedure((a, b) => { return a == b; });

            /// <summary>
            /// Function that returns <i>a < b</i>.
            /// </summary>
            public static IntIntProcedure IsLess = new IntIntProcedure((a, b) => { return a < b; });

            /// <summary>
            /// Function that returns <i>a > b</i>.
            /// </summary>
            public static IntIntProcedure IsGreater = new IntIntProcedure((a, b) => { return a > b; });

            /// <summary>
            /// Function that returns <i>System.Math.Max(a,b)</i>.
            /// </summary>
            public static IntIntFunction Max = new IntIntFunction((a, b) => { return (a >= b) ? a : b; });

            /// <summary>
            /// Function that returns <i>System.Math.Min(a,b)</i>.
            /// </summary>
            public static IntIntFunction Min = new IntIntFunction((a, b) => { return (a <= b) ? a : b; });

            /// <summary>
            /// Function that returns <i>a - b</i>.
            /// </summary>
            public static IntIntFunction Minus = new IntIntFunction((a, b) => { return a - b; });

            /// <summary>
            /// Function that returns <i>a % b</i>.
            /// </summary>
            public static IntIntFunction Mod = new IntIntFunction((a, b) => { return a % b; });

            /// <summary>
            /// Function that returns <i>a * b</i>.
            /// </summary>
            public static IntIntFunction Mult = new IntIntFunction((a, b) => { return a * b; });

            /// <summary>
            /// Function that returns <i>a | b</i>.
            /// </summary>
            public static IntIntFunction Or = new IntIntFunction((a, b) => { return a | b; });

            /// <summary>
            /// Function that returns <i>a + b</i>.
            /// </summary>
            public static IntIntFunction Plus = new IntIntFunction((a, b) => { return a + b; });

            /// <summary>
            /// Function that returns <i>(int) System.Math.Pow(a,b)</i>.
            /// </summary>
            public static IntIntFunction Pow = new IntIntFunction((a, b) => { return (int)System.Math.Pow(a, b); });

            /// <summary>
            /// Function that returns <i>a %lt;%lt; b</i>.
            /// </summary>
            public static IntIntFunction ShiftLeft = new IntIntFunction((a, b) => { return a << b; });


            /// <summary>
            /// Function that returns <i>a >> b</i>.
            /// </summary>
            public static IntIntFunction ShiftRightSigned = new IntIntFunction((a, b) => { return a >> b; });

            /// <summary>
            /// Function that returns <i>a >>> b</i>.
            /// </summary>
            public static IntIntFunction ShiftRightUnsigned = new IntIntFunction((a, b) => { return (int)UInt64.Parse(a.ToString()) >> b; });

            /// <summary>
            /// Function that returns <i>a ^ b</i>.
            /// </summary>
            public static IntIntFunction Xor = new IntIntFunction((a, b) => { return a ^ b; });

            /// <summary>
            /// Constructs the function <i>g( h(a,b) )</i>.
            /// </summary>
            /// <param name="g">a unary function.</param>
            /// <param name="h">a binary function.</param>
            /// <returns>the unary function <i>g( h(a,b) )</i>.</returns>
            public static IntIntFunction Chain(IntFunction g, IntIntFunction h)
            {
                return new IntIntFunction((a, b) => { return g(h(a, b)); });
            }
            /// <summary>
            /// Constructs the function <i>f( g(a), h(b) )</i>.
            /// </summary>
            /// <param name="f">a binary function.</param>
            /// <param name="g">a unary function.</param>
            /// <param name="h">a unary function.</param>
            /// <returns>the binary function <i>f( g(a), h(b) )</i>.</returns>
            public static IntIntFunction Chain(IntIntFunction f, IntFunction g, IntFunction h)
            {
                return new IntIntFunction((a, b) => { return f(g(a), h(b)); });
            }

        }

        #region Local Public Methods




        #endregion

        #region Local Private Methods

        #endregion

    }
}
