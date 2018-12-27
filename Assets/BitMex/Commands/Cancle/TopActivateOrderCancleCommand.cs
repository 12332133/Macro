using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitmex.Net;
using Bitmex.Net.Model.Param;

namespace Assets.BitMex.Commands
{
    public class TopActivateOrderCancleCommand : BitMexCommand<TopActivateOrderCancleCommand>
    {
        public TopActivateOrderCancleCommand(IBitMexMainAdapter bitmexMain) 
            : base(bitmexMain)
        {
        }

        public TopActivateOrderCancleCommand(IBitMexCommand command) : base(command)
        {
        }

        protected override TopActivateOrderCancleCommand Create()
        {
            return new TopActivateOrderCancleCommand(this);
        }

        /// <summary>
        /// Parameter[0] : 거래 코인
        /// </summary>
        public override void Execute()
        {
            var symbol = (string)Parameters[1];
            var dtos = this.BitMexMain.ApiService.Execute(BitmexApiActionAttributes.Order.DeleteOrderAll,
                new OrderAllDELETERequestParams() { Symbol = symbol });
        }

        public override string GetCommandText()
        {
            return "현재 코인 주문 취소";
        }
    }
}
