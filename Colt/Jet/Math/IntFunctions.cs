using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #region Local Public Methods
        public static IntFunction abs = new IntFunction()
        {

        public int apply(int a) { return (a < 0) ? -a : a; }
    };

    /**
	 * Function that returns <tt>a--</tt>.
	 */
    public static IntFunction dec = new IntFunction()
    {

        public int apply(int a) { return a--; }
};

/**
 * Function that returns <tt>(int) Arithmetic.factorial(a)</tt>.
 */
public static IntFunction factorial = new IntFunction()
{

        public int apply(int a) { return (int)Arithmetic.factorial(a); }
	};
		
	/**
	 * Function that returns its argument.
	 */
	public static IntFunction identity = new IntFunction()
{

        public int apply(int a) { return a; }   
	};

	/**
	 * Function that returns <tt>a++</tt>.
	 */
	public static IntFunction inc = new IntFunction()
{

        public int apply(int a) { return a++; }   
	};

	/**
	 * Function that returns <tt>-a</tt>.
	 */
	public static IntFunction neg = new IntFunction()
{

        public int apply(int a) { return -a; }
	};
		
	/**
	 * Function that returns <tt>~a</tt>.
	 */
	public static IntFunction not = new IntFunction()
{

        public int apply(int a) { return ~a; }
	};
		
	/**
	 * Function that returns <tt>a < 0 ? -1 : a > 0 ? 1 : 0</tt>.
	 */
	public static IntFunction sign = new IntFunction()
{

        public int apply(int a) { return a < 0 ? -1 : a > 0 ? 1 : 0; }
	};
		
	/**
	 * Function that returns <tt>a * a</tt>.
	 */
	public static IntFunction square = new IntFunction()
{

        public int apply(int a) { return a * a; }
	};
		





	/*****************************
	 * <H3>Binary functions</H3>
	 *****************************/
		
	/**
	 * Function that returns <tt>a & b</tt>.
	 */
	public static IntIntFunction and = new IntIntFunction()
{

        public int apply(int a, int b) { return a & b; }
	};
		
	/**
	 * Function that returns <tt>a < b ? -1 : a > b ? 1 : 0</tt>.
	 */
	public static IntIntFunction compare = new IntIntFunction()
{

        public int apply(int a, int b) { return a < b ? -1 : a > b ? 1 : 0; }
	};
		
	/**
	 * Function that returns <tt>a / b</tt>.
	 */
	public static IntIntFunction div = new IntIntFunction()
{

        public int apply(int a, int b) { return a / b; }
	};
		
	/**
	 * Function that returns <tt>a == b ? 1 : 0</tt>.
	 */
	public static IntIntFunction equals = new IntIntFunction()
{

        public int apply(int a, int b) { return a == b ? 1 : 0; }
	};
		
	/**
	 * Function that returns <tt>a == b</tt>.
	 */
	public static IntIntProcedure isEqual = new IntIntProcedure()
{

        public Boolean apply(int a, int b) { return a == b; }
	};		

	/**
	 * Function that returns <tt>a < b</tt>.
	 */
	public static IntIntProcedure isLess = new IntIntProcedure()
{

        public Boolean apply(int a, int b) { return a < b; }
	};		

	/**
	 * Function that returns <tt>a > b</tt>.
	 */
	public static IntIntProcedure isGreater = new IntIntProcedure()
{

        public Boolean apply(int a, int b) { return a > b; }
	};		

	/**
	 * Function that returns <tt>System.Math.Max(a,b)</tt>.
	 */
	public static IntIntFunction max = new IntIntFunction()
{

        public int apply(int a, int b) { return (a >= b) ? a : b; }
	};
		
	/**
	 * Function that returns <tt>System.Math.Min(a,b)</tt>.
	 */
	public static IntIntFunction min = new IntIntFunction()
{

        public int apply(int a, int b) { return (a <= b) ? a : b; }
	};
		
	/**
	 * Function that returns <tt>a - b</tt>.
	 */
	public static IntIntFunction minus = new IntIntFunction()
{

        public int apply(int a, int b) { return a - b; }
	};
		
	/**
	 * Function that returns <tt>a % b</tt>.
	 */
	public static IntIntFunction mod = new IntIntFunction()
{

        public int apply(int a, int b) { return a % b; }
	};
		
	/**
	 * Function that returns <tt>a * b</tt>.
	 */
	public static IntIntFunction mult = new IntIntFunction()
{

        public int apply(int a, int b) { return a * b; }
	};
		
	/**
	 * Function that returns <tt>a | b</tt>.
	 */
	public static IntIntFunction or = new IntIntFunction()
{

        public int apply(int a, int b) { return a | b; }
	};
		
	/**
	 * Function that returns <tt>a + b</tt>.
	 */
	public static IntIntFunction plus = new IntIntFunction()
{

        public int apply(int a, int b) { return a + b; }
	};
		
	/**
	 * Function that returns <tt>(int) System.Math.Pow(a,b)</tt>.
	 */
	public static IntIntFunction pow = new IntIntFunction()
{

        public int apply(int a, int b) { return (int)System.Math.Pow(a, b); }
	};
	
	/**
	 * Function that returns <tt>a << b</tt>.
	 */
	public static IntIntFunction shiftLeft = new IntIntFunction()
{

        public int apply(int a, int b) { return a << b; }
	};
		
	
	/**
	 * Function that returns <tt>a >> b</tt>.
	 */
	public static IntIntFunction shiftRightSigned = new IntIntFunction()
{

        public int apply(int a, int b) { return a >> b; }
	};
		
	/**
	 * Function that returns <tt>a >>> b</tt>.
	 */
	public static IntIntFunction shiftRightUnsigned = new IntIntFunction()
{

        public int apply(int a, int b) { return a >>> b; }
	};
		
	/**
	 * Function that returns <tt>a ^ b</tt>.
	 */
	public static IntIntFunction xor = new IntIntFunction()
{

        public int apply(int a, int b) { return a ^ b; }
	};

