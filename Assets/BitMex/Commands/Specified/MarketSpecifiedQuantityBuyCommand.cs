using Bitmex.Net;
using Bitmex.Net.Model.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketSpecifiedQuantityBuyCommand : BitMexCommand<MarketSpecifiedQuantityBuyCommand>
    {
        public MarketSpecifiedQuantityBuyCommand(IBitMexMainAdapter bitmexMain, Action<List<object>> initializer) 
            : base(bitmexMain)
        {
            initializer(Parameters);
        }

        public MarketSpecifiedQuantityBuyCommand(IBitMexCommand command) : base(command)
        {
        }

        protected override MarketSpecifiedQuantityBuyCommand Create()
        {
            return new MarketSpecifiedQuantityBuyCommand(this);
        }

        /// <summary>
        /// Parameter[0] : 매수/매도 수량
        /// Parameter[1] : 거래 코인
        /// </summary>
        public override void Execute()
        {
            var quantity = Convert.ToInt32(Parameters[0]);
            var symbol = (string)Parameters[1];
            var price = BitMexMain.Session.Trades[symbol].Price;

            var dto = this.BitMexMain.ApiService.Execute(BitmexApiActionAttributes.Order.PostOrder,
                OrderPOSTRequestParams.CreateSimpleLimit(
                    symbol,
                    quantity,
                    price,
                    OrderSide.Buy));
        }

        public override string GetCommandText()
        {
            return string.Format("빠른 지정가 {0}개 매수", Parameters[0].ToString());
        }
    }
}
