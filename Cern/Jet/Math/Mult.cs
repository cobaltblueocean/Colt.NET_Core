using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Function;

namespace Cern.Jet.Math
{
    public class Mult: IDoubleFunction
    {
        private DoubleFunctionDelegate _eval;
        public double Multiplicator
        {
            get { return _multiplicator; }
            set { _multiplicator = value; }
        }

        public DoubleFunctionDelegate Eval { get { return _eval; } set { _eval = value; } }

        private double _multiplicator;

        protected Mult(double multiplicator)
        {
            this._multiplicator = multiplicator;
            _eval = new DoubleFunctionDelegate((a) => { return a * _multiplicator; });
        }

        public static Mult Div(double constant)
        {
            return CreateInstance(1 / constant);
        }

        public static Mult CreateInstance(double constant)
        {
            return new Mult(constant);
        }

        public double Apply(double x)
        {
            if (_eval == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return _eval(x);
            }

        }
    }
}
