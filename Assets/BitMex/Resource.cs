using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.BitMex
{
    public static class Resource
    {
        public static string Dir
        {
            get
            {
                return  Application.streamingAssetsPath + "/";
            }
        }
    }
}
