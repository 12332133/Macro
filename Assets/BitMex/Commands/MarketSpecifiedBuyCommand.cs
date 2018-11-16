using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketSpecifiedBuyCommand : BitMexCommand
    {
        //public int Magnification { get; set; }

        public MarketSpecifiedBuyCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose)
            : base(bitmexMain, contentString, isExpose)
        {
        }

        public override object Clone()
        {
            return new MarketSpecifiedBuyCommand(BitMexMain, ContentString, IsExpose);
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
    }
}
