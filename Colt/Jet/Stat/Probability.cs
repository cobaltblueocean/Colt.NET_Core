using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Jet.Math;

namespace Cern.Jet.Stat
{
    /// <summary>
    /// Custom tailored numerical integration of certain probability distributions.
    /// 
    /// Implementation:
    /// Some code taken and adapted from the <A HREF="http://www.sci.usq.edu.au/staff/leighb/graph/Top.html">Java 2D Graph Package 2.4</A>,
    /// which in turn is a port from the<A HREF="http://people.ne.mediaone.net/moshier/index.html#Cephes">Cephes 2.2</A> Math Library (C).
    /// Most Cephes code(missing from the 2D Graph Package) directly ported.
    /// </summary>
    public class Probability : Cern.Jet.Math.Constants
    {

        #region Local Variables
        #region COEFFICIENTS FOR METHOD  normalInverse()

        /// <summary>
        /// approximation for 0 <= |y - 0.5| <= 3/8
        /// </summary>
        protected static double[] P0 = {
        -5.99633501014107895267E1,
         9.80010754185999661536E1,
        -5.66762857469070293439E1,
         1.39312609387279679503E1,
        -1.23916583867381258016E0,
        };
        protected static double[] Q0 = {
		/* 1.00000000000000000000E0,*/
		 1.95448858338141759834E0,
         4.67627912898881538453E0,
         8.63602421390890590575E1,
        -2.25462687854119370527E2,
         2.00260212380060660359E2,
        -8.20372256168333339912E1,
         1.59056225126211695515E1,
        -1.18331621121330003142E0,
        };

        /// <summary>
        /// Approximation for interval z = sqrt(-2 log y ) between 2 and 8
        /// i.ed, y between exp(-2) = .135 and exp(-32) = 1.27e-14.
        /// </summary>
        protected static double[] P1 = {
         4.05544892305962419923E0,
         3.15251094599893866154E1,
         5.71628192246421288162E1,
         4.40805073893200834700E1,
         1.46849561928858024014E1,
         2.18663306850790267539E0,
        -1.40256079171354495875E-1,
        -3.50424626827848203418E-2,
        -8.57456785154685413611E-4,
        };
        protected static double[] Q1 = {
		/*  1.00000000000000000000E0,*/
		 1.57799883256466749731E1,
         4.53907635128879210584E1,
         4.13172038254672030440E1,
         1.50425385692907503408E1,
         2.50464946208309415979E0,
        -1.42182922854787788574E-1,
        -3.80806407691578277194E-2,
        -9.33259480895457427372E-4,
        };

        /// <summary>
        /// Approximation for interval z = sqrt(-2 log y ) between 8 and 64
        /// i.ed, y between exp(-32) = 1.27e-14 and exp(-2048) = 3.67e-890.
        /// </summary>
        protected static double[] P2 = {
          3.23774891776946035970E0,
          6.91522889068984211695E0,
          3.93881025292474443415E0,
          1.33303460815807542389E0,
          2.01485389549179081538E-1,
          1.23716634817820021358E-2,
          3.01581553508235416007E-4,
          2.65806974686737550832E-6,
          6.23974539184983293730E-9,
        };
        protected static double[] Q2 = {
		/*  1.00000000000000000000E0,*/
		  6.02427039364742014255E0,
          3.67983563856160859403E0,
          1.37702099489081330271E0,
          2.16236993594496635890E-1,
          1.34204006088543189037E-2,
          3.28014464682127739104E-4,
          2.89247864745380683936E-6,
          6.79019408009981274425E-9,
        };

        #endregion

        #endregion

        #region Property

        #endregion

        #region Constructor

        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected Probability() { }

        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods
        /// <summary>
        /// Returns the area from zero to <i>x</i> under the beta density
        /// function.
        /// <pre>
        ///                          x
        ///            -             -
        ///           | (a+b)       | |  a-1      b-1
        /// P(x)  =  ----------     |   t    (1-t)    dt
        ///           -     -     | |
        ///          | (a) | (b)   -
        ///                         0
        /// </pre>
        /// This function is identical to the incomplete beta
        /// integral function <i>Gamma.IncompleteBeta(a, b, x)</i>.
        ///
        /// The complemented function is
        ///
        /// <i>1 - P(1-x)  =  Gamma.IncompleteBeta( b, a, x )</i>;
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        static public double Beta(double a, double b, double x)
        {
            return Stat.Gamma.IncompleteBeta(a, b, x);
        }

