using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Jet.Math;

namespace Cern.Jet.Stat
{
    public class Gamma : Cern.Jet.Math.Constants
    {

        #region Local Variables

        #endregion

        #region Property

        #endregion

        #region Constructor

        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected Gamma() { }

        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static public double Beta(double a, double b)
        {

            double y;

            y = a + b;
            y = GetGamma(y);
            if (y == 0.0) return 1.0;

            if (a > b)
            {
                y = GetGamma(a) / y;
                y *= GetGamma(b);
            }
            else
            {
                y = GetGamma(b) / y;
                y *= GetGamma(a);
            }

            return (y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        static public double GetGamma(double x)
        {

            double[] P = {
               1.60119522476751861407E-4,
               1.19135147006586384913E-3,
               1.04213797561761569935E-2,
               4.76367800457137231464E-2,
               2.07448227648435975150E-1,
               4.94214826801497100753E-1,
               9.99999999999999996796E-1
              };
            double[] Q = {
               -2.31581873324120129819E-5,
                5.39605580493303397842E-4,
               -4.45641913851797240494E-3,
                1.18139785222060435552E-2,
                3.58236398605498653373E-2,
               -2.34591795718243348568E-1,
                7.14304917030273074085E-2,
                1.00000000000000000320E0
               };
            //double MAXGAM = 171.624376956302725;
            //double LOGPI  = 1.14472988584940017414;

            double p, z;
            int i;

            double q = System.Math.Abs(x);

            if (q > 33.0)
            {
                if (x < 0.0)
                {
                    p = System.Math.Floor(q);
                    if (p == q) throw new ArithmeticException(Cern.LocalizedResources.Instance().Exception_GammaOverflow);
                    i = (int)p;
                    z = q - p;
                    if (z > 0.5)
                    {
                        p += 1.0;
                        z = q - p;
                    }
                    z = q * System.Math.Sin(System.Math.PI * z);
                    if (z == 0.0) throw new ArithmeticException(Cern.LocalizedResources.Instance().Exception_GammaOverflow);
                    z = System.Math.Abs(z);
                    z = System.Math.PI / (z * StirlingFormula(q));

                    return -z;
                }
                else
                {
                    return StirlingFormula(x);
                }
            }

            z = 1.0;
            while (x >= 3.0)
            {
                x -= 1.0;
                z *= x;
            }

            while (x < 0.0)
            {
                if (x == 0.0)
                {
                    throw new ArithmeticException(Cern.LocalizedResources.Instance().Exception_GammaSingular);
                }
                else
                if (x > -1E-9)
                {
                    return (z / ((1.0 + 0.5772156649015329 * x) * x));
                }
                z /= x;
                x += 1.0;
            }

            while (x < 2.0)
            {
                if (x == 0.0)
                {
                    throw new ArithmeticException(Cern.LocalizedResources.Instance().Exception_GammaSingular);
                }
                else
                if (x < 1E-9)
                {
                    return (z / ((1.0 + 0.5772156649015329 * x) * x));
                }
                z /= x;
                x += 1.0;
            }

            if ((x == 2.0) || (x == 3.0)) return z;

            x -= 2.0;
            p = Polynomial.Polevl(x, P, 6);
            q = Polynomial.Polevl(x, Q, 7);
            return z * p / q;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aa"></param>
        /// <param name="bb"></param>
        /// <param name="xx"></param>
        /// <returns></returns>
        public static double IncompleteBeta(double aa, double bb, double xx)
        {

            double a, b, t, x, xc, w, y;
            Boolean flag;

            if (aa <= 0.0 || bb <= 0.0) throw new

                             ArithmeticException(Cern.LocalizedResources.Instance().Exception_IBetaDomainError);

            if ((xx <= 0.0) || (xx >= 1.0))
            {
                if (xx == 0.0) return 0.0;
                if (xx == 1.0) return 1.0;
                throw new ArithmeticException(Cern.LocalizedResources.Instance().Exception_IBetaDomainError);
            }

            flag = false;
            if ((bb * xx) <= 1.0 && xx <= 0.95)
            {
                t = PowerSeries(aa, bb, xx);
                return t;
            }

            w = 1.0 - xx;

            /* Reverse a and b if x is greater than the meand */
            if (xx > (aa / (aa + bb)))
            {
                flag = true;
                a = bb;
                b = aa;
                xc = xx;
                x = w;
            }
            else
            {
                a = aa;
                b = bb;
                xc = w;
                x = xx;
            }

            if (flag && (b * x) <= 1.0 && x <= 0.95)
            {
                t = PowerSeries(a, b, x);
                if (t <= MACHEP) t = 1.0 - MACHEP;
                else t = 1.0 - t;
                return t;
            }

            /* Choose expansion for better convergenced */
            y = x * (a + b - 2.0) - (a - 1.0);
            if (y < 0.0)

                w = IncompleteBetaFraction1(a, b, x);
            else
                w = IncompleteBetaFraction2(a, b, x) / xc;

            /* Multiply w by the factor
               a      b   _             _     _
              x  (1-x)   | (a+b) / ( a | (a) | (b) ) d   */

            y = a * System.Math.Log(x);
            t = b * System.Math.Log(xc);
            if ((a + b) < MAXGAM && System.Math.Abs(y) < MAXLOG && System.Math.Abs(t) < MAXLOG)
            {
                t = System.Math.Pow(xc, b);
                t *= System.Math.Pow(x, a);
                t /= a;
                t *= w;
                t *= GetGamma(a + b) / (GetGamma(a) * GetGamma(b));
                if (flag)
                {
                    if (t <= MACHEP) t = 1.0 - MACHEP;
                    else t = 1.0 - t;
                }
                return t;
            }
            /* Resort to logarithmsd  */
            y += t + LogGamma(a + b) - LogGamma(a) - LogGamma(b);
            y += System.Math.Log(w / a);
            if (y < MINLOG)

                t = 0.0;
            else
                t = System.Math.Exp(y);

            if (flag)
            {
                if (t <= MACHEP) t = 1.0 - MACHEP;
                else t = 1.0 - t;
            }
            return t;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        static public double IncompleteGamma(double a, double x)

        {



            double ans, ax, c, r;

            if (x <= 0 || a <= 0) return 0.0;

            if (x > 1.0 && x > a) return 1.0 - IncompleteGammaComplement(a, x);

            /* Compute  x**a * exp(-x) / gamma(a)  */
            ax = a * System.Math.Log(x) - x - LogGamma(a);
            if (ax < -MAXLOG) return (0.0);

            ax = System.Math.Exp(ax);

            /* power series */
            r = a;
            c = 1.0;
            ans = 1.0;

            do
            {
                r += 1.0;
                c *= x / r;
                ans += c;
            }
            while (c / ans > MACHEP);

            return (ans * ax / a);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        static public double IncompleteGammaComplement(double a, double x)
        {

            double ans, ax, c, yc, r, t, y, z;
            double pk, pkm1, pkm2, qk, qkm1, qkm2;

            if (x <= 0 || a <= 0) return 1.0;

            if (x < 1.0 || x < a) return 1.0 - IncompleteGamma(a, x);

            ax = a * System.Math.Log(x) - x - LogGamma(a);
            if (ax < -MAXLOG) return 0.0;

            ax = System.Math.Exp(ax);

            /* continued fraction */
            y = 1.0 - a;
            z = x + y + 1.0;
            c = 0.0;
            pkm2 = 1.0;
            qkm2 = x;
            pkm1 = x + 1.0;
            qkm1 = z * x;
            ans = pkm1 / qkm1;

            do
            {
                c += 1.0;
                y += 1.0;
                z += 2.0;
                yc = y * c;
                pk = pkm1 * z - pkm2 * yc;
                qk = qkm1 * z - qkm2 * yc;
                if (qk != 0)
                {
                    r = pk / qk;
                    t = System.Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                    t = 1.0;

                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                if (System.Math.Abs(pk) > big)
                {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }
            } while (t > MACHEP);

            return ans * ax;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double LogGamma(double x)
        {

            double p, q, w, z;

            double[] A = {
                       8.11614167470508450300E-4,
                       -5.95061904284301438324E-4,
                        7.93650340457716943945E-4,
                       -2.77777777730099687205E-3,
                        8.33333333333331927722E-2
                       };
            double[] B = {
                       -1.37825152569120859100E3,
                       -3.88016315134637840924E4,
                       -3.31612992738871184744E5,
                       -1.16237097492762307383E6,
                       -1.72173700820839662146E6,
                       -8.53555664245765465627E5
                       };
            double[] C = {
					   /* 1.00000000000000000000E0, */
					   -3.51815701436523470549E2,
                       -1.70642106651881159223E4,
                       -2.20528590553854454839E5,
                       -1.13933444367982507207E6,
                       -2.53252307177582951285E6,
                       -2.01889141433532773231E6
                      };

            if (x < -34.0)
            {
                q = -x;
                w = LogGamma(q);
                p = System.Math.Floor(q);
                if (p == q) throw new ArithmeticException(Cern.LocalizedResources.Instance().Exception_LogGammaOverflow);
                z = q - p;
                if (z > 0.5)
                {
                    p += 1.0;
                    z = p - q;
                }
                z = q * System.Math.Sin(System.Math.PI * z);
                if (z == 0.0) throw new

                                       ArithmeticException(Cern.LocalizedResources.Instance().Exception_LogGammaOverflow);
                z = LOGPI - System.Math.Log(z) - w;
                return z;
            }

            if (x < 13.0)
            {
                z = 1.0;
                while (x >= 3.0)
                {
                    x -= 1.0;
                    z *= x;
                }
                while (x < 2.0)
                {
                    if (x == 0.0) throw new

                                           ArithmeticException(Cern.LocalizedResources.Instance().Exception_LogGammaOverflow);
                    z /= x;
                    x += 1.0;
                }
                if (z < 0.0) z = -z;
                if (x == 2.0) return System.Math.Log(z);
                x -= 2.0;
                p = x * Polynomial.Polevl(x, B, 5) / Polynomial.P1evl(x, C, 6);
                return (System.Math.Log(z) + p);
            }

            if (x > 2.556348e305) throw new

                            ArithmeticException(Cern.LocalizedResources.Instance().Exception_LogGammaOverflow);

            q = (x - 0.5) * System.Math.Log(x) - x + 0.91893853320467274178;
            //if( x > 1.0e8 ) return( q );
            if (x > 1.0e8) return (q);

            p = 1.0 / (x * x);
            if (x >= 1000.0)
                q += ((7.9365079365079365079365e-4 * p
                     - 2.7777777777777777777778e-3) * p
                    + 0.0833333333333333333333) / x;
            else
                q += Polynomial.Polevl(p, A, 4) / x;
            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        static double PowerSeries(double a, double b, double x)
        {

            double s, t, u, v, n, t1, z, ai;

            ai = 1.0 / a;
            u = (1.0 - b) * x;
            v = u / (a + 1.0);
            t1 = v;
            t = u;
            n = 2.0;
            s = 0.0;
            z = MACHEP * ai;
            while (System.Math.Abs(v) > z)
            {
                u = (n - b) * x / n;
                t *= u;
                v = t / (a + n);
                s += v;
                n += 1.0;
            }
            s += t1;
            s += ai;

            u = a * System.Math.Log(x);
            if ((a + b) < MAXGAM && System.Math.Abs(u) < MAXLOG)
            {
                t = Stat.Gamma.GetGamma(a + b) / (Stat.Gamma.GetGamma(a) * Stat.Gamma.GetGamma(b));
                s = s * t * System.Math.Pow(x, a);
            }
            else
            {
                t = Stat.Gamma.LogGamma(a + b) - Stat.Gamma.LogGamma(a) - Stat.Gamma.LogGamma(b) + u + System.Math.Log(s);
                if (t < MINLOG) s = 0.0;
                else s = System.Math.Exp(t);
            }
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        static double StirlingFormula(double x)
        {

            double[] STIR = {
                     7.87311395793093628397E-4,
                    -2.29549961613378126380E-4,
                    -2.68132617805781232825E-3,
                     3.47222221605458667310E-3,
                     8.33333333333482257126E-2,
                    };
            double MAXSTIR = 143.01608;

            double w = 1.0 / x;
            double y = System.Math.Exp(x);

            w = 1.0 + w * Polynomial.Polevl(w, STIR, 4);

            if (x > MAXSTIR)
            {
                /* Avoid overflow in System.Math.Pow() */
                double v = System.Math.Pow(x, 0.5 * x - 0.25);
                y = v * (v / y);
            }
            else
            {
                y = System.Math.Pow(x, x - 0.5) / y;
            }
            y = SQTPI * y * w;
            return y;
        }

        #endregion

        #region Local Protected Methods

        #endregion

        #region Local Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        static double IncompleteBetaFraction1(double a, double b, double x)
        {

            double xk, pk, pkm1, pkm2, qk, qkm1, qkm2;
            double k1, k2, k3, k4, k5, k6, k7, k8;
            double r, t, ans, thresh;
            int n;

            k1 = a;
            k2 = a + b;
            k3 = a;
            k4 = a + 1.0;
            k5 = 1.0;
            k6 = b - 1.0;
            k7 = k4;
            k8 = a + 2.0;

            pkm2 = 0.0;
            qkm2 = 1.0;
            pkm1 = 1.0;
            qkm1 = 1.0;
            ans = 1.0;
            r = 1.0;
            n = 0;
            thresh = 3.0 * MACHEP;
            do
            {
                xk = -(x * k1 * k2) / (k3 * k4);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                xk = (x * k5 * k6) / (k7 * k8);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                if (qk != 0) r = pk / qk;
                if (r != 0)
                {
                    t = System.Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                    t = 1.0;

                if (t < thresh) return ans;

                k1 += 1.0;
                k2 += 1.0;
                k3 += 2.0;
                k4 += 2.0;
                k5 += 1.0;
                k6 -= 1.0;
                k7 += 2.0;
                k8 += 2.0;

                if ((System.Math.Abs(qk) + System.Math.Abs(pk)) > big)
                {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }
                if ((System.Math.Abs(qk) < biginv) || (System.Math.Abs(pk) < biginv))
                {
                    pkm2 *= big;
                    pkm1 *= big;
                    qkm2 *= big;
                    qkm1 *= big;
                }
            } while (++n < 300);

            return ans;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        static double IncompleteBetaFraction2(double a, double b, double x)
        {

            double xk, pk, pkm1, pkm2, qk, qkm1, qkm2;
            double k1, k2, k3, k4, k5, k6, k7, k8;
            double r, t, ans, z, thresh;
            int n;

            k1 = a;
            k2 = b - 1.0;
            k3 = a;
            k4 = a + 1.0;
            k5 = 1.0;
            k6 = a + b;
            k7 = a + 1.0; ;
            k8 = a + 2.0;

            pkm2 = 0.0;
            qkm2 = 1.0;
            pkm1 = 1.0;
            qkm1 = 1.0;
            z = x / (1.0 - x);
            ans = 1.0;
            r = 1.0;
            n = 0;
            thresh = 3.0 * MACHEP;
            do
            {
                xk = -(z * k1 * k2) / (k3 * k4);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                xk = (z * k5 * k6) / (k7 * k8);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                if (qk != 0) r = pk / qk;
                if (r != 0)
                {
                    t = System.Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                    t = 1.0;

                if (t < thresh) return ans;

                k1 += 1.0;
                k2 -= 1.0;
                k3 += 2.0;
                k4 += 2.0;
                k5 += 1.0;
                k6 += 1.0;
                k7 += 2.0;
                k8 += 2.0;

                if ((System.Math.Abs(qk) + System.Math.Abs(pk)) > big)
                {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }
                if ((System.Math.Abs(qk) < biginv) || (System.Math.Abs(pk) < biginv))
                {
                    pkm2 *= big;
                    pkm1 *= big;
                    qkm2 *= big;
                    qkm1 *= big;
                }
            } while (++n < 300);

            return ans;
        }


        #endregion

    }
}
