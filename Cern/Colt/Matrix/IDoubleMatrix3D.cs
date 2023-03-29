using System;
using Cern.Colt.Function;
using Cern.Colt.List;
using System.Collections.Generic;
using Cern.Colt.Matrix.Implementation;

namespace Cern.Colt.Matrix
{
    public interface IDoubleMatrix3D : IMatrix3D<double>, IEnumerable<double>
    {
        double Aggregate(IDoubleDoubleFunction aggr, IDoubleFunction f);
        double Aggregate(IDoubleMatrix3D other, IDoubleDoubleFunction aggr, IDoubleDoubleFunction f);
        IDoubleMatrix3D Assign(double value);
        IDoubleMatrix3D Assign(double[,,] values);
        IDoubleMatrix3D Assign(double[][][] values);
        IDoubleMatrix3D Assign(IDoubleFunction function);
        IDoubleMatrix3D Assign(IDoubleMatrix3D other);
        IDoubleMatrix3D Assign(IDoubleMatrix3D y, IDoubleDoubleFunction function);
        int Cardinality();
        IDoubleMatrix3D Copy();
        bool Equals(double value);
        bool Equals(object obj);
        IDoubleMatrix3D GetContent();
        Boolean HaveSharedCells(IDoubleMatrix3D other);
        Boolean HaveSharedCellsRaw(IDoubleMatrix3D other);
        IDoubleMatrix3D View();
        IDoubleMatrix3D Like();
        IDoubleMatrix3D Like(int Slices, int Rows, int Columns);
        IDoubleMatrix2D ViewColumn(int column);
        IDoubleMatrix3D ViewColumnFlip();
        IDoubleMatrix3D ViewDice(int axis0, int axis1, int axis2);
        IDoubleMatrix3D ViewPart(int slice, int row, int column, int depth, int height, int width);
        IDoubleMatrix2D ViewRow(int row);
        IDoubleMatrix3D ViewRowFlip();
        IDoubleMatrix3D ViewSelection(DoubleMatrix2DProcedure condition);
        IDoubleMatrix3D ViewSelection(int[] sliceIndexes, int[] rowIndexes, int[] columnIndexes);
        IDoubleMatrix2D ViewSlice(int slice);
        IDoubleMatrix3D ViewSliceFlip();
        IDoubleMatrix3D ViewSorted(int row, int column);
        IDoubleMatrix3D ViewStrides(int Slicestride, int Rowstride, int Columnstride);
        int GetHashCode();
        void GetNonZeros(IntArrayList sliceList, IntArrayList rowList, IntArrayList columnList, DoubleArrayList valueList);
        double GetQuick(int slice, int row, int column);
        void SetQuick(int slice, int row, int column, double value);
        double[][][] ToArray();
        string ToString();
        void ZAssign27Neighbors(IDoubleMatrix3D B, Double27FunctionDelegate function);
        double ZSum();
    }
}