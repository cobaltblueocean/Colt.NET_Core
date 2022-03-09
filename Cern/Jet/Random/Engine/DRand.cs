using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Jet.Random.Engine
{
    /// <summary>
    /// Quick medium quality uniform pseudo-random number generator.
    ///
    /// Produces uniformly distributed <i>int</i>'s and <i>long</i>'s in the closed intervals <i>[int.MinValue,int.MaxValue]</i> and <i>[Long.MinValue,Long.MaxValue]</i>, respectively, 
    /// as well as <i>float</i>'s and <i>double</i>'s in the open unit intervals <i>(0.0f,1.0f)</i> and <i>(0.0,1.0)</i>, respectively.
    /// <p>
    /// The seed can be any int satisfying <i>0 &lt; 4*seed+1 &lt; 2<sup>32</sup></i>.
    /// In other words, there must hold <i>seed &gt;= 0 && seed &lt; 1073741823</i>.
    /// <p>
    /// <b>Quality:</b> This generator follows the multiplicative congruential method of the form                    
    /// <dt>
    /// <i>z(i+1) = a * z(i)(mod m)</i> with
    /// <i>a=663608941 (=0X278DDE6DL), m=2<sup>32</sup></i>.
    /// <dt>
    /// <i>z(i)</i> assumes all different values <i>0 &lt; 4*seed+1 &lt; m</i> during a full period of 2<sup>30</sup>.
    /// </summary>
    public class DRand : RandomEngine
    {

        #region Local Variables
        private int current;
        public static int DEFAULT_SEED = 1;
        #endregion

        #region Property

        #endregion

        #region Constructor
        /// <summary>
        /// Constructs and returns a random number generator with a default seed, which is a <b>constant</b>.
        /// </summary>
        public DRand(): this(DEFAULT_SEED)
        {
        }

        /// <summary>
        /// Constructs and returns a random number generator with the given seed.
        /// </summary>
        /// <param name="seed">should not be 0, in such a case <i>DRand.DEFAULT_SEED</i> is substituted.</param>
        public DRand(int seed)
        {
            SetSeed(seed);
        }

        /// <summary>
        /// Constructs and returns a random number generator seeded with the given date.
        /// </summary>
        /// <param name="d">typically <see cref="System.DateTime.Now"/></param>
        public DRand(DateTime d): this((int)DateTimeUtility.GetTime(d))
        {
        }
        #endregion

        #region Implement Methods
        /// <summary>
        /// Make a copy of this object
        /// </summary>
        /// <returns></returns>
        public override RandomEngine Clone()
        {
            RandomEngine copy = (RandomEngine)this.MemberwiseClone();

            return copy;
        }

        /// <summary>
        /// Returns a 32 bit uniformly distributed random number in the open unit interval <code>(0.0f,1.0f)</code> (excluding 0.0f and 1.0f).
        /// </summary>
        /// <returns></returns>
        public override uint NextUInt32()
        {
            return (uint)NextInt();
        }

        #endregion

        #region Local Public Methods
        /// <summary>
        /// Returns a 32 bit uniformly distributed random number in the closed interval <i>[int.MinValue,int.MaxValue]</i> (including <i>int.MinValue</i> and <i>int.MaxValue</i>).
        /// </summary>
        /// <returns></returns>
        public int NextInt()
        {
            current *= 0x278DDE6D;     /* z(i+1)=a*z(i)(mod 2**32) */
                                       // a == 0x278DDE6D == 663608941
            return current;
        }

        /// <summary>
        /// Sets the receiver's seedd 
        /// This method resets the receiver's entire internal state.
        /// The following condition must hold: <i>seed &gt;= 0 && seed &lt; (2<sup>32</sup>-1) / 4</i>.
        /// </summary>
        /// <param name="seed">if the above condition does not hold, a modified seed that meets the condition is silently substituted.</param>
        protected void SetSeed(int seed)
        {
            if (seed < 0) seed = -seed;
            int limit = (int)((System.Math.Pow(2, 32) - 1) / 4); // --> 536870911
            if (seed >= limit) seed = seed >> 3;

            this.current = 4 * seed + 1;
        }
        #endregion

        #region Local Private Methods

        #endregion
    }
}
