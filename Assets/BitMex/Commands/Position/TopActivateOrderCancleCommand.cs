using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class TopActivateOrderCancleCommand : BitMexCommand<TopActivateOrderCancleCommand>
    {
        public TopActivateOrderCancleCommand(IBitMexMainAdapter bitmexMain) 
            : base(bitmexMain)
        {
        }

        public TopActivateOrderCancleCommand()
        {
        }

        protected override TopActivateOrderCancleCommand Create()
        {
            return new TopActivateOrderCancleCommand()
            {
                BitMexMain = this.BitMexMain,
                CommandType = this.CommandType,
                Parameters = new List<object>(this.Parameters)
            };
        }

        public override void Execute()
        {
            BitMexMain.DriverService.HandleCancleActivatedOrders(
                BitMexMain.DriverService.HandleGetCurrentSymbol(), false);
        }

        public override string GetCommandText()
        {
            return "최상위 주문 취소";
        }
    }
}
