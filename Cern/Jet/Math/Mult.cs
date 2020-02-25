using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Function;

namespace Cern.Jet.Math
{
    public class Mult
    {
        public double Multiplicator
        {
            get { return _multiplicator; }
            set { _multiplicator = value; }
        }

        private double _multiplicator;

        protected Mult(double multiplicator)
        {
            this._multiplicator = multiplicator;
            Apply = new DoubleFunction((a) => { return a * _multiplicator; });
        }

        public DoubleFunction Apply;

        public static Mult Div(double constant)
        {
            return CreateInstance(1 / constant);
        }

        public static Mult CreateInstance(double constant)
        {
            return new Mult(constant);
        }
    }
}
