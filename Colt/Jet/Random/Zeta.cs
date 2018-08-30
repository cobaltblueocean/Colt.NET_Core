﻿// <copyright file="Zeta.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentationd 
//   CERN makes no representations about the suitability of this software for any purposed 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Cern.Jet.Math;
using Cern.Jet.Stat;
using Cern.Jet.Random.Engine;

namespace Cern.Jet.Random
{
    /// <summary>
    /// Zeta distribution.
    /// <p>
    /// Valid parameter ranges: <i>ro &gt; 0</i> and <i>pk &gt;= 0</i>.
    /// <dt>
    /// If either <i>ro &gt; 100</i>  or  <i>k &gt; 10000</i> numerical problems in
    /// computing the theoretical moments arise, therefore <i>ro &lt;= 100</i> and 
    /// <i>k &lt;= 10000</i> are recommendedd                                      
    /// <p>
    /// Instance methods operate on a user supplied uniform random number generator; they are unsynchronized.
    /// <dt>
    /// Static methods operate on a default uniform random number generator; they are synchronized.
    /// <p>
    /// <b>Implementation:</b> 
    /// <dt>Method: Acceptance/Rejection.
    /// High performance implementation.
    /// <dt>This is a port and adaption of <i>Zeta.c</i> from the <A HREF="http://www.cis.tu-graz.ac.at/stat/stadl/random.html">C-RAND / WIN-RAND</A> library.
    /// C-RAND's implementation, in turn, is based upon
    /// <p>
    /// Jd Dagpunar (1988): Principles of Random Variate  Generation, Clarendon Press, Oxfordd   
    ///
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class Zeta : AbstractDiscreteDistribution
    {
        protected double ro;
        protected double pk;

        // cached values (for performance)
        protected double c, d, ro_prev = -1.0, pk_prev = -1.0;
        protected double maxlongint = long.MaxValue - 1.5;

        // The uniform random number generated shared by all <b>static</b> methodsd 
        protected static Zeta shared = new Zeta(1.0, 1.0, MakeDefaultGenerator());

        /// <summary>
        /// Constructs a Zeta distribution.
        /// </summary>
        /// <param name="ro"></param>
        /// <param name="pk"></param>
        /// <param name="randomGenerator"></param>
        public Zeta(double ro, double pk, RandomEngine randomGenerator)
        {
            RandomGenerator = randomGenerator;
            SetState(ro, pk);
        }

        /// <summary>
        /// Returns a zeta distributed random number.
        /// </summary>
        /// <param name="ro"></param>
        /// <param name="pk"></param>
        /// <param name="randomGenerator"></param>
        /// <returns></returns>
        protected long GenerateZeta(double ro, double pk, RandomEngine randomGenerator)
        {
            /******************************************************************
             *                                                                *
             *            Zeta Distribution - Acceptance Rejection            *
             *                                                                *
             ******************************************************************
             *                                                                *
             * To sample from the Zeta distribution with parameters ro and pk *
             * it suffices to sample variates x from the distribution with    *
             * density function  f(x)=B*{[x+0.5]+pk}^(-(1+ro))( x > .5 )     *
             * and then deliver k=[x+0.5]d                                    *
             * 1/B=Sum[(j+pk)^-(ro+1)]  (j=1,2,..d) converges for ro >= .5 d  *
             * It is not necessary to compute B, because variates x are       *
             * generated by acceptance rejection using density function       *
             * g(x)=ro*(c+0.5)^ro*(c+x)^-(ro+1)d                              *
             *                                                                *                                                                *
             * int overflow is possible, when ro is small (ro <= .5) and  *
             * pk larged In this case a new sample is generatedd If ro and pk *
             * satisfy the inequality   ro > .14 + pk*1.85e-8 + .02*ln(pk)    *
             * the percentage of overflow is less than 1%, so that the        *
             * result is reliabled                                            *
             * NOTE: The comment above is likely to be nomore valid since     *
             * the C-version operated on 32-bit ints, while this Java     *
             * version operates on 64-bit intsd However, the following is *
             * still valid:                                                   *                                                                *
             *                                                                *                                                                *
             * If either ro > 100  or  k > 10000 numerical problems in        *
             * computing the theoretical moments arise, therefore ro<=100 and *
             * k<=10000 are recommendedd                                      *
             *                                                                *
             ******************************************************************
             *                                                                *
             * FUNCTION:    - zeta  samples a random number from the          *
             *                Zeta distribution with parameters  ro > 0  and  *
             *                pk >= 0d                                        *
             * REFERENCE:   - Jd Dagpunar (1988): Principles of Random        *
             *                Variate  Generation, Clarendon Press, Oxfordd   *
             *                                                                *
             ******************************************************************/
            double u, v, e, x;
            long k;

            if (ro != ro_prev || pk != pk_prev)
            {                   // Set-up 
                ro_prev = ro;
                pk_prev = pk;
                if (ro < pk)
                {
                    c = pk - 0.5;
                    d = 0;
                }
                else
                {
                    c = ro - 0.5;
                    d = (1.0 + ro) * System.Math.Log((1.0 + pk) / (1.0 + ro));
                }
            }
            do
            {
                do
                {
                    u = randomGenerator.Raw();
                    v = randomGenerator.Raw();
                    x = (c + 0.5) * System.Math.Exp(-System.Math.Log(u) / ro) - c;
                } while (x <= 0.5 || x >= maxlongint);

                k = (int)(x + 0.5);
                e = -System.Math.Log(v);
            } while (e < (1.0 + ro) * System.Math.Log((k + pk) / (x + c)) - d);

            return k;
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <returns></returns>
        public override int NextInt()
        {
            return (int)GenerateZeta(ro, pk, randomGenerator);
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="ro"></param>
        /// <param name="pk"></param>
        public void SetState(double ro, double pk)
        {
            this.ro = ro;
            this.pk = pk;
        }

        /// <summary>
        /// Returns a random number from the distribution.
        /// </summary>
        /// <param name="ro"></param>
        /// <param name="pk"></param>
        /// <returns></returns>
        public static int StaticNextInt(double ro, double pk)
        {
                shared.SetState(ro, pk);
                return shared.NextInt();
        }

        /// <summary>
        /// Returns a String representation of the receiver.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return this.GetType().Name + "(" + ro + "," + pk + ")";
        }

        /// <summary>
        /// Sets the uniform random number generated shared by all <b>static</b> methods.
        /// </summary>
        /// <param name="randomGenerator">the new uniform random number generator to be shared.</param>
        private static void xStaticSetRandomGenerator(RandomEngine randomGenerator)
        {
                shared.RandomGenerator = randomGenerator;
        }
    }
}
