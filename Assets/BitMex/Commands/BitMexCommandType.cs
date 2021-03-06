﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public enum BitMexCommandTableType : ushort
    {
        Percent,
        Quantity,
        Etc,
    }

    public enum BitMexCommandType : ushort
    {
        None,

        // 시장가 구매 
        MarketPriceBuyMagnification,

        // 시장가 판매
        MarketPriceSellMagnification,

        // 지정가 구매
        MarketSpecifiedPriceBuy,

        // 지정가 판매
        MarketSpecifiedPriceSell,

        // 시장가 지정 수량 구매
        MarketPriceSpecifiedQuantityBuy,

        // 시장가 지정 수량 판매
        MarketPriceSpecifiedQuantitySell,

        // 지정가 지정 수량 구매
        MarketSpecifiedQuantityBuy,

        // 지정가 지정 수량 판매
        MarketSpecifiedQuantitySell,

        // 포지션 취소 (시장가)
        ClearPosition,

        // 가장 최근 활동 제거
        CancleTopActivateOrder,

        // 모든 확동 제거
        CancleAllActivateOrder,

        // 코인 탭 이동
        ChangeCoinTap,
    }
}
