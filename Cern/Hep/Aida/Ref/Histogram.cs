// <copyright file="Histogram.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2018.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Hep.Aida;

namespace Cern.Hep.Aida.Ref
{
    public abstract class Histogram : IHistogram
    {
        private String title;

        public Histogram(String title)
        {
            this.title = title;
        }

        public abstract int AllEntries { get; }

        public abstract int Dimensions { get; }

        public abstract int Entries { get; }

        public abstract double EquivalentBinEntries { get; }

        public abstract int ExtraEntries { get; }

        public abstract void Reset();

        public abstract double SumAllBinHeights { get; }

        public abstract double SumBinHeights { get; }

        public abstract double SumExtraBinHeights { get; }

        public string Title
        {
            get { return title; }
        }
    }
}
