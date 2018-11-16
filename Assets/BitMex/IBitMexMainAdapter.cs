using Assets.BitMex.Commands;
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
        BitMexCommandExecutor CommandExecutor { get; }
        BitMexCommandRepository CommandRepository { get; }
        BitMexCoinTable CoinTable { get; }
        IBitMexCommandHandler CommandHandler { get; }
        bool ResisterMacro(List<RawKey> keys, BitMexCommandType type);
        void ResisterPriceSchedule(IBitMexPriceSchedule schedule);
        void WriteMacroLog(string log);
    }
}
