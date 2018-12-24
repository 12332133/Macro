using Bitmex.Net;
using Bitmex.Net.Model.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketSpecifiedSellCommand : OrderCommand<MarketSpecifiedSellCommand>
    {
        public MarketSpecifiedSellCommand(IBitMexMainAdapter bitmexMain, Action<List<object>> initializer) 
            : base(bitmexMain)
        {
            initializer(this.Parameters);
        }

        public MarketSpecifiedSellCommand(IBitMexCommand command) : base(command)
        {
        }

        protected override MarketSpecifiedSellCommand Create()
        {
            return new MarketSpecifiedSellCommand(this);
        }

        /// <summary>
        /// Parameter[0] : 거래 퍼센트
        /// Parameter[1] : 거래 코인
        /// </summary>
        public override void Execute()
        {
            var percent = Convert.ToInt32(Parameters[0]);
            var symbol = (string)Parameters[1];
            var price = BitMexMain.Session.Trades[symbol].Price;
            var quantity = CalculationQuantity(symbol, percent, price);

            var dto = this.BitMexMain.ApiService.Execute(BitmexApiActionAttributes.Order.PostOrder,
                OrderPOSTRequestParams.CreateSimpleLimit(
                    symbol,
                    quantity,
                    price,
                    OrderSide.Sell));
        }

        public override string GetCommandText()
        {
            return string.Format("빠른 지정가 {0}% 매도", Parameters[0].ToString());
        }
    }
}
