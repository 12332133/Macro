using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public enum BitMexCommandType
    {
        None,
        Test,
        //FixedAvailableXbt,
        //SpecifiedAditional,

        MarketPriceBuyMagnification,
        //MarketPriceBuy25Magnification,
        //MarketPriceBuy50Magnification,
        //MarketPriceBuy100Magnification,

        MarketPriceSellMagnification,
        //MarketPriceSell25Magnification,
        //MarketPriceSell50Magnification,
        //MarketPriceSell100Magnification,

        MarketSpecifiedPriceBuy,
        //MarketSpecified25PriceBuy,
        //MarketSpecified50PriceBuy,
        //MarketSpecified100PriceBuy,

        MarketSpecifiedPriceSell,
        //MarketSpecified25PriceSell,
        //MarketSpecified50PriceSell,
        //MarketSpecified100PriceSell,

        ClearPosition,
        CancleTopActivateOrder,
        CancleAllActivateOrder
    }
}
