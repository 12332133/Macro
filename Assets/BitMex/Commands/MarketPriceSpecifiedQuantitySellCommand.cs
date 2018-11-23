using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketPriceSpecifiedQuantitySellCommand : BitMexCommand<MarketPriceSpecifiedQuantitySellCommand>
    {
        public MarketPriceSpecifiedQuantitySellCommand(IBitMexMainAdapter bitmexMain) 
            : base(bitmexMain)
        {
        }

        public MarketPriceSpecifiedQuantitySellCommand(IBitMexMainAdapter bitmexMain, BitMexCommandType commandType, List<object> paramters)
            : base(bitmexMain, commandType, paramters)
        {
        }

        protected override MarketPriceSpecifiedQuantitySellCommand Create()
        {
            return new MarketPriceSpecifiedQuantitySellCommand(BitMexMain, CommandType, Parameters);
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.HandleOrderMarketQty(
                (decimal)Parameters[0], 
                0, 
                0, 
                BitMexMain.DriverService.HandleGetCurrentSymbol()) == true)
            {
                BitMexMain.DriverService.HandleSell();
            }
        }

        public override string GetCommandText()
        {
            return "시장가 지정 수량 매도";
        }
    }
}