/**
 * Constructs a function that returns <tt>a & b</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntFunction and(int b)
{
    return new IntFunction()
    {

        public int apply(int a) { return a & b; }
	};
}
/**
 * Constructs a function that returns <tt>(from<=a && a<=to) ? 1 : 0</tt>.
 * <tt>a</tt> is a variable, <tt>from</tt> and <tt>to</tt> are fixed.
 */
public static IntFunction between(int from, int to)
{
    return new IntFunction()
    {

        public int apply(int a) { return (from <= a && a <= to) ? 1 : 0; }
	};
}
/**
 * Constructs a unary function from a binary function with the first operand (argument) fixed to the given constant <tt>c</tt>.
 * The second operand is variable (free).
 * 
 * @param function a binary function taking operands in the form <tt>function.apply(c,var)</tt>.
 * @return the unary function <tt>function(c,var)</tt>.
 */
public static IntFunction bindArg1(IntIntFunction function, int c)
{
    return new IntFunction()
    {

        public int apply(int var) { return function.apply(c, var); }
	};
}
/**
 * Constructs a unary function from a binary function with the second operand (argument) fixed to the given constant <tt>c</tt>.
 * The first operand is variable (free).
 * 
 * @param function a binary function taking operands in the form <tt>function.apply(var,c)</tt>.
 * @return the unary function <tt>function(var,c)</tt>.
 */
public static IntFunction bindArg2(IntIntFunction function, int c)
{
    return new IntFunction()
    {

        public int apply(int var) { return function.apply(var, c); }
	};
}
/**
 * Constructs the function <tt>g( h(a) )</tt>.
 * 
 * @param g a unary function.
 * @param h a unary function.
 * @return the unary function <tt>g( h(a) )</tt>.
 */
public static IntFunction chain(IntFunction g, IntFunction h)
{
    return new IntFunction()
    {

        public int apply(int a) { return g.apply(h.apply(a)); }
	};
}
/**
 * Constructs the function <tt>g( h(a,b) )</tt>.
 * 
 * @param g a unary function.
 * @param h a binary function.
 * @return the unary function <tt>g( h(a,b) )</tt>.
 */
public static IntIntFunction chain(IntFunction g, IntIntFunction h)
{
    return new IntIntFunction()
    {

        public int apply(int a, int b) { return g.apply(h.apply(a, b)); }
	};
}
/**
 * Constructs the function <tt>f( g(a), h(b) )</tt>.
 * 
 * @param f a binary function.
 * @param g a unary function.
 * @param h a unary function.
 * @return the binary function <tt>f( g(a), h(b) )</tt>.
 */
public static IntIntFunction chain(IntIntFunction f, IntFunction g, IntFunction h)
{
    return new IntIntFunction()
    {

        public int apply(int a, int b) { return f.apply(g.apply(a), h.apply(b)); }
	};
}
/**
 * Constructs a function that returns <tt>a < b ? -1 : a > b ? 1 : 0</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntFunction compare(int b)
{
    return new IntFunction()
    {

        public int apply(int a) { return a < b ? -1 : a > b ? 1 : 0; }
	};
}
/**
 * Constructs a function that returns the constant <tt>c</tt>.
 */
public static IntFunction constant(int c)
{
    return new IntFunction()
    {

        public int apply(int a) { return c; }
	};
}
/**
 * Constructs a function that returns <tt>a / b</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntFunction div(int b)
{
    return new IntFunction()
    {

        public int apply(int a) { return a / b; }
	};
}
/**
 * Constructs a function that returns <tt>a == b ? 1 : 0</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntFunction Equals(int b)
{
    return new IntFunction()
    {

        public int apply(int a) { return a == b ? 1 : 0; }
	};
}
/**
 * Constructs a function that returns <tt>from<=a && a<=to</tt>.
 * <tt>a</tt> is a variable, <tt>from</tt> and <tt>to</tt> are fixed.
 */
