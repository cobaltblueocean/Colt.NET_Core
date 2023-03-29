// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDoubleMatrix1D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentation. 
//   CERN makes no representations about the suitability of this software for any purpose. 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   A condition or procedure : takes a single argument and returns a boolean value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Cern.Colt.Function;
using Cern.Colt.List;
using Cern.Colt.Matrix.Implementation;
using Cern.Jet.Math;
using System.Collections.Generic;

namespace Cern.Colt.Matrix
{
    public interface IDoubleMatrix1D: IMatrix1D<double>, IEnumerable<double>
    {
        double Aggregate(IDoubleDoubleFunction aggr, IDoubleFunction f);
        double Aggregate(IMatrix1D<double> other, IDoubleDoubleFunction aggr, IDoubleDoubleFunction f);
        IDoubleMatrix1D Assign(double value);
        IDoubleMatrix1D Assign(double[] values);
        IDoubleMatrix1D Assign(IDoubleFunction function);
        IDoubleMatrix1D Assign(IDoubleMatrix1D other);
        IDoubleMatrix1D Assign(IDoubleMatrix1D y, IDoubleDoubleFunction function);
        IDoubleMatrix1D Assign(IDoubleMatrix1D y, IDoubleDoubleFunction function, IntArrayList nonZeroIndexes);
        //IDoubleMatrix1D Assign(IDoubleMatrix1D y, PlusMult function);
        //IDoubleMatrix1D Assign(IDoubleMatrix1D y, PlusMult function, IntArrayList nonZeroIndexes);
        //IDoubleMatrix1D Assign(Mult mult);
        int Cardinality();
        IDoubleMatrix1D Copy();
        bool Equals(double value);
        bool Equals(object obj);
        IDoubleMatrix1D GetContent();
        int GetHashCode();
        void GetNonZeros(IntArrayList indexList, List<double> valueList);
        void GetNonZeros(IntArrayList indexList, List<double> valueList, int maxCardinality);
        double GetQuick(int index);
        bool HaveSharedCells(IDoubleMatrix1D other);
        bool HaveSharedCellsRaw(IDoubleMatrix1D other);
        IDoubleMatrix1D Like();
        IDoubleMatrix1D Like(int size);
        IDoubleMatrix2D Like2D(int rows, int columns);
        void SetQuick(int index, double value);
        void Swap(IDoubleMatrix1D other);
        double[] ToArray();
        void ToArray(ref double[] values);
        IDoubleMatrix1D View();
        IDoubleMatrix1D ViewFlip();
        IDoubleMatrix1D ViewPart(int index, int width);
        IDoubleMatrix1D ViewSelection(IDoubleProcedure condition);
        IDoubleMatrix1D ViewSelection(int[] indexes);
        IDoubleMatrix1D ViewSelectionLike(int[] offsets);
        IDoubleMatrix1D ViewSorted();
        IDoubleMatrix1D ViewStrides(int s);
        double ZDotProduct(IDoubleMatrix1D y);
        double ZDotProduct(IDoubleMatrix1D y, int from, int length);
        double ZDotProduct(IDoubleMatrix1D y, int from, int length, IntArrayList nonZeroIndexes);
        double ZDotProduct(IDoubleMatrix1D y, IntArrayList nonZeroIndexes);
        double ZSum();
    }
}