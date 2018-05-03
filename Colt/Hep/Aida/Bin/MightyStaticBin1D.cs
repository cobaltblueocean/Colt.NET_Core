using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Hep.Aida.Bin
{
    public class MightyStaticBin1D: StaticBin1D
    {

        #region Local Variables
        protected Boolean hasSumOfLogarithms = false;
        protected double sumOfLogarithms = 0.0; // Sum( Log(x[i]) )

        protected Boolean hasSumOfInversions = false;
        protected double sumOfInversions = 0.0; // Sum( 1/x[i] )

        protected double[] sumOfPowers = null;  // Sum( x[i]^3 ) .d Sum( x[i]^max_k )
        #endregion

        #region Property

        #endregion

        #region Constructor

        #endregion

        #region Implement Methods

        #endregion

        #region Local Public Methods

        #endregion

        #region Local Private Methods

        #endregion

    }
}
