using Assets.KeyBoardHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex
{
    public interface IBitMexMainAdapter
    {
        BitMexSession BitMexSession { get; }
        BitMexDriverService DriverService { get; }
        void ResisterMacro(List<RawKey> keys, BitMexCommandType type);
        Dictionary<BitMexCommandType, IBitMexActionCommand> BitMexCommandList { get; }
    }
}
