using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Function
{
    public interface IDoubleDoubleMultFunction: IDoubleDoubleFunction
    {
        double Multiplicator { get; set; }
    }
}