        /// <summary>
        /// Returns the area under the right hand tail (from <i>x</i> to
        /// infinity) of the beta density function.
        /// 
        /// This function is identical to the incomplete beta
        /// integral function <i>Gamma.IncompleteBeta(b, a, x)</i>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        static public double BetaComplemented(double a, double b, double x)
        {
            return Stat.Gamma.IncompleteBeta(b, a, x);
        }

        /// <summary>
        /// Returns the sum of the terms <i>0</i> through <i>k</i> of the Binomial
        /// probability density.
        /// <pre>
        ///   k
        ///   --  ( n )   j      n-j
        ///   >   (   )  p  (1-p)
        ///   --  ( j )
        ///  j=0
        /// </pre>
        /// The terms are not summed directly; instead the incomplete
        /// beta integral is employed, according to the formula
        /// <p>
        /// <i>y = binomial( k, n, p ) = Gamma.IncompleteBeta( n-k, k+1, 1-p )</i>.
        /// <p>
        /// All arguments must be positive, 
        /// </summary>
        /// <param name="k">end term.</param>
        /// <param name="n">the number of trials.</param>
        /// <param name="p">the probability of success (must be in <i>(0.0,1.0)</i>).</param>
        /// <returns></returns>
        static public double Binomial(int k, int n, double p)
        {
            if ((p < 0.0) || (p > 1.0)) throw new ArgumentException();
            if ((k < 0) || (n < k)) throw new ArgumentException();

            if (k == n) return (1.0);
            if (k == 0) return System.Math.Pow(1.0 - p, n - k);

            return Stat.Gamma.IncompleteBeta(n - k, k + 1, 1.0 - p);
        }

        /// <summary>
        /// Returns the sum of the terms <i>k+1</i> through <i>n</i> of the Binomial
        /// probability density.
        /// <pre>
        ///   n
        ///   --  ( n )   j      n-j
        ///   >   (   )  p  (1-p)
        ///   --  ( j )
        ///  j=k+1
        /// </pre>
        /// The terms are not summed directly; instead the incomplete
        /// beta integral is employed, according to the formula
        /// <p>
        /// <i>y = BinomialComplemented( k, n, p ) = Gamma.IncompleteBeta( k+1, n-k, p )</i>.
        /// <p>
        /// All arguments must be positive, 
        /// </summary>
        /// <param name="k">end term.</param>
        /// <param name="n">the number of trials.</param>
        /// <param name="p">the probability of success (must be in <i>(0.0,1.0)</i>).</param>
        /// <returns></returns>
        static public double BinomialComplemented(int k, int n, double p)
        {
            if ((p < 0.0) || (p > 1.0)) throw new ArgumentException();
            if ((k < 0) || (n < k)) throw new ArgumentException();

            if (k == n) return (0.0);
            if (k == 0) return 1.0 - System.Math.Pow(1.0 - p, n - k);

            return Stat.Gamma.IncompleteBeta(k + 1, n - k, p);
        }

        /// <summary>
        /// Returns the area under the left hand tail (from 0 to <i>x</i>)
        /// of the Chi square probability density function with
        /// <i>v</i> degrees of freedom.
        /// <pre>
        ///                                  inf.
        ///                                    -
        ///                        1          | |  v/2-1  -t/2
        ///  P( x | v )   =   -----------     |   t      e     dt
        ///                    v/2  -       | |
        ///                   2    | (v/2)   -
        ///                                   x
        /// </pre>
        /// where <i>x</i> is the Chi-square variable.
        /// <p>
        /// The incomplete gamma integral is used, according to the
        /// formula
        /// <p>
        /// <i>y = ChiSquare( v, x ) = IncompleteGamma( v/2.0, x/2.0 )</i>.
        /// <p>
        /// The arguments must both be positive.
        /// </summary>
        /// <param name="v">degrees of freedom.</param>
        /// <param name="x">integration end point.</param>
        /// <returns></returns>
        static public double ChiSquare(double v, double x)
        {
            if (x < 0.0 || v < 1.0) return 0.0;
            return Stat.Gamma.IncompleteGamma(v / 2.0, x / 2.0);
        }

