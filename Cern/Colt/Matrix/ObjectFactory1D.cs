// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDoubleMatrix1D.cs" company="CERN">
//   Copyright © 1999 CERN - European Organization for Nuclear Research.
//   Permission to use, copy, modify, distribute and sell this software and its documentation for any purpose 
//   is hereby granted without fee, provided that the above copyright notice appear in all copies and 
//   that both that copyright notice and this permission notice appear in supporting documentationd 
//   CERN makes no representations about the suitability of this software for any purposed 
//   It is provided "as is" without expressed or implied warranty.
//   Ported from Java to C# by Kei Nakai, 2020.
// </copyright>
// <summary>
//   A condition or procedure : takes a single argument and returns a Boolean value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cern.Colt.Matrix.Implementation;

namespace Cern.Colt.Matrix
{
	public class ObjectFactory1D : PersistentObject
	{
		/// <summary>
		/// A factory producing dense matrices.
		/// <summary>
		public static ObjectFactory1D dense = new ObjectFactory1D();

		/// <summary>
		/// A factory producing sparse matrices.
		/// <summary>
		public static ObjectFactory1D sparse = new ObjectFactory1D();
		/// <summary>
		/// Makes this class non instantiable, but still let's others inherit from it.
		/// <summary>
		protected ObjectFactory1D() { }
		/// <summary>
		/// C = A||B; Constructs a new matrix which is the concatenation of two other matrices.
		/// Example: <i>0 1</i> append<i>3 4</i> --> <i>0 1 3 4</i>.
		/// <summary>
		public ObjectMatrix1D Append(ObjectMatrix1D A, ObjectMatrix1D B)
		{
			// concatenate
			ObjectMatrix1D matrix = Make(A.Count() + B.Count());
			matrix.ViewPart(0, A.Count()).Assign(A);
			matrix.ViewPart(A.Count(), B.Count()).Assign(B);
			return matrix;
		}
		/// <summary>
		/// Constructs a matrix which is the concatenation of all given parts.
		/// Cells are copied.
		/// <summary>
		public ObjectMatrix1D Make(ObjectMatrix1D[] parts)
		{
			if (parts.Length == 0) return Make(0);

			int size = 0;
			for (int i = 0; i < parts.Length; i++) size += parts[i].Count();

			ObjectMatrix1D vector = Make(size);
			size = 0;
			for (int i = 0; i < parts.Length; i++)
			{
				vector.ViewPart(size, parts[i].Count()).Assign(parts[i]);
				size += parts[i].Count();
			}

			return vector;
		}
		/// <summary>
		/// Constructs a matrix with the given cell values.
		/// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
		/// 
		/// <summary>
		/// <param name="values">The values to be filled into the new matrix.</param>
		public ObjectMatrix1D Make(Object[] values)
		{
			if (this == sparse) return new SparseObjectMatrix1D(values);
			else return new DenseObjectMatrix1D(values);
		}
		/// <summary>
		/// Constructs a matrix with the given shape, each cell initialized with zero.
		/// <summary>
		public ObjectMatrix1D Make(int size)
		{
			if (this == sparse) return new SparseObjectMatrix1D(size);
			return new DenseObjectMatrix1D(size);
		}
		/// <summary>
		/// Constructs a matrix with the given shape, each cell initialized with the given value.
		/// <summary>
		public ObjectMatrix1D Make(int size, Object initialValue)
		{
			return Make(size).Assign(initialValue);
		}
		/// <summary>
		/// Constructs a matrix from the values of the given list.
		/// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the matrix, and vice-versa.
		/// 
		/// <summary>
		/// <param name="values">The values to be filled into the new matrix.</param>
		/// <returns>a new matrix.</returns>
		public ObjectMatrix1D Make(Cern.Colt.List.ObjectArrayList values)
		{
			int size = values.Count();
			ObjectMatrix1D vector = Make(size);
			for (int i = size; --i >= 0;) vector[i] = values[i];
			return vector;
		}
		/// <summary>
		///		C = A||A||..||A; Constructs a new matrix which is concatenated<i> repeat</i> times.
		///		Example:
		///<pre>
		/// 0 1
		/// repeat(3) -->
		/// 0 1 0 1 0 1
		///</pre>
		/// <summary>
		public ObjectMatrix1D Repeat(ObjectMatrix1D A, int repeat)
		{
			int size = A.Count();
			ObjectMatrix1D matrix = Make(repeat * size);
			for (int i = repeat; --i >= 0;)
			{
				matrix.ViewPart(size * i, size).Assign(A);
			}
			return matrix;
		}
		/// <summary>
		/// Constructs a list from the given matrix.
		/// The values are copiedd So subsequent changes in <i>values</i> are not reflected in the list, and vice-versa.
		/// 
		/// <summary>
		/// <param name="values">The values to be filled into the new list.</param>
		/// <returns>a new list.</returns>
		public Cern.Colt.List.ObjectArrayList ToList(ObjectMatrix1D values)
		{
			int size = values.Count();
			Cern.Colt.List.ObjectArrayList list = new Cern.Colt.List.ObjectArrayList(size);
			list.SetSize(size);
			for (int i = size; --i >= 0;) list[i] = values[i];
			return list;
		}
	}
}
