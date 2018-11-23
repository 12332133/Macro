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

        public ActivateOrderCancleCommand(IBitMexMainAdapter bitmexMain, BitMexCommandType commandType, List<object> paramters)
            : base(bitmexMain, commandType, paramters)
        {
        }

        protected override ActivateOrderCancleCommand Create()
        {
            return new ActivateOrderCancleCommand(BitMexMain, CommandType, Parameters);
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
