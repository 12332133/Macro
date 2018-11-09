using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex
{
    public class BitMexDriverServiceException : Exception
    {
        public BitMexDriverServiceException(string action)
            :base(action)
        {
        }

        public BitMexDriverServiceException()
        {
        }
    }
}
