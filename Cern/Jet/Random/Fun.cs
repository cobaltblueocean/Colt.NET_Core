﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Jet.Random
{
    /// <summary>
    /// Contains various mathematical helper methods.
    /// </summary>
    public class Fun
    {
        /// <summary>
        /// Makes this class non instantiable, but still let's others inherit from it.
        /// </summary>
        protected Fun()
        {
            throw new NotSupportedException(Cern.LocalizedResources.Instance().Exception_NonInstantiable);
        }

        private static double _fkt_value(double lambda, double z1, double z2, double x_value)
        {
            double y_value;

            y_value = System.Math.Cos(z1 * x_value) / (System.Math.Pow((x_value * x_value + z2 * z2), (lambda + 0.5)));
            return y_value;
        }

        public static double Bessel2_fkt(double lambda, double beta)
        {
            double pi = System.Math.PI;
            double sum, x, step, x1, first_value, new_value;
            double epsilon = 0.01;
            double y, fx, z1, z2, erg;
            double period, border, first_sum, second_sum;
            double my, c, prod = 0.0, diff, value;
            int i, j, nr_per;

            double[] b0 =  { -1.5787132, -0.6130827,  0.1735823,  1.4793411,
            2.6667307,  4.9086836,  8.1355339,
           };
            double[] b05 = { -1.9694802, -0.7642538,  0.0826017,  1.4276355,
            2.6303682,  4.8857787,  8.1207968,
           };
            double[] b1 =  { -2.9807345, -1.1969943, -0.1843161,  1.2739241,
            2.5218256,  4.8172216,  8.0765633,
           };
            double[] b2 =  { -5.9889676, -2.7145389, -1.1781269,  0.6782201,
            2.0954009,  4.5452152,  7.9003173,
           };
            double[] b3 =  { -9.6803440, -4.8211925, -2.6533185, -0.2583337,
            1.4091915,  4.0993448,  7.6088310,
           };
            double[] b5 =  {-18.1567152,-10.0939408, -6.5819139, -2.9371545,
           -0.6289005,  2.7270412,  6.6936799,
           };
            double[] b8 =  {-32.4910195,-19.6065943,-14.0347298, -8.3839439,
           -4.9679730, -0.3567823,  4.5589697,
           };

            if (lambda == 0.0)
            {
                if (beta == 0.1) return (b0[0]);
                if (beta == 0.5) return (b0[1]);
                if (beta == 1.0) return (b0[2]);
                if (beta == 2.0) return (b0[3]);
                if (beta == 3.0) return (b0[4]);
                if (beta == 5.0) return (b0[5]);
                if (beta == 8.0) return (b0[6]);
            }

            if (lambda == 0.5)
            {
                if (beta == 0.1) return (b05[0]);
                if (beta == 0.5) return (b05[1]);
                if (beta == 1.0) return (b05[2]);
                if (beta == 2.0) return (b05[3]);
                if (beta == 3.0) return (b05[4]);
                if (beta == 5.0) return (b05[5]);
                if (beta == 8.0) return (b05[6]);
            }

            if (lambda == 1.0)
            {
                if (beta == 0.1) return (b1[0]);
                if (beta == 0.5) return (b1[1]);
                if (beta == 1.0) return (b1[2]);
                if (beta == 2.0) return (b1[3]);
                if (beta == 3.0) return (b1[4]);
                if (beta == 5.0) return (b1[5]);
                if (beta == 8.0) return (b1[6]);
            }

            if (lambda == 2.0)
            {
                if (beta == 0.1) return (b2[0]);
                if (beta == 0.5) return (b2[1]);
                if (beta == 1.0) return (b2[2]);
                if (beta == 2.0) return (b2[3]);
                if (beta == 3.0) return (b2[4]);
                if (beta == 5.0) return (b2[5]);
                if (beta == 8.0) return (b2[6]);
            }

            if (lambda == 3.0)
            {
                if (beta == 0.1) return (b3[0]);
                if (beta == 0.5) return (b3[1]);
                if (beta == 1.0) return (b3[2]);
                if (beta == 2.0) return (b3[3]);
                if (beta == 3.0) return (b3[4]);
                if (beta == 5.0) return (b3[5]);
                if (beta == 8.0) return (b3[6]);
            }

            if (lambda == 5.0)
            {
                if (beta == 0.1) return (b5[0]);
                if (beta == 0.5) return (b5[1]);
                if (beta == 1.0) return (b5[2]);
                if (beta == 2.0) return (b5[3]);
                if (beta == 3.0) return (b5[4]);
                if (beta == 5.0) return (b5[5]);
                if (beta == 8.0) return (b5[6]);
            }

            if (lambda == 8.0)
            {
                if (beta == 0.1) return (b8[0]);
                if (beta == 0.5) return (b8[1]);
                if (beta == 1.0) return (b8[2]);
                if (beta == 2.0) return (b8[3]);
                if (beta == 3.0) return (b8[4]);
                if (beta == 5.0) return (b8[5]);
                if (beta == 8.0) return (b8[6]);
            }


            if ((beta - 5.0 * lambda - 8.0) >= 0.0)
            {
                my = 4.0 * lambda * lambda;
                c = -0.9189385 + 0.5 * System.Math.Log(beta) + beta;
                sum = 1.0;
                value = 1.0;
                diff = 8.0;
                i = 1;
                for (; ; )
                { //while (!NULL) {
                    if ((Factorial(i) * System.Math.Pow((8.0 * beta), i)) > 1.0e250) break;
                    if (i > 10) break;
                    if (i == 1) prod = my - 1.0;
                    else
                    {
                        value += diff;
                        prod = prod * (my - value);
                        diff *= 2.0;
                    }
                    sum = sum + prod / (Factorial(i) * System.Math.Pow((8.0 * beta), i));
                    i++;
                }
                erg = c - System.Math.Log(sum);
                return (erg);
            }

            if ((lambda > 0.0) && ((beta - 0.04 * lambda) <= 0.0))
            {
                if (lambda < 11.5)
                {
                    erg = -System.Math.Log(Gamma(lambda)) - lambda * System.Math.Log(2.0) + lambda * System.Math.Log(beta);
                    return (erg);
                }
                else
                {
                    erg = -(lambda + 1.0) * System.Math.Log(2.0) - (lambda - 0.5) * System.Math.Log(lambda) + lambda + lambda * System.Math.Log(beta) - 0.5 * System.Math.Log(0.5 * pi);
                    return (erg);
                }
            }


            // otherwise numerical integration of the function defined above 

            x = 0.0;

            if (beta < 1.57)
            {
                fx = (Fkt2_value(lambda, beta, x)) * 0.01;
                y = 0.0;
                for (; ; )
                { //while (!NULL) {
                    y += 0.1;
                    if ((Fkt2_value(lambda, beta, y)) < fx) break;
                }
                step = y * 0.001;
                x1 = step;
                sum = (0.5 * (10.0 * step + Fkt2_value(lambda, beta, x1))) * step;
                first_value = sum;
                for (; ; )
                { //while (!NULL) {
                    x = x1;
                    x1 += step;
                    new_value = (0.5 * (Fkt2_value(lambda, beta, x) + Fkt2_value(lambda, beta, x1))) * step;
                    sum += new_value;
                    if ((new_value / first_value) < epsilon) break;
                }
                erg = -System.Math.Log(2.0 * sum);
                return (erg);
            }
            else
            {
                z2 = 1.57;
                z1 = beta / 1.57;
                sum = 0.0;
                period = pi / z1;
                step = 0.1 * period;
                border = 100.0 / ((lambda + 0.1) * (lambda + 0.1));
                nr_per = (int)System.Math.Ceiling((border / period)) + 20;
                x1 = step;
                for (i = 1; i <= nr_per; i++)
                {
                    for (j = 1; j <= 10; j++)
                    {
                        new_value = (0.5 * (_fkt_value(lambda, z1, z2, x) + _fkt_value(lambda, z1, z2, x1))) * step;
                        sum += new_value;
                        x = x1;
                        x1 += step;
                    }
                }
                for (j = 1; j <= 5; j++)
                {
                    new_value = (0.5 * (_fkt_value(lambda, z1, z2, x) + _fkt_value(lambda, z1, z2, x1))) * step;
                    sum += new_value;
                    x = x1;
                    x1 += step;
                }
                first_sum = sum;
                for (j = 1; j <= 10; j++)
                {
                    new_value = (0.5 * (_fkt_value(lambda, z1, z2, x) + _fkt_value(lambda, z1, z2, x1))) * step;
                    sum += new_value;
                    x = x1;
                    x1 += step;
                }
                second_sum = sum;
                sum = 0.5 * (first_sum + second_sum);
                erg = Gamma(lambda + 0.5) * System.Math.Pow((2.0 * z2), lambda) / (System.Math.Sqrt(pi) * System.Math.Pow(z1, lambda)) * sum;
                erg = -System.Math.Log(2.0 * erg);
                return (erg);
            }
        }

        /// <summary>
        /// Modified Bessel Functions of First Kind - Order 0.
        /// </summary>
        public static double Bessi0(double x)
        {
            double ax, ans;
            double y;

            if ((ax = System.Math.Abs(x)) < 3.75)
            {
                y = x / 3.75;
                y *= y;
                ans = 1.0 + y * (3.5156229 + y * (3.0899424 + y * (1.2067492
                    + y * (0.2659732 + y * (0.360768e-1 + y * 0.45813e-2)))));
            }
            else
            {
                y = 3.75 / ax;
                ans = (System.Math.Exp(ax) / System.Math.Sqrt(ax)) * (0.39894228 + y * (0.1328592e-1
                    + y * (0.225319e-2 + y * (-0.157565e-2 + y * (0.916281e-2
                    + y * (-0.2057706e-1 + y * (0.2635537e-1 + y * (-0.1647633e-1
                    + y * 0.392377e-2))))))));
            }
            return ans;
        }

        /// <summary>
        /// Modified Bessel Functions of First Kind - Order 1.
        /// </summary>
        public static double Bessi1(double x)
        {
            double ax, ans;
            double y;

            if ((ax = System.Math.Abs(x)) < 3.75)
            {
                y = x / 3.75;
                y *= y;
                ans = ax * (0.5 + y * (0.87890594 + y * (0.51498869 + y * (0.15084934
                    + y * (0.2658733e-1 + y * (0.301532e-2 + y * 0.32411e-3))))));
            }
            else
            {
                y = 3.75 / ax;
                ans = 0.2282967e-1 + y * (-0.2895312e-1 + y * (0.1787654e-1
                    - y * 0.420059e-2));
                ans = 0.39894228 + y * (-0.3988024e-1 + y * (-0.362018e-2
                    + y * (0.163801e-2 + y * (-0.1031555e-1 + y * ans))));
                ans *= (System.Math.Exp(ax) / System.Math.Sqrt(ax));
            }
            return x < 0.0 ? -ans : ans;
        }

        /// <summary>
        /// Returns <i>n!</i>.
        /// </summary>
        public static long Factorial(int n)
        {
            return Cern.Jet.Math.Arithmetic.LongFactorial(n);
            /*
            long i,prod;

            prod = 1;
            if (n != 0) {
                for (i = 2; i <= n; i++) prod///= i;
            }
            return prod;
            **/
        }

        private static double Fkt2_value(double lambda, double beta, double x_value)
        {
            double y_value;

            y_value = Cosh(lambda * x_value) * System.Math.Exp(-beta * Cosh(x_value));
            return y_value;
        }

        private static double Cosh(double x)
        {
            return (System.Math.Exp(x) + System.Math.Exp(-x)) / 2.0;
        }

        /// <summary>
        /// Returns the gamma function <i>gamma(x)</i>.
        /// </summary>
        public static double Gamma(double x)
        {
            x = LogGamma(x);
            //if (x > System.Math.Log(Double.MaxValue)) return Double.MaxValue;
            return System.Math.Exp(x);
        }

        /// <summary>
        /// Returns a quick approximation of <i>log(gamma(x))</i>.
        /// </summary>
        public static double LogGamma(double x)
        {
            double c0 = 9.1893853320467274e-01, c1 = 8.3333333333333333e-02,
                c2 = -2.7777777777777777e-03, c3 = 7.9365079365079365e-04,
                c4 = -5.9523809523809524e-04, c5 = 8.4175084175084175e-04,
                c6 = -1.9175269175269175e-03;
            double g, r, z;

            if (x <= 0.0 /* || x > 1.3e19 */ ) return -999;

            for (z = 1.0; x < 11.0; x++) z *= x;

            r = 1.0 / (x * x);
            g = c1 + r * (c2 + r * (c3 + r * (c4 + r * (c5 + r + c6))));
            g = (x - 0.5) * System.Math.Log(x) - x + c0 + g / x;
            if (z == 1.0) return g;
            return g - System.Math.Log(z);
        }
    }
}
