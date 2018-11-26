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
        MarketPriceBuyMagnification,
        //MarketPriceBuyMagnificationCreate,

        //시장가 판매
        MarketPriceSellMagnification,
        //MarketPriceSellMagnificationCreate,

        //지정가 구매
        MarketSpecifiedPriceBuy,
        //MarketSpecifiedPriceBuyCreate,

        //지정가 판매
        MarketSpecifiedPriceSell,
        MarketSpecifiedPriceSellCreate,

        //시장가 지정 수량 구매
        MarketPriceSpecifiedQuantityBuy,
        //MarketPriceSpecifiedQuantityBuyCreate,

        //시장가 지정 수량 판매
        MarketPriceSpecifiedQuantitySell,
        //MarketPriceSpecifiedQuantitySellCreate,

        //지정가 지정 수량 구매
        MarketSpecifiedQuantityBuy,
        //MarketSpecifiedQuantityBuyCreate,

        //지정가 지정 수량 판매
        MarketSpecifiedQuantitySell,
        //MarketSpecifiedQuantitySellCreate,

        OrderCommandCreate,

        ClearPosition,
        CancleTopActivateOrder,
        CancleAllActivateOrder,

        ChangeCoinTap,
    }
}
