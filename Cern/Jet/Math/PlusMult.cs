using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Function;

namespace Cern.Jet.Math
{
    public class PlusMult : IDoubleDoubleFunction
    {
        #region Local Variables
        public double Multiplicator;
        private DoubleDoubleFunctionDelegate _eval;
        public DoubleDoubleFunctionDelegate Eval {
            get { return _eval; }
            set {
                _eval = value;
            }
        }

        #endregion

        #region Property

        #endregion

        #region Constructor
        public PlusMult(double multiplicator)
        {
            this.Multiplicator = multiplicator;
            Eval = new DoubleDoubleFunctionDelegate((a, b) => { return a + b * Multiplicator; });
        }
        #endregion

        #region Implement Methods
        public double Apply(double x, double y)
        {
            if (_eval == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return _eval(x, y);
            }
        }

        #endregion

        #region Local Public Methods
        /// <summary>
        /// <tt>a - b/constant</tt>.
        /// <summary>
        public static PlusMult MinusDiv(double constant)
        {
            return new PlusMult(-1 / constant);
        }
        /// <summary>
        /// <tt>a - b*constant</tt>.
        /// <summary>
        public static PlusMult MinusMult(double constant)
        {
            return new PlusMult(-constant);
        }
        /// <summary>
        /// <tt>a + b/constant</tt>.
        /// <summary>
        public static PlusMult PlusDiv(double constant)
        {
            return new PlusMult(1 / constant);
        }
        /// <summary>
        /// <tt>a + b*constant</tt>.
        /// <summary>
        public static PlusMult Plus(double constant)
        {
            return new PlusMult(constant);
        }
        #endregion

        #region Local Private Methods

        #endregion
    }
}