        /// <summary>
        /// Returns the area under the right hand tail (from <i>x</i> to
        /// infinity) of the Chi square probability density function
        /// with <i>v</i> degrees of freedom.
        /// <pre>
        ///                                  inf.
        ///                                    -
        ///                        1          | |  v/2-1  -t/2
        ///  P( x | v )   =   -----------     |   t      e     dt
        ///                    v/2  -       | |
        ///                   2    | (v/2)   -
        ///                                   x
        /// </pre>
        /// where <i>x</i> is the Chi-square variable.
        ///
        /// The incomplete gamma integral is used, according to the
        /// formula
        ///
        /// <i>y = ChiSquareComplemented( v, x ) = IncompleteGammaComplement( v/2.0, x/2.0 )</i>.
        ///
        ///
        /// The arguments must both be positive.
        /// </summary>
        /// <param name="v">degrees of freedom.</param>
        /// <param name="x">integration end point.</param>
        /// <returns></returns>
        static public double ChiSquareComplemented(double v, double x)
        {
            if (x < 0.0 || v < 1.0) return 0.0;
            return Stat.Gamma.IncompleteGammaComplement(v / 2.0, x / 2.0);
        }

        /// <summary>
        /// Returns the error function of the normal distribution; formerly named <i>erf</i>.
        /// The integral is
        /// <pre>
        ///                           x 
        ///                            -
        ///                 2         | |          2
        ///   erf(x)  =  --------     |    exp( - t  ) dt.
        ///              sqrt(pi)   | |
        ///                          -
        ///                           0
        /// </pre>
        /// <b>Implementation:</b>
        /// For <i>0 <= |x| < 1, erf(x) = x/// P4(x**2)/Q5(x**2)</i>; otherwise
        /// <i>erf(x) = 1 - erfc(x)</i>.
        /// <p>
        /// Code adapted from the <A HREF="http://www.sci.usq.edu.au/staff/leighb/graph/Top.html">Java 2D Graph Package 2.4</A>,
        /// which in turn is a port from the <A HREF="http://people.ne.mediaone.net/moshier/index.html#Cephes">Cephes 2.2</A> Math Library (C).
        /// </summary>
        /// <param name="x">the argument to the function.</param>
        /// <returns></returns>
        static public double ErrorFunction(double x)
        {

            double y, z;
            double[] T = {
                     9.60497373987051638749E0,
                     9.00260197203842689217E1,
                     2.23200534594684319226E3,
                     7.00332514112805075473E3,
                     5.55923013010394962768E4
                    };
            double[] U = {
				   //1.00000000000000000000E0,
					 3.35617141647503099647E1,
                     5.21357949780152679795E2,
                     4.59432382970980127987E3,
                     2.26290000613890934246E4,
                     4.92673942608635921086E4
                    };

            if (System.Math.Abs(x) > 1.0) return (1.0 - ErrorFunctionComplemented(x));
            z = x * x;
            y = x * Polynomial.Polevl(z, T, 4) / Polynomial.P1evl(z, U, 5);
            return y;
        }

