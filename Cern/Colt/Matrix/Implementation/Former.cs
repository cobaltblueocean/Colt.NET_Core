// <copyright file="Former.cs" company="CERN">
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

namespace Cern.Colt.Matrix
{
    public class Former
    {
        private string _format;

        public Former(String format)
        {
            this._format = format;
        }

        /// <summary>
        /// Formats a double into a string (like sprintf in C).
        /// </summary>
        /// <param name="value">
        /// the number to format
        /// </param>
        /// <returns>
        /// the formatted string
        /// </returns>
        public delegate String formdlg(double value);
        public formdlg form;

        public String Format(double value)
        {
            if (String.IsNullOrEmpty(_format))
                return value.ToString();

            if (form == null)
            {
                return String.Format(_format, value);
            }

            return form(value); 
        }
    }

}
