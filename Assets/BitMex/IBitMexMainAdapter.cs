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
        BitMexCoinTable CoinTable { get; }

        ContentsBase.ModifyCommandPercentPopup<IBitMexCommand> PopupInput { get; }
        ContentsBase.ModifyCommandCoinTypePopup<IBitMexCommand> PopupDropdown { get; }
        ContentsBase.ContentsPopupMessage PopupMessage { get; }
    }
}
