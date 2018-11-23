using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public enum BitMexCommandType : ushort
    {
        None,
        //FixedAvailableXbt,
        //SpecifiedAditional,

        //시장가 구매 
        MarketPriceBuyMagnification1,
        MarketPriceBuyMagnification2,
        MarketPriceBuyMagnification3,
        MarketPriceBuyMagnification4,
        MarketPriceBuyMagnificationCustom,

        //시장가 판매
        MarketPriceSellMagnification1,
        MarketPriceSellMagnification2,
        MarketPriceSellMagnification3,
        MarketPriceSellMagnification4,
        MarketPriceSellMagnificationCustom,

        //지정가 구매
        MarketSpecifiedPriceBuy1,
        MarketSpecifiedPriceBuy2,
        MarketSpecifiedPriceBuy3,
        MarketSpecifiedPriceBuy4,
        MarketSpecifiedPriceBuyCustom,


        //지정가 판매
        MarketSpecifiedPriceSell1,
        MarketSpecifiedPriceSell2,
        MarketSpecifiedPriceSell3,
        MarketSpecifiedPriceSell4,
        MarketSpecifiedPriceSellCustom,

        //시장가 지정 수량 구매
        MarketPriceSpecifiedQuantityBuy,
        //시장가 지정 수량 판매
        MarketPriceSpecifiedQuantitySell,

        //지정가 지정 수량 구매
        MarketSpecifiedQuantityBuy,
        //지정가 지정 수량 판매
        MarketSpecifiedQuantitySell,

        ClearPosition,
        CancleTopActivateOrder,
        CancleAllActivateOrder,

        ChangeCoinTap,
    }
}
