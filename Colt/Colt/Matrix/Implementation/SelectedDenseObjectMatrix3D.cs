using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cern.Colt.Matrix.Implementation
{

    /// <summary>
    /// Selection view on sparse 3-d matrices holding <i>Object</i> elements.
    /// First see the <a href="package-summary.html">package summary</a> and javadoc <a href="package-tree.html">tree view</a> to get the broad picture.
    /// <p>
    /// <b>Implementation:</b>
    /// <p>
    /// Objects of this class are typically constructed via <i>viewIndexes</i> methods on some source matrix.
    /// The interface introduced in abstract base classes defines everything a user can do.
    /// From a user point of view there is nothing special about this class; it presents the same functionality with the same signatures and semantics as its abstract baseclass(es) while introducing no additional functionality.
    /// Thus, this class need not be visible to users.
    /// By the way, the same principle applies to concrete DenseXXX and SparseXXX classes: they presents the same functionality with the same signatures and semantics as abstract baseclass(es) while introducing no additional functionality.
    /// Thus, they need not be visible to users, eitherd 
    /// Factory methods could hide all these concrete types.
    /// <p>
    /// This class uses no delegationd 
    /// Its instances point directly to the datad 
    /// Cell addressing overhead is is 1 additional int addition and 3 additional array index accesses per get/set.
    /// <p>
    /// Note that this implementation is not synchronized.
    /// <p>
    /// <b>Memory requirements:</b>
    /// <p>
    /// <i>memory [bytes] = 4*(sliceIndexes.Length+rowIndexes.Length+columnIndexes.Length)</i>.
    /// Thus, an index view with 100 x 100 x 100 indexes additionally uses 8 KB.
    /// <p>
    /// <b>Time complexity:</b>
    /// <p>
    /// Depends on the parent view holding cells.
    /// <p>
    /// @author wolfgang.hoschek@cern.ch
    /// @version 1.0, 09/24/99
    /// </summary>
    public class SelectedDenseObjectMatrix3D : ObjectMatrix3D
    {
        /// <summary>
        /// The elements of this matrix.
        /// </summary>
        protected internal IDictionary<int, Object> Elements { get; private set; }

        public override object this[int slice, int row, int column] {
            get
            {
                //if (debug) if (slice<0 || slice>=slices || row<0 || row>=rows || column<0 || column>=columns) throw new IndexOutOfRangeException("slice:"+slice+", row:"+row+", column:"+column);
                //return elements.Get(index(slice,row,column));
                //manually inlined:
                return Elements[offset + sliceOffsets[SliceZero + slice * SliceStride] + rowOffsets[RowZero + row * RowStride] + columnOffsets[ColumnZero + column * ColumnStride]];
            }
            set
            {
                //if (debug) if (slice<0 || slice>=slices || row<0 || row>=rows || column<0 || column>=columns) throw new IndexOutOfRangeException("slice:"+slice+", row:"+row+", column:"+column);
                //int index =	index(slice,row,column);
                //manually inlined:
                int index = offset + sliceOffsets[SliceZero + slice * SliceStride] + rowOffsets[RowZero + row * RowStride] + columnOffsets[ColumnZero + column * ColumnStride];
                if (value == null)
                    this.Elements.Remove(index);
                else
                    this.Elements[index] = value;
            }
        }

        /// <summary>
        /// The offsets of the visible cells of this matrix.
        /// </summary>
        protected int[] sliceOffsets;
        protected int[] rowOffsets;
        protected int[] columnOffsets;

        /// <summary>
        /// The offset.
        /// </summary>
        protected int offset;


    }
}
