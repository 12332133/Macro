using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketSpecifiedQuantitySellCommand : BitMexCommand<MarketSpecifiedQuantitySellCommand>
    {
        public MarketSpecifiedQuantitySellCommand(IBitMexMainAdapter bitmexMain) 
            : base(bitmexMain)
        {
        }

        public MarketSpecifiedQuantitySellCommand(IBitMexMainAdapter bitmexMain, BitMexCommandType commandType, List<object> paramters)
            : base(bitmexMain, commandType, paramters)
        {
        }

        protected override MarketSpecifiedQuantitySellCommand Create()
        {
            return new MarketSpecifiedQuantitySellCommand(BitMexMain, CommandType, Parameters);
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.HandleOrderSpecifiedQty(
                (decimal)Parameters[0],
                0,
                (decimal)Parameters[1], 
                0,
                BitMexMain.DriverService.HandleGetCurrentSymbol()
                ) == true)
            {
                BitMexMain.DriverService.HandleBuy();
            }
        }

        public override string GetCommandText()
        {
            return "지정가 지정 수량 매도";
        }
    }
}
