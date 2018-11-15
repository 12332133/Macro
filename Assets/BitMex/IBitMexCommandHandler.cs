using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex
{
    public interface IBitMexCommandHandler
    {
        bool HandleBuy();
        bool HandleSell();
        string HandleGetCurrentSymbol();
        int HandleGetCurrentPositionCount(string symbol);
        decimal HandleGetCrossSelectionMaxLeverage();
        bool HandleIsTradingPage();
    }
}
