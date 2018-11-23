using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketSpecifiedBuyCommand : BitMexCommand<MarketSpecifiedBuyCommand>
    {
        //public int Magnification { get; set; }

        public MarketSpecifiedBuyCommand(IBitMexMainAdapter bitmexMain, Action<List<object>> initializer) 
            : base(bitmexMain)
        {
            initializer(this.Parameters);
        }

        public MarketSpecifiedBuyCommand(IBitMexMainAdapter bitmexMain, BitMexCommandType commandType, List<object> paramters)
            : base(bitmexMain, commandType, paramters)
        {
        }

        protected override MarketSpecifiedBuyCommand Create()
        {
            return new MarketSpecifiedBuyCommand(BitMexMain, CommandType, Parameters);
        }

        public override void Execute()
        {
            //if (BitMexMain.DriverService.HandleOrderSpecifiedQty(
            //    0,
            //    Magnification,
            //    -BitMexMain.Session.SpecifiedAditional,
            //    BitMexMain.Session.FixedAvailableXbt,
            //    BitMexMain.DriverService.HandleGetCurrentSymbol()
            //    ) == true)
            //{
            //    BitMexMain.DriverService.HandleBuy();
            //}
        }

        public override string GetCommandText()
        {
            return "빠른 지정가 매수";
        }
    }
}
