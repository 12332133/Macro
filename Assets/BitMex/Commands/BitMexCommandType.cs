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

        MarketPriceBuy10Magnification,
        MarketPriceBuy25Magnification,
        MarketPriceBuy50Magnification,
        MarketPriceBuy100Magnification,

        MarketPriceSell10Magnification,
        MarketPriceSell25Magnification,
        MarketPriceSell50Magnification,
        MarketPriceSell100Magnification,

        MarketSpecified10PriceBuy,
        MarketSpecified25PriceBuy,
        MarketSpecified50PriceBuy,
        MarketSpecified100PriceBuy,

        MarketSpecified10PriceSell,
        MarketSpecified25PriceSell,
        MarketSpecified50PriceSell,
        MarketSpecified100PriceSell,

        ClearPosition,
        CancleTopActivateOrder,
        CancleAllActivateOrder
    }
}
