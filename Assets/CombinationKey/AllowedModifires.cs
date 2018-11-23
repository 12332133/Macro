using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.CombinationKey
{
    public static class AllowedModifire
    {
        private static Dictionary<RawKey, int> modifires = new Dictionary<RawKey, int>();

        static AllowedModifire()
        {
            AllowedModifire.modifires.Add(RawKey.LeftShift, 1);
            AllowedModifire.modifires.Add(RawKey.Shift, 1);
            AllowedModifire.modifires.Add(RawKey.RightShift, 1);
            AllowedModifire.modifires.Add(RawKey.Control, 1);
            AllowedModifire.modifires.Add(RawKey.LeftControl, 1);
            AllowedModifire.modifires.Add(RawKey.RightControl, 1);
            AllowedModifire.modifires.Add(RawKey.Menu, 1);
            AllowedModifire.modifires.Add(RawKey.LeftMenu, 1);
            AllowedModifire.modifires.Add(RawKey.RightMenu, 1);
        }

        public static bool IsAllowed(RawKey key)
        {
            return AllowedModifire.modifires.ContainsKey(key);
        }
    }
}