        /// <summary>
        /// Returns the complementary Error function of the normal distribution; formerly named <i>erfc</i>.
        /// <pre>
        ///  1 - erf(x) =
        ///
        ///                           infd 
        ///                             -
        ///                  2         | |          2
        ///   erfc(x)  =  --------     |    exp( - t  ) dt
        ///               sqrt(pi)   | |
        ///                           -
        ///                            x
        /// </pre>
        /// <b>Implementation:</b>
        /// For small x, <i>erfc(x) = 1 - erf(x)</i>; otherwise rational
        /// approximations are computed.
        /// <p>
        /// Code adapted from the <A HREF="http://www.sci.usq.edu.au/staff/leighb/graph/Top.html">Java 2D Graph Package 2.4</A>,
        /// which in turn is a port from the <A HREF="http://people.ne.mediaone.net/moshier/index.html#Cephes">Cephes 2.2</A> Math Library (C).
        /// </summary>
        /// <param name="a">the argument to the function.</param>
        /// <returns></returns>
        static public double ErrorFunctionComplemented(double a)
        {
            double x, y, z, p, q;

            double[] P = {
            2.46196981473530512524E-10,
                     5.64189564831068821977E-1,
                     7.46321056442269912687E0,
                     4.86371970985681366614E1,
                     1.96520832956077098242E2,
                     5.26445194995477358631E2,
                     9.34528527171957607540E2,
                     1.02755188689515710272E3,
                     5.57535335369399327526E2

                    };
            double[] Q = {
            //1.0
            1.32281951154744992508E1,
                      8.67072140885989742329E1,
                      3.54937778887819891062E2,
                      9.75708501743205489753E2,
                      1.82390916687909736289E3,
                      2.24633760818710981792E3,
                      1.65666309194161350182E3,
                      5.57535340817727675546E2

                     };

            double[] R = {
            5.64189583547755073984E-1,
                      1.27536670759978104416E0,
                      5.01905042251180477414E0,
                      6.16021097993053585195E0,
                      7.40974269950448939160E0,
                      2.97886665372100240670E0

                     };
            double[] S = {
            //1.00000000000000000000E0, 
            2.26052863220117276590E0,
                      9.39603524938001434673E0,
                      1.20489539808096656605E1,
                      1.70814450747565897222E1,
                      9.60896809063285878198E0,
                      3.36907645100081516050E0

                     };

            if (a < 0.0) x = -a;
            else x = a;

            if (x < 1.0) return 1.0 - ErrorFunction(a);

            z = -a * a;

            if (z < -MAXLOG)
            {
                if (a < 0) return (2.0);
                else return (0.0);
            }

            z = System.Math.Exp(z);

            if (x < 8.0)
            {
                p = Polynomial.Polevl(x, P, 8);
                q = Polynomial.P1evl(x, Q, 8);
            }
            else
            {
                p = Polynomial.Polevl(x, R, 5);
                q = Polynomial.P1evl(x, S, 6);
            }

            y = (z * p) / q;

            if (a < 0) y = 2.0 - y;

            if (y == 0.0)
            {
                if (a < 0) return 2.0;
                else return (0.0);
            }

            return y;
        }

        /// <summary>
        /// Returns the integral from zero to <i>x</i> of the gamma probability
        /// density function.
        /// <pre>
        ///                x
        ///        b       -
        ///       a       | |   b-1  -at
        /// y =  -----    |    t    e    dt
        ///       -     | |
        ///      | (b)   -
        ///               0
        /// </pre>
        /// The incomplete gamma integral is used, according to the
        /// relation
        ///
        /// <i>y = Gamma.IncompleteGamma( b, a*x )</i>.
        /// </summary>
        /// <param name="a">the paramater a (alpha) of the gamma distribution.</param>
        /// <param name="b">the paramater b (beta, lambda) of the gamma distribution.</param>
        /// <param name="x">integration end point.</param>
        /// <returns></returns>
        static public double Gamma(double a, double b, double x)
        {
            if (x < 0.0) return 0.0;
            return Stat.Gamma.IncompleteGamma(b, a * x);
        }

        /// <summary>
        /// Returns the integral from <i>x</i> to infinity of the gamma
        /// probability density function:
        /// <pre>
        ///               inf.
        ///        b       -
        ///       a       | |   b-1  -at
        /// y =  -----    |    t    e    dt
        ///       -     | |
        ///      | (b)   -
        ///               x
        /// </pre>
        /// The incomplete gamma integral is used, according to the
        /// relation
        /// <p>
        /// y = Gamma.IncompleteGammaComplement( b, a*x ).
        /// </summary>
        /// <param name="a">the paramater a (alpha) of the gamma distribution.</param>
        /// <param name="b">the paramater b (beta, lambda) of the gamma distribution.</param>
        /// <param name="x">integration end point.</param>
        /// <returns></returns>
        static public double GammaComplemented(double a, double b, double x)
        {
            if (x < 0.0) return 0.0;
            return Stat.Gamma.IncompleteGammaComplement(b, a * x);
        }

