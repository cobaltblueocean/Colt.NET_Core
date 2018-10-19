using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class UnsupportedPlatformException : Exception
    {
        public UnsupportedPlatformException()
            : this("Platform is not supported.")
        {

        }

        public UnsupportedPlatformException(string message)
            : base(message)
        {

        }
    }
}
