using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketPriceBuyCommand : BitMexCommand
    {
        public MarketPriceBuyCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose)
            : base(bitmexMain, contentString, isExpose)
        {
        }

        public override object Clone()
        {
            return new MarketPriceBuyCommand(BitMexMain, ContentString, IsExpose);
        }

        public override void Execute()
        {
            //수량 퍼센트 동시 가능??

            if (BitMexMain.DriverService.HandleOrderMarketQty(
                Parameters[0],
                Parameters[1],
                BitMexMain.DriverService.FixedAvailableXbt,
                BitMexMain.DriverService.HandleGetCurrentSymbol()
                ) == true)
            {
                BitMexMain.DriverService.HandleBuy();
            }
        }
    }
}
