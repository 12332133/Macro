using Bitmex.Net;
using Bitmex.Net.Model.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketPriceSellCommand : OrderCommand<MarketPriceSellCommand>
    {
        public MarketPriceSellCommand(IBitMexMainAdapter bitmexMain, Action<List<object>> initializer) 
            : base(bitmexMain)
        {
            initializer(this.Parameters);
        }

        public MarketPriceSellCommand(IBitMexCommand command) : base(command)
        {
        }

        protected override MarketPriceSellCommand Create()
        {
            return new MarketPriceSellCommand(this);
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
            if (quantity <= 0)
                return;

            var dto = this.BitMexMain.ApiService.Execute(BitmexApiActionAttributes.Order.PostOrder,
                OrderPOSTRequestParams.CreateSimpleMarket(
                    symbol,
                    quantity,
                    OrderSide.Sell));
        }

        public override string GetCommandText()
        {
            return string.Format("시장가 {0}% 매도", Parameters[0].ToString());
        }
    }
}
