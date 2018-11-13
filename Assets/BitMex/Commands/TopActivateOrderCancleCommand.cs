using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class TopActivateOrderCancleCommand : BitMexCommand
    {
        public TopActivateOrderCancleCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose)
            : base(bitmexMain, contentString, isExpose)
        {
        }

        public override void Execute()
        {
            BitMexMain.DriverService.HandleCancleActivatedOrders(
                BitMexMain.DriverService.HandleGetCurrentSymbol(), false);
        }
    }
}
