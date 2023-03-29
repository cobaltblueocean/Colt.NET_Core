using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.List;

namespace Cern.Jet.Stat.Quantile
{
    public interface IDoubleQuantileFinder
    {
        void Add(double value);

        void AddAllOf(DoubleArrayList values);

        void AddAllOfFromTo(DoubleArrayList values, int from, int to);

        void Clear();

        Object Clone();

        Boolean ForEach(Cern.Colt.Function.DoubleProcedureDelegate procedure);

        long Memory();

        double Phi(double element);

        DoubleArrayList QuantileElements(DoubleArrayList phis);

        long Size { get; }

        long TotalMemory();

    }
}
