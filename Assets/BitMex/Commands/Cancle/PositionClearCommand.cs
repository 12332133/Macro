using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitmex.Net;
using Bitmex.Net.Model.Param;

namespace Assets.BitMex.Commands
{
    public class PositionClearCommand : BitMexCommand<PositionClearCommand>
    {
        public PositionClearCommand(IBitMexMainAdapter bitmexMain) 
            : base(bitmexMain)
        {
        }

        public PositionClearCommand(IBitMexCommand command) : base(command)
        {
        }

        protected override PositionClearCommand Create()
        {
            return new PositionClearCommand(this);
        }

        /// <summary>
        /// Parameter[0] : 거래 코인
        /// </summary>
        public override void Execute()
        {
            var symbol = (string)Parameters[0];

            var dto = this.BitMexMain.ApiService.Execute(BitmexApiActionAttributes.Order.PostOrder,
                OrderPOSTRequestParams.ClosePositionByMarket(symbol));
        }

        public override string GetCommandText()
        {
            return "해당 포지션 청산";
        }
    }
}
