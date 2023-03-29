// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDoubleMatrix2D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentationd 
//   CERN makes no representations about the suitability of this software for any purposed 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Mauro Mazzieri, 2010.
// </copyright>
// <summary>
//   Abstract base class for 2-d matrices holding <tt>double</tt> elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Cern.Colt.Function;
using Cern.Colt.List;
using System.Collections.Generic;
using Cern.Colt.Matrix.Implementation;

namespace Cern.Colt.Matrix
{
    public interface IDoubleMatrix2D : IMatrix2D<double>, IEnumerable<double>
    {
        bool IsDiagonal { get; }
        bool IsSquare { get; }

        double Aggregate(IDoubleDoubleFunction aggr, IDoubleFunction f);
        double Aggregate(IDoubleMatrix2D other, IDoubleDoubleFunction aggr, IDoubleDoubleFunction f);
        IDoubleMatrix2D Assign(double value);
        IDoubleMatrix2D Assign(double[,] values);
        IDoubleMatrix2D Assign(double[][] values);
        IDoubleMatrix2D Assign(IDoubleFunction function);
        IDoubleMatrix2D Assign(IDoubleMatrix2D other);
        IDoubleMatrix2D Assign(IDoubleMatrix2D y, IDoubleDoubleFunction function);
        int Cardinality();
        IDoubleMatrix2D Copy();
        bool Equals(double value);
        bool Equals(object obj);
        IDoubleMatrix2D ForEachNonZero(IntIntDoubleFunctionDelegate function);
        int GetHashCode();
        IDoubleMatrix2D GetContent();
        bool HaveSharedCells(IDoubleMatrix2D other);
        bool HaveSharedCellsRaw(IDoubleMatrix2D other);
        void GetNonZeros(IntArrayList rowList, IntArrayList columnList, List<double> valueList);
        double GetQuick(int row, int column);
        IDoubleMatrix2D Like();
        IDoubleMatrix2D Like(int rows, int columns);
        IDoubleMatrix1D Like1D(int size);
        IDoubleMatrix1D Like1D(int size, int zero, int stride);
        IEnumerable<DoubleMatrix2D.Element> NonZeroElements();
        void SetQuick(int row, int column, double value);
        IDoubleMatrix1D ViewColumn(int column);
        IDoubleMatrix2D ViewColumnFlip();
        IDoubleMatrix2D ViewDice();
        IDoubleMatrix2D ViewPart(int row, int column, int height, int width);
        IDoubleMatrix1D ViewRow(int row);
        IDoubleMatrix2D ViewRowFlip();
        IDoubleMatrix2D ViewSelection(IDoubleMatrix1DProcedure condition);
        IDoubleMatrix2D ViewSelection(int[] rowIndexes, int[] columnIndexes);
        IDoubleMatrix2D ViewSorted(int column);
        IDoubleMatrix2D ViewStrides(int rStride, int cStride);
        double[][] ToArray();
        void ZAssign8Neighbors(IDoubleMatrix2D b, Double9FunctionDelegate function);
        IDoubleMatrix1D ZMult(IDoubleMatrix1D y, IDoubleMatrix1D z);
        IDoubleMatrix1D ZMult(IDoubleMatrix1D y, IDoubleMatrix1D z, double alpha, double beta, bool transposeA);
        IDoubleMatrix2D ZMult(IDoubleMatrix2D b, IDoubleMatrix2D c);
        IDoubleMatrix2D ZMult(IDoubleMatrix2D b, IDoubleMatrix2D c, double alpha, double beta, bool transposeA, bool transposeB);
        double ZSum();
    }
}