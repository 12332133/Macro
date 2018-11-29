using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

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
