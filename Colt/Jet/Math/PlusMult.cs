using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Function;

namespace Cern.Jet.Math
{
    public sealed class PlusMult
    {

        #region Local Variables
        public double Multiplicator;
        public DoubleDoubleFunction Apply;
        #endregion

        #region Property

        #endregion

        #region Constructor
        public PlusMult(double multiplicator)
        {
            this.Multiplicator = multiplicator;
            Apply = new DoubleDoubleFunction((a, b) => { return a + b * Multiplicator; });
        }

        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods
        /**
         * <tt>a - b/constant</tt>.
         */
        public static PlusMult MinusDiv(double constant)
        {
            return new PlusMult(-1 / constant);
        }
        /**
         * <tt>a - b*constant</tt>.
         */
        public static PlusMult MinusMult(double constant)
        {
            return new PlusMult(-constant);
        }
        /**
         * <tt>a + b/constant</tt>.
         */
        public static PlusMult PlusDiv(double constant)
        {
            return new PlusMult(1 / constant);
        }
        /**
         * <tt>a + b*constant</tt>.
         */
        public static PlusMult Plus(double constant)
        {
            return new PlusMult(constant);
        }
        #endregion

        #region Local Private Methods

        #endregion

    }
}
