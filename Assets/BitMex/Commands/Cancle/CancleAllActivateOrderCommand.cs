using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitmex.Net;
using Bitmex.Net.Model.Param;

namespace Assets.BitMex.Commands
{
    public class ActivateOrderCancleCommand : BitMexCommand<ActivateOrderCancleCommand>
    {
        public ActivateOrderCancleCommand(IBitMexMainAdapter bitmexMain) 
            : base(bitmexMain)
        {
        }

        public ActivateOrderCancleCommand(IBitMexCommand command) : base(command)
        {
        }

        protected override ActivateOrderCancleCommand Create()
        {
            return new ActivateOrderCancleCommand(this);
        }

        public override void Execute()
        {
            var dtos = this.BitMexMain.ApiService.Execute(BitmexApiActionAttributes.Order.DeleteOrderAll, new OrderAllDELETERequestParams());
        }

        public override string GetCommandText()
        {
            return "전체 주문 취소";
        }
    }
}