        /// <summary>
        /// Returns the sum of the terms <i>0</i> through <i>k</i> of the Negative Binomial Distribution.
        /// <pre>
        ///   k
        ///   --  ( n+j-1 )   n      j
        ///   >   (       )  p  (1-p)
        ///   --  (   j   )
        ///  j=0
        /// </pre>
        /// In a sequence of Bernoulli trials, this is the probability
        /// that <i>k</i> or fewer failures precede the <i>n</i>-th success.
        /// <p>
        /// The terms are not computed individually; instead the incomplete
        /// beta integral is employed, according to the formula
        /// <p>
        /// <i>y = NegativeBinomial( k, n, p ) = Gamma.IncompleteBeta( n, k+1, p )</i>.
        /// 
        /// All arguments must be positive, 
        /// </summary>
        /// <param name="k">end term.</param>
        /// <param name="n">the number of trials.</param>
        /// <param name="p">the probability of success (must be in <i>(0.0,1.0)</i>).</param>
        /// <returns></returns>
        static public double NegativeBinomial(int k, int n, double p)
        {
            if ((p < 0.0) || (p > 1.0)) throw new ArgumentException();
            if (k < 0) return 0.0;

            return Stat.Gamma.IncompleteBeta(n, k + 1, p);
        }

        /// <summary>
        /// Returns the sum of the terms <i>k+1</i> to infinity of the Negative
        /// Binomial distribution.
        /// <pre>
        ///   inf
        ///   --  ( n+j-1 )   n      j
        ///   >   (       )  p  (1-p)
        ///   --  (   j   )
        ///  j=k+1
        /// </pre>
        /// The terms are not computed individually; instead the incomplete
        /// beta integral is employed, according to the formula
        /// <p>
        /// y = NegativeBinomialComplemented( k, n, p ) = Gamma.IncompleteBeta( k+1, n, 1-p ).
        ///
        /// All arguments must be positive, 
        /// </summary>
        /// <param name="k">end term.</param>
        /// <param name="n">the number of trials.</param>
        /// <param name="p">the probability of success (must be in <i>(0.0,1.0)</i>).</param>
        /// <returns></returns>
        static public double NegativeBinomialComplemented(int k, int n, double p)
        {
            if ((p < 0.0) || (p > 1.0)) throw new ArgumentException();
            if (k < 0) return 0.0;

            return Stat.Gamma.IncompleteBeta(k + 1, n, 1.0 - p);
        }

        /// <summary>
        /// Returns the area under the Normal (Gaussian) probability density
        /// function, integrated from minus infinity to <i>x</i> (assumes mean is zero, variance is one).
        /// <pre>
        ///                            x
        ///                             -
        ///                   1        | |          2
        ///  normal(x)  = ---------    |    exp( - t /2 ) dt
        ///               sqrt(2pi)  | |
        ///                           -
        ///                          -inf.
        ///
        ///             =  ( 1 + erf(z) ) / 2
        ///             =  erfc(z) / 2
        /// </pre>
        /// where <i>z = x/sqrt(2)</i>.
        /// Computation is via the functions <i>errorFunction</i> and <i>errorFunctionComplement</i>.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        static public double Normal(double a)
        {
            double x, y, z;

            x = a * SQRTH;
            z = System.Math.Abs(x);

            if (z < SQRTH) y = 0.5 + 0.5 * ErrorFunction(x);
            else
            {
                y = 0.5 * ErrorFunctionComplemented(z);
                if (x > 0) y = 1.0 - y;
            }

            return y;
        }

        /// <summary>
        /// Returns the area under the Normal (Gaussian) probability density
        /// function, integrated from minus infinity to <i>x</i>.
        /// <pre>
        ///                            x
        ///                             -
        ///                   1        | |                 2
        ///  normal(x)  = ---------    |    exp( - (t-mean) / 2v ) dt
        ///               sqrt(2pi*v)| |
        ///                           -
        ///                          -inf.
        ///
        /// </pre>
        /// where <i>v = variance</i>.
        /// Computation is via the functions <i>errorFunction</i>.
        /// </summary>
        /// <param name="mean">the mean of the normal distribution.</param>
        /// <param name="variance">the variance of the normal distribution.</param>
        /// <param name="x">the integration limit.</param>
        /// <returns></returns>
        static public double Normal(double mean, double variance, double x)
        {
            if (x > 0)
                return 0.5 + 0.5 * ErrorFunction((x - mean) / System.Math.Sqrt(2.0 * variance));
            else
                return 0.5 - 0.5 * ErrorFunction((-(x - mean)) / System.Math.Sqrt(2.0 * variance));
        }

