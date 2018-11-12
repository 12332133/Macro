using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class ActivateOrderCancleCommand : BitMexCommand
    {
        public ActivateOrderCancleCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose)
            : base(bitmexMain, contentString, isExpose)
        {
        }

        public override void Execute()
        {
            if (BitMexMain.DriverService.IsInvaildEmail(BitMexMain.Session.Email) == false)
            {
                return;
            }

            BitMexMain.DriverService.HandleCancleActivatedOrders(BitMexMain.DriverService.HandleGetCurrentSymbol(), true);
        }
    }
}
