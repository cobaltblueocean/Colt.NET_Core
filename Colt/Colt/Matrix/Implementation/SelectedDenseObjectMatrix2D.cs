using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Matrix.Implementation
{
    public class SelectedDenseObjectMatrix2D: ObjectMatrix2D
    {
        /// <summary>
        /// The elements of this matrix.
        /// </summary>
        protected internal Object[] Elements { get; private set; }

        public override object this[int row, int column]
        {
            get;
            set;
        }

        public override ObjectMatrix2D Like(int Rows, int Columns)
        {
            throw new NotImplementedException();
        }

        public override ObjectMatrix1D Like1D(int size)
        {
            throw new NotImplementedException();
        }

        protected override ObjectMatrix1D Like1D(int size, int zero, int stride)
        {
            throw new NotImplementedException();
        }

        protected override ObjectMatrix2D ViewSelectionLike(int[] rowOffsets, int[] columnOffsets)
        {
            throw new NotImplementedException();
        }
    }
}
