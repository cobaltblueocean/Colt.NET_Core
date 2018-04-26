using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Jet.Stat.Quantile
{
    public interface IDoubleQuantileFinder
    {
        void Add(double value);

        void AddAllOf(List<Double> values);

        void AddAllOfFromTo(List<Double> values, int from, int to);

        void Clear();

        Object Clone();

        Boolean ForEach(Cern.Colt.Function.DoubleProcedure procedure);

        long Memory();

        double Phi(double element);

        List<Double> QuantileElements(List<Double> phis);

        long Size { get; }

        long TotalMemory();

    }
}
