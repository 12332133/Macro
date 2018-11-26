using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class ActivateOrderCancleCommand : BitMexCommand<ActivateOrderCancleCommand>
    {
        public ActivateOrderCancleCommand(IBitMexMainAdapter bitmexMain) 
            : base(bitmexMain)
        {
        }

        public ActivateOrderCancleCommand()
        {
        }

        protected override ActivateOrderCancleCommand Create()
        {
            return new ActivateOrderCancleCommand()
            {
                BitMexMain = this.BitMexMain,
                CommandType = this.CommandType,
                Parameters = new List<object>(this.Parameters)
            };
        }

        public override void Execute()
        {
            BitMexMain.DriverService.HandleCancleActivatedOrders(BitMexMain.DriverService.HandleGetCurrentSymbol(), true);
        }

        public override string GetCommandText()
        {
            return "전체 주문 취소";
        }
    }
}
