using Assets.BitMex.Commands;
using Assets.CombinationKey;
using Bitmex.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex
{
    public interface IBitMexMainAdapter
    {
        BitmexSession Session { get; }
        BitMexCoinTable CoinTable { get; }
        BitmexApiService ApiService { get; }
        BitMexCommandExecutor CommandExecutor { get; }

        void SetHook();
        void ClearHook();

        ContentsBase.ModifyCommandPercentPopup<IBitMexCommand> PopupInput { get; }
        ContentsBase.ModifyCommandCoinTypePopup<IBitMexCommand> PopupDropdown { get; }
        ContentsBase.ContentsPopupMessage PopupMessage { get; }
    }
}
