using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketSpecifiedQuantityBuyCommand : BitMexCommand<MarketSpecifiedQuantityBuyCommand>
    {
        public MarketSpecifiedQuantityBuyCommand(IBitMexMainAdapter bitmexMain) 
            : base(bitmexMain)
        {
        }

        public MarketSpecifiedQuantityBuyCommand(IBitMexMainAdapter bitmexMain, BitMexCommandType commandType, List<object> paramters)
            : base(bitmexMain, commandType, paramters)
        {
        }

        protected override MarketSpecifiedQuantityBuyCommand Create()
        {
            return new MarketSpecifiedQuantityBuyCommand(BitMexMain, CommandType, Parameters);
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.HandleOrderSpecifiedQty(
                (decimal)Parameters[0],
                0,
                -(decimal)Parameters[1],
                0,
                BitMexMain.DriverService.HandleGetCurrentSymbol()
                ) == true)
            {
                BitMexMain.DriverService.HandleBuy();
            }
        }

        public override string GetCommandText()
        {
            return "지정가 지정 수량 매수";
        }
    }
}
