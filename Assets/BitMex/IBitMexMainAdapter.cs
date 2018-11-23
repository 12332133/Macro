using Assets.BitMex.Commands;
using Assets.CombinationKey;
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
        BitMexCommandExecutor CommandExecutor { get; }
        BitMexCommandRepository CommandRepository { get; }
        BitMexCoinTable CoinTable { get; }
        BitMexMacro Macro { get; }
        void ResisterSchedule(IBitMexSchedule schedule);
        void WriteMacroLog(string log);
    }
}
