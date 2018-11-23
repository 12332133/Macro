using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketSpecifiedSellCommand : BitMexCommand<MarketSpecifiedSellCommand>
    {
        //public int Magnification { get; set; }

        public MarketSpecifiedSellCommand(IBitMexMainAdapter bitmexMain, Action<List<object>> initializer) 
            : base(bitmexMain)
        {
            initializer(this.Parameters);
        }

        public MarketSpecifiedSellCommand(IBitMexMainAdapter bitmexMain, BitMexCommandType commandType, List<object> paramters)
            : base(bitmexMain, commandType, paramters)
        {
        }

        protected override MarketSpecifiedSellCommand Create()
        {
            return new MarketSpecifiedSellCommand(BitMexMain, CommandType, Parameters);
        }

        public override void Execute()
        {
            //if (BitMexMain.DriverService.HandleOrderSpecifiedQty(
            //    0,
            //    Magnification,
            //    BitMexMain.Session.SpecifiedAditional,
            //    BitMexMain.Session.FixedAvailableXbt,
            //    BitMexMain.DriverService.HandleGetCurrentSymbol()) == true)
            //{
            //    BitMexMain.DriverService.HandleSell();
            //}
        }

        public override string GetCommandText()
        {
            return "빠른 지정가 매도";
        }
    }
}
