﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class IListExtension2
    {

        public static Boolean ForEach<T>(this IList<Double> list, Cern.Colt.Function.DoubleProcedure procedure)
        {
            Double[] theElements = list.ToArray();
            int theSize = list.Count;

            for (int i = 0; i < theSize;) if (!procedure(theElements[i++])) return false;
            return true;
        }

    }
}
