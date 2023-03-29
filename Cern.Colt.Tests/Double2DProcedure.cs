using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Matrix;

namespace Cern.Colt.Tests
{
    public abstract class Double2DProcedure
    {
        public delegate void TimerProcedure(Timer timer);

        public TimerProcedure timerProc;
        public IDoubleMatrix2D A;
        public IDoubleMatrix2D B;
        public IDoubleMatrix2D C;
        public IDoubleMatrix2D D;

        public abstract void Init();

        /**
         * The number of operations a single call to "apply" involves.
         */
        public virtual double Operations()
        {
            return A.Rows * A.Columns / 1.0E6;
        }
        /**
         * Sets the matrices to operate upon.
         */
        public virtual void SetParameters(IDoubleMatrix2D A, IDoubleMatrix2D B)
        {
            this.A = A;
            this.B = B;
            this.C = A.Copy();
            this.D = B.Copy();
        }
    }
}