        /// <summary>
        /// Returns the value, <i>x</i>, for which the area under the
        /// Normal (Gaussian) probability density function (integrated from
        /// minus infinity to <i>x</i>) is equal to the argument <i>y</i> (assumes mean is zero, variance is one); formerly named <i>ndtri</i>.
        /// <p>
        /// For small arguments <i>0 < y < exp(-2)</i>, the program computes
        /// <i>z = sqrt( -2.0/// log(y) )</i>;  then the approximation is
        /// <i>x = z - log(z)/z  - (1/z) P(1/z) / Q(1/z)</i>.
        /// There are two rational functions P/Q, one for <i>0 < y < exp(-32)</i>
        /// and the other for <i>y</i> up to <i>exp(-2)</i>d 
        /// For larger arguments,
        /// <i>w = y - 0.5</i>, and  <i>x/sqrt(2pi) = w + w**3 R(w**2)/S(w**2))</i>.
        /// </summary>
        /// <param name="y0"></param>
        /// <returns></returns>
        static public double NormalInverse(double y0)
        {
            double x, y, z, y2, x0, x1;
            int code;

            double s2pi = System.Math.Sqrt(2.0 * System.Math.PI);

            if (y0 <= 0.0) throw new ArgumentException();
            if (y0 >= 1.0) throw new ArgumentException();
            code = 1;
            y = y0;
            if (y > (1.0 - 0.13533528323661269189))
            { /* 0.135..d = exp(-2) */
                y = 1.0 - y;
                code = 0;
            }

            if (y > 0.13533528323661269189)
            {
                y = y - 0.5;
                y2 = y * y;
                x = y + y * (y2 * Polynomial.Polevl(y2, P0, 4) / Polynomial.P1evl(y2, Q0, 8));
                x = x * s2pi;
                return (x);
            }

            x = System.Math.Sqrt(-2.0 * System.Math.Log(y));
            x0 = x - System.Math.Log(x) / x;

            z = 1.0 / x;
            if (x < 8.0) /* y > exp(-32) = 1.2664165549e-14 */

                x1 = z * Polynomial.Polevl(z, P1, 8) / Polynomial.P1evl(z, Q1, 8);
            else
                x1 = z * Polynomial.Polevl(z, P2, 8) / Polynomial.P1evl(z, Q2, 8);
            x = x0 - x1;
            if (code != 0)
                x = -x;
            return (x);
        }

        /// <summary>
        /// Returns the sum of the first <i>k</i> terms of the Poisson distribution.
        /// <pre>
        ///   k         j
        ///   --   -m  m
        ///   >   e    --
        ///   --       j!
        ///  j=0
        /// </pre>
        /// The terms are not summed directly; instead the incomplete
        /// gamma integral is employed, according to the relation
        /// <p>
        /// <i>y = Poisson( k, m ) = Gamma.IncompleteGammaComplement( k+1, m )</i>.
        ///
        /// The arguments must both be positive.
        /// </summary>
        /// <param name="k">number of terms.</param>
        /// <param name="mean">the mean of the poisson distribution.</param>
        /// <returns></returns>
        static public double Poisson(int k, double mean)
        {
            if (mean < 0) throw new ArgumentException();
            if (k < 0) return 0.0;
            return Stat.Gamma.IncompleteGammaComplement((double)(k + 1), mean);
        }

        /// <summary>
        /// Returns the sum of the terms <i>k+1</i> to <i>Infinity</i> of the Poisson distribution.
        /// <pre>
        ///  infd       j
        ///   --   -m  m
        ///   >   e    --
        ///   --       j!
        ///  j=k+1
        /// </pre>
        /// The terms are not summed directly; instead the incomplete
        /// gamma integral is employed, according to the formula
        /// <p>
        /// <i>y = poissonComplemented( k, m ) = Gamma.IncompleteGamma( k+1, m )</i>.
        ///
        /// The arguments must both be positive.
        /// </summary>
        /// <param name="k">start term.</param>
        /// <param name="mean">the mean of the poisson distribution.</param>
        /// <returns></returns>
        static public double PoissonComplemented(int k, double mean)
        {
            if (mean < 0) throw new ArgumentException();
            if (k < -1) return 0.0;
            return Stat.Gamma.IncompleteGamma((double)(k + 1), mean);
        }

