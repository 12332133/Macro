using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketPriceBuyCommand : BitMexCommand
    {
        public int Magnification { get; set; }

        public MarketPriceBuyCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose, int magnification)
            : base(bitmexMain, contentString, isExpose)
        {
            Magnification = magnification;
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            if (BitMexMain.DriverService.HandleOrderMarketQty(
                0,
                Magnification,
                BitMexMain.Session.FixedAvailableXbt,
                BitMexMain.DriverService.HandleGetCurrentSymbol()
                ) == true)
            {
                BitMexMain.DriverService.HandleBuy();
            }
        }
    }
}
