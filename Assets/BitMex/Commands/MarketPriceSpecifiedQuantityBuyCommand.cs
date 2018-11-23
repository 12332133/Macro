using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketPriceSpecifiedQuantityBuyCommand : BitMexCommand<MarketPriceSpecifiedQuantityBuyCommand>
    {
        public MarketPriceSpecifiedQuantityBuyCommand(IBitMexMainAdapter bitmexMain) 
            : base(bitmexMain)
        {
        }

        public MarketPriceSpecifiedQuantityBuyCommand(IBitMexMainAdapter bitmexMain, BitMexCommandType commandType, List<object> paramters)
            : base(bitmexMain, commandType, paramters)
        {
        }

        protected override MarketPriceSpecifiedQuantityBuyCommand Create()
        {
            return new MarketPriceSpecifiedQuantityBuyCommand(BitMexMain, CommandType, Parameters);
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.HandleOrderMarketQty(
                (decimal)Parameters[0], 
                0, 
                0, 
                BitMexMain.DriverService.HandleGetCurrentSymbol()) == true)
            {
                BitMexMain.DriverService.HandleBuy();
            }
        }

        public override string GetCommandText()
        {
            return "시장가 지정 수량 매수";
        }
    }
}
