using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.BitMex
{
    public class BitMexDriverServiceException : Exception
    {
        private readonly string DataPath = Application.dataPath + "/Resources/Log/exception.txt";

        public BitMexDriverServiceException(string action)
            :base(action)
        {
        }

        public BitMexDriverServiceException()
        {
            Task.Run(() => {
                File.AppendAllLines(this.DataPath,
                    new[] { string.Format("[{0}] Message : {1} StackTrace {2}",
                    DateTime.Now.ToString(),
                    this.Message,
                    this.StackTrace)});
            });
        }
    }
}
