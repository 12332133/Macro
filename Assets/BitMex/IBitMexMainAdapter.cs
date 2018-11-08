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
        BitMexSession Session { get; }
        BitMexDriverService DriverService { get; }
        BitMexCommandRepository CommandRepository { get; }
        BitMexCommandExecutor CommandExecutor { get; }
        bool ResisterMacro(List<RawKey> keys, BitMexCommandType type);
        void WriteMacroLog(string log);
    }
}