public static IntProcedure isBetween(int from, int to)
{
    return new IntProcedure()
    {

        public Boolean apply(int a) { return from <= a && a <= to; }
	};
}
/**
 * Constructs a function that returns <tt>a == b</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntProcedure isEqual(int b)
{
    return new IntProcedure()
    {

        public Boolean apply(int a) { return a == b; }
	};
}
/**
 * Constructs a function that returns <tt>a > b</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntProcedure isGreater(int b)
{
    return new IntProcedure()
    {

        public Boolean apply(int a) { return a > b; }
	};
}
/**
 * Constructs a function that returns <tt>a < b</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntProcedure isLess(int b)
{
    return new IntProcedure()
    {

        public Boolean apply(int a) { return a < b; }
	};
}
/**
 * Constructs a function that returns <tt>System.Math.Max(a,b)</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntFunction max(int b)
{
    return new IntFunction()
    {

        public int apply(int a) { return (a >= b) ? a : b; }
	};
}
/**
 * Constructs a function that returns <tt>System.Math.Min(a,b)</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntFunction min(int b)
{
    return new IntFunction()
    {

        public int apply(int a) { return (a <= b) ? a : b; }
	};
}
/**
 * Constructs a function that returns <tt>a - b</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntFunction minus(int b)
{
    return new IntFunction()
    {

        public int apply(int a) { return a - b; }
	};
}
/**
 * Constructs a function that returns <tt>a % b</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntFunction mod(int b)
{
    return new IntFunction()
    {

        public int apply(int a) { return a % b; }
	};
}
/**
 * Constructs a function that returns <tt>a * b</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntFunction mult(int b)
{
    return new IntFunction()
    {

        public int apply(int a) { return a * b; }
	};
}
/**
 * Constructs a function that returns <tt>a | b</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntFunction or(int b)
{
    return new IntFunction()
    {

        public int apply(int a) { return a | b; }
	};
}
/**
 * Constructs a function that returns <tt>a + b</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntFunction plus(int b)
{
    return new IntFunction()
    {

        public int apply(int a) { return a + b; }
	};
}
/**
 * Constructs a function that returns <tt>(int) System.Math.Pow(a,b)</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntFunction pow(int b)
{
    return new IntFunction()
    {

        public int apply(int a) { return (int)System.Math.Pow(a, b); }
	};
}
/**
 * Constructs a function that returns a 32 bit uniformly distributed random number in the closed interval <tt>[int.MinValue,int.MaxValue]</tt> (including <tt>int.MinValue</tt> and <tt>int.MaxValue</tt>).
 * Currently the engine is {@link cern.Cern.Jet.random.engine.MersenneTwister}
 * and is seeded with the current time.
 * <p>
 * Note that any random engine derived from {@link cern.Cern.Jet.random.engine.RandomEngine} and any random distribution derived from {@link cern.Cern.Jet.random.AbstractDistribution} are function objects, because they implement the proper interfaces.
 * Thus, if you are not happy with the default, just pass your favourite random generator to function evaluating methods.
 */
public static IntFunction random()
{
    return new cern.Cern.Jet.random.engine.MersenneTwister(new java.util.Date());
}
/**
 * Constructs a function that returns <tt>a << b</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntFunction shiftLeft(int b)
{
    return new IntFunction()
    {

        public int apply(int a) { return a << b; }
	};
}
/**
 * Constructs a function that returns <tt>a >> b</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntFunction shiftRightSigned(int b)
{
    return new IntFunction()
    {

        public int apply(int a) { return a >> b; }
	};
}
/**
 * Constructs a function that returns <tt>a >>> b</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntFunction shiftRightUnsigned(int b)
{
    return new IntFunction()
    {

        public int apply(int a) { return a >>> b; }
	};
}
/**
 * Constructs a function that returns <tt>function.apply(b,a)</tt>, i.ed applies the function with the first operand as second operand and the second operand as first operand.
 * 
 * @param function a function taking operands in the form <tt>function.apply(a,b)</tt>.
 * @return the binary function <tt>function(b,a)</tt>.
 */
public static IntIntFunction swapArgs(IntIntFunction function)
{
    return new IntIntFunction()
    {

        public int apply(int a, int b) { return function.apply(b, a); }
	};
}
/**
 * Constructs a function that returns <tt>a | b</tt>.
 * <tt>a</tt> is a variable, <tt>b</tt> is fixed.
 */
public static IntFunction xor(int b)
{
    return new IntFunction()
    {

        public int apply(int a) { return a ^ b; }
	};
}

        #endregion

        #region Local Private Methods

        #endregion

    }
}
