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
            public static IntFunctionDelegate Abs = new IntFunctionDelegate((a) => { return (a < 0) ? -a : a; });

            /// <summary>
            /// Function that returns <i>a--</i>.
            /// </summary>
            public static IntFunctionDelegate Dec = new IntFunctionDelegate((a) => { return a--; });

            /// <summary>
            /// Function that returns <i>(int) Arithmetic.factorial(a)</i>.
            /// </summary>
            public static IntFunctionDelegate Factorial = new IntFunctionDelegate((a) => { return (int)Arithmetic.Factorial(a); });

            /// <summary>
            /// Function that returns its argument.
            /// </summary>
            public static IntFunctionDelegate Identity = new IntFunctionDelegate((a) => { return a; });

            /// <summary>
            /// Function that returns <i>a++</i>.
            /// </summary>
            public static IntFunctionDelegate Inc = new IntFunctionDelegate((a) => { return a++; });

            /// <summary>
            /// Function that returns <i>-a</i>.
            /// </summary>
            public static IntFunctionDelegate Neg = new IntFunctionDelegate((a) => { return -a; });

            /// <summary>
            /// Function that returns <i>~a</i>.
            /// </summary>
            public static IntFunctionDelegate Not = new IntFunctionDelegate((a) => { return ~a; });

            /// <summary>
            /// Function that returns <i>a < 0 ? -1 : a > 0 ? 1 : 0</i>.
            /// </summary>
            public static IntFunctionDelegate Sign = new IntFunctionDelegate((a) => { return a < 0 ? -1 : a > 0 ? 1 : 0; });

            /// <summary>
            /// Function that returns <i>a/// a</i>.
            /// </summary>
            public static IntFunctionDelegate Square = new IntFunctionDelegate((a) => { return a * a; });

            /// <summary>
            /// Constructs a function that returns <i>a & b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunctionDelegate And(int b)
            {
                return new IntFunctionDelegate((a) => { return a & b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>(from %lt;= a && a %lt;= to) ? 1 : 0</i>.
            /// <i>a</i> is a variable, <i>from</i> and <i>to</i> are fixed.
            /// </summary>
            public static IntFunctionDelegate Between(int from, int to)
            {
                return new IntFunctionDelegate((a) => { return (from <= a && a <= to) ? 1 : 0; });
            }

            /// <summary>
            /// Constructs a unary function from a binary function with the first operand (argument) fixed to the given constant <i>c</i>.
            /// The second operand is variable (free).
            /// </summary>
            /// <param name="function">a binary function taking operands in the form <i>function(c,var)</i>.</param>
            /// <param name="c"></param>
            /// <returns>the unary function <i>function(c,var)</i>.</returns>
            public static IntFunctionDelegate BindArg1(IntIntFunctionDelegate function, int c)
            {
                return new IntFunctionDelegate((a) => { return function(c, a); });
            }
            /// <summary>
            /// Constructs a unary function from a binary function with the second operand (argument) fixed to the given constant <i>c</i>.
            /// The first operand is variable (free).
            /// </summary>
            /// <param name="function">a binary function taking operands in the form <i>function(var,c)</i>.</param>
            /// <param name="c"></param>
            /// <returns>the unary function <i>function(var,c)</i>.</returns>
            public static IntFunctionDelegate BindArg2(IntIntFunctionDelegate function, int c)
            {
                return new IntFunctionDelegate((a) => { return function(a, c); });
            }
            /// <summary>
            /// Constructs the function <i>g( h(a) )</i>.
            /// </summary>
            /// <param name="g">a unary function.</param>
            /// <param name="h">a unary function.</param>
            /// <returns>the unary function <i>g( h(a) )</i>.</returns>
            public static IntFunctionDelegate Chain(IntFunctionDelegate g, IntFunctionDelegate h)
            {
                return new IntFunctionDelegate((a) => { return g(h(a)); });
            }
            /// <summary>
            /// Constructs a function that returns <i>a < b ? -1 : a > b ? 1 : 0</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunctionDelegate Compare(int b)
            {
                return new IntFunctionDelegate((a) => { return a < b ? -1 : a > b ? 1 : 0; });
            }
            /// <summary>
            /// Constructs a function that returns the constant <i>c</i>.
            /// </summary>
            public static IntFunctionDelegate Constant(int c)
            {
                return new IntFunctionDelegate((a) => { return c; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a / b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunctionDelegate Div(int b)
            {
                return new IntFunctionDelegate((a) => { return a / b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a == b ? 1 : 0</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunctionDelegate Equals(int b)
            {
                return new IntFunctionDelegate((a) => { return a == b ? 1 : 0; });
            }
            /// <summary>
            /// Constructs a function that returns <i>from<=a && a<=to</i>.
            /// <i>a</i> is a variable, <i>from</i> and <i>to</i> are fixed.
            /// </summary>
            public static IntProcedureDelegate IsBetween(int from, int to)
            {
                return new IntProcedureDelegate((a) => { return from <= a && a <= to; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a == b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntProcedureDelegate IsEqual(int b)
            {
                return new IntProcedureDelegate((a) => { return a == b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a > b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntProcedureDelegate IsGreater(int b)
            {
                return new IntProcedureDelegate((a) => { return a > b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a < b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntProcedureDelegate IsLess(int b)
            {
                return new IntProcedureDelegate((a) => { return a < b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>System.Math.Max(a,b)</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunctionDelegate Max(int b)
            {
                return new IntFunctionDelegate((a) => { return (a >= b) ? a : b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>System.Math.Min(a,b)</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunctionDelegate Min(int b)
            {
                return new IntFunctionDelegate((a) => { return (a <= b) ? a : b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a - b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunctionDelegate Minus(int b)
            {
                return new IntFunctionDelegate((a) => { return a - b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a % b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunctionDelegate Mod(int b)
            {
                return new IntFunctionDelegate((a) => { return a % b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a * b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunctionDelegate Mult(int b)
            {
                return new IntFunctionDelegate((a) =>
                {
                    return a * b;
                });
            }
            /// <summary>
            /// Constructs a function that returns <i>a | b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunctionDelegate Or(int b)
            {
                return new IntFunctionDelegate((a) => { return a | b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a + b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunctionDelegate Plus(int b)
            {
                return new IntFunctionDelegate((a) => { return a + b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>(int) System.Math.Pow(a,b)</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunctionDelegate Pow(int b)
            {
                return new IntFunctionDelegate((a) => { return (int)System.Math.Pow(a, b); });
            }
            /// <summary>
            /// Constructs a function that returns a 32 bit uniformly distributed random number in the closed interval <i>[int.MinValue,int.MaxValue]</i> (including <i>int.MinValue</i> and <i>int.MaxValue</i>).
            /// Currently the engine is <see cref="Cern.Jet.Random.Engine.MersenneTwister"/>
            /// and is seeded with the current time.
            /// <p>
            /// Note that any random engine derived from <see cref="Cern.Jet.Random.Engine.RandomEngine"/> and any random distribution derived from <see cref="Cern.Jet.Random.AbstractDistribution"/> are function objects, because they implement the proper interfaces.
            /// Thus, if you are not happy with the default, just pass your favourite random generator to function evaluating methods.
            /// </summary>
            public static IntFunctionDelegate Random()
            {
                return new Cern.Jet.Random.Engine.MersenneTwister(new DateTime()).ApplyIntFunction();
            }
            /// <summary>
            /// Constructs a function that returns <i>a << b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunctionDelegate ShiftLeft(int b)
            {
                return new IntFunctionDelegate((a) => { return a << b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a >> b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunctionDelegate ShiftRightSigned(int b)
            {
                return new IntFunctionDelegate((a) => { return a >> b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>a >>> b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunctionDelegate ShiftRightUnsigned(int b)
            {
                return new IntFunctionDelegate((a) => { return (int)UInt64.Parse(a.ToString()) >> b; });
            }
            /// <summary>
            /// Constructs a function that returns <i>function(b,a)</i>, i.ed applies the function with the first operand as second operand and the second operand as first operand.
            /// </summary>
            /// <param name="function">a function taking operands in the form <i>function(a,b)</i>.</param>
            /// <param name="c"></param>
            /// <returns>the binary function <i>function(b,a)</i>.</returns>
            public static IntIntFunctionDelegate SwapArgs(IntIntFunctionDelegate function)
            {
                return new IntIntFunctionDelegate((a, b) => { return function(b, a); });
            }
            /// <summary>
            /// Constructs a function that returns <i>a | b</i>.
            /// <i>a</i> is a variable, <i>b</i> is fixed.
            /// </summary>
            public static IntFunctionDelegate Xor(int b)
            {
                return new IntFunctionDelegate((a) => { return a ^ b; });
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
            public static IntIntFunctionDelegate And = new IntIntFunctionDelegate((a, b) => { return a & b; });

            /// <summary>
            /// Function that returns <i>a < b ? -1 : a > b ? 1 : 0</i>.
            /// </summary>
            public static IntIntFunctionDelegate Compare = new IntIntFunctionDelegate((a, b) => { return a < b ? -1 : a > b ? 1 : 0; });

            /// <summary>
            /// Function that returns <i>a / b</i>.
            /// </summary>
            public static IntIntFunctionDelegate Div = new IntIntFunctionDelegate((a, b) => { return a / b; });

            /// <summary>
            /// Function that returns <i>a == b ? 1 : 0</i>.
            /// </summary>
            public static new IntIntFunctionDelegate Equals = new IntIntFunctionDelegate((a, b) => { return a == b ? 1 : 0; });

            /// <summary>
            /// Function that returns <i>a == b</i>.
            /// </summary>
            public static IntIntProcedureDelegate IsEqual = new IntIntProcedureDelegate((a, b) => { return a == b; });

            /// <summary>
            /// Function that returns <i>a < b</i>.
            /// </summary>
            public static IntIntProcedureDelegate IsLess = new IntIntProcedureDelegate((a, b) => { return a < b; });

            /// <summary>
            /// Function that returns <i>a > b</i>.
            /// </summary>
            public static IntIntProcedureDelegate IsGreater = new IntIntProcedureDelegate((a, b) => { return a > b; });

            /// <summary>
            /// Function that returns <i>System.Math.Max(a,b)</i>.
            /// </summary>
            public static IntIntFunctionDelegate Max = new IntIntFunctionDelegate((a, b) => { return (a >= b) ? a : b; });

            /// <summary>
            /// Function that returns <i>System.Math.Min(a,b)</i>.
            /// </summary>
            public static IntIntFunctionDelegate Min = new IntIntFunctionDelegate((a, b) => { return (a <= b) ? a : b; });

            /// <summary>
            /// Function that returns <i>a - b</i>.
            /// </summary>
            public static IntIntFunctionDelegate Minus = new IntIntFunctionDelegate((a, b) => { return a - b; });

            /// <summary>
            /// Function that returns <i>a % b</i>.
            /// </summary>
            public static IntIntFunctionDelegate Mod = new IntIntFunctionDelegate((a, b) => { return a % b; });

            /// <summary>
            /// Function that returns <i>a * b</i>.
            /// </summary>
            public static IntIntFunctionDelegate Mult = new IntIntFunctionDelegate((a, b) => { return a * b; });

            /// <summary>
            /// Function that returns <i>a | b</i>.
            /// </summary>
            public static IntIntFunctionDelegate Or = new IntIntFunctionDelegate((a, b) => { return a | b; });

            /// <summary>
            /// Function that returns <i>a + b</i>.
            /// </summary>
            public static IntIntFunctionDelegate Plus = new IntIntFunctionDelegate((a, b) => { return a + b; });

            /// <summary>
            /// Function that returns <i>(int) System.Math.Pow(a,b)</i>.
            /// </summary>
            public static IntIntFunctionDelegate Pow = new IntIntFunctionDelegate((a, b) => { return (int)System.Math.Pow(a, b); });

            /// <summary>
            /// Function that returns <i>a %lt;%lt; b</i>.
            /// </summary>
            public static IntIntFunctionDelegate ShiftLeft = new IntIntFunctionDelegate((a, b) => { return a << b; });


            /// <summary>
            /// Function that returns <i>a >> b</i>.
            /// </summary>
            public static IntIntFunctionDelegate ShiftRightSigned = new IntIntFunctionDelegate((a, b) => { return a >> b; });

            /// <summary>
            /// Function that returns <i>a >>> b</i>.
            /// </summary>
            public static IntIntFunctionDelegate ShiftRightUnsigned = new IntIntFunctionDelegate((a, b) => { return (int)UInt64.Parse(a.ToString()) >> b; });

            /// <summary>
            /// Function that returns <i>a ^ b</i>.
            /// </summary>
            public static IntIntFunctionDelegate Xor = new IntIntFunctionDelegate((a, b) => { return a ^ b; });

            /// <summary>
            /// Constructs the function <i>g( h(a,b) )</i>.
            /// </summary>
            /// <param name="g">a unary function.</param>
            /// <param name="h">a binary function.</param>
            /// <returns>the unary function <i>g( h(a,b) )</i>.</returns>
            public static IntIntFunctionDelegate Chain(IntFunctionDelegate g, IntIntFunctionDelegate h)
            {
                return new IntIntFunctionDelegate((a, b) => { return g(h(a, b)); });
            }
            /// <summary>
            /// Constructs the function <i>f( g(a), h(b) )</i>.
            /// </summary>
            /// <param name="f">a binary function.</param>
            /// <param name="g">a unary function.</param>
            /// <param name="h">a unary function.</param>
            /// <returns>the binary function <i>f( g(a), h(b) )</i>.</returns>
            public static IntIntFunctionDelegate Chain(IntIntFunctionDelegate f, IntFunctionDelegate g, IntFunctionDelegate h)
            {
                return new IntIntFunctionDelegate((a, b) => { return f(g(a), h(b)); });
            }

        }

        #region Local Public Methods




        #endregion

        #region Local Private Methods

        #endregion

    }
}
