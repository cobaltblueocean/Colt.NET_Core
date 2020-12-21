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
            : this(Cern.LocalizedResources.Instance().PlatformNotSupportedMessage)
        {

        }

        public UnsupportedPlatformException(string message)
            : base(message)
        {

        }
    }
}