        /// <summary>
        /// Returns the integral from minus infinity to <i>t</i> of the Student-t 
        /// distribution with <i>k &gt; 0</i> degrees of freedom.
        /// <pre>
        ///                                      t
        ///                                      -
        ///                                     | |
        ///              -                      |         2   -(k+1)/2
        ///             | ( (k+1)/2 )           |  (     x   )
        ///       ----------------------        |  ( 1 + --- )        dx
        ///                     -               |  (      k  )
        ///       sqrt( k pi ) | ( k/2 )        |
        ///                                   | |
        ///                                    -
        ///                                   -inf.
        /// </pre>
        /// Relation to incomplete beta integral:
        /// <p>
        /// <i>1 - StudentT(k,t) = 0.5/// Gamma.IncompleteBeta( k/2, 1/2, z )</i>
        /// where <i>z = k/(k + t**2)</i>.
        /// <p>
        /// Since the function is symmetric about <i>t=0</i>, the area under the
        /// right tail of the density is found by calling the function
        /// with <i>-t</i> instead of <i>t</i>.
        /// </summary>
        /// <param name="k">degrees of freedom.</param>
        /// <param name="t">integration end point.</param>
        /// <returns></returns>
        static public double StudentT(double k, double t)
        {
            if (k <= 0) throw new ArgumentException();
            if (t == 0) return (0.5);

            double cdf = 0.5 * Stat.Gamma.IncompleteBeta(0.5 * k, 0.5, k / (k + t * t));

            if (t >= 0) cdf = 1.0 - cdf; // fixes bug reported by stefan.bentink@molgen.mpg.de

            return cdf;
        }

        /// <summary>
        /// Returns the value, <i>t</i>, for which the area under the
        /// Student-t probability density function (integrated from
        /// minus infinity to <i>t</i>) is equal to <i>1-alpha/2</i>.
        /// The value returned corresponds to usual Student t-distribution lookup
        /// table for <i>t<sub>alpha[size]</sub></i>.
        /// <p>
        /// The function uses the studentT function to determine the return
        /// value iteratively.
        /// </summary>
        /// <param name="alpha">probability</param>
        /// <param name="size">size of data HashSet</param>
        /// <returns></returns>
        public static double StudentTInverse(double alpha, int size)
        {
            double cumProb = 1 - alpha / 2; // Cumulative probability
            double f1, f2, f3;
            double x1, x2, x3;
            double g, s12;

            cumProb = 1 - alpha / 2; // Cumulative probability
            x1 = NormalInverse(cumProb);

            // Return inverse of normal for large size
            if (size > 200)
            {
                return x1;
            }

            // Find a pair of x1,x2 that braket zero
            f1 = StudentT(size, x1) - cumProb;
            x2 = x1; f2 = f1;
            do
            {
                if (f1 > 0)
                {
                    x2 = x2 / 2;
                }
                else
                {
                    x2 = x2 + x1;
                }
                f2 = StudentT(size, x2) - cumProb;
            } while (f1 * f2 > 0);

            // Find better approximation
            // Pegasus-method
            do
            {
                // Calculate slope of secant and t value for which it is 0.
                s12 = (f2 - f1) / (x2 - x1);
                x3 = x2 - f2 / s12;

                // Calculate function value at x3
                f3 = StudentT(size, x3) - cumProb;
                if (System.Math.Abs(f3) < 1e-8)
                { // This criteria needs to be very tight!
                  // We found a perfect value -> return
                    return x3;
                }

                if (f3 * f2 < 0)
                {
                    x1 = x2; f1 = f2;
                    x2 = x3; f2 = f3;
                }
                else
                {
                    g = f2 / (f2 + f3);
                    f1 = g * f1;
                    x2 = x3; f2 = f3;
                }
            } while (System.Math.Abs(x2 - x1) > 0.001);

            if (System.Math.Abs(f2) <= System.Math.Abs(f1))
            {
                return x2;
            }
            else
            {
                return x1;
            }
        }

        #endregion

        #region Local Protected Methods

        #endregion

        #region Local Private Methods

        #endregion

    }
}
