using Bitmex.Net;
using Bitmex.Net.Model.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketPriceSpecifiedQuantitySellCommand : BitMexCommand<MarketPriceSpecifiedQuantitySellCommand>
    {
        public MarketPriceSpecifiedQuantitySellCommand(IBitMexMainAdapter bitmexMain, Action<List<object>> initializer) 
            : base(bitmexMain)
        {
            initializer(Parameters);
        }

        public MarketPriceSpecifiedQuantitySellCommand(IBitMexCommand command) : base(command)
        {
        }

        protected override MarketPriceSpecifiedQuantitySellCommand Create()
        {
            return new MarketPriceSpecifiedQuantitySellCommand(this);
        }

        /// <summary>
        /// Parameter[0] : 매수/매도 수량
        /// Parameter[1] : 거래 코인
        /// </summary>
        public override void Execute()
        {
            var quantity = Convert.ToInt32(Parameters[0]);
            var symbol = (string)Parameters[1];

            var dto = this.BitMexMain.ApiService.Execute(BitmexApiActionAttributes.Order.PostOrder,
                OrderPOSTRequestParams.CreateSimpleMarket(
                    symbol,
                    quantity,
                    OrderSide.Sell));
        }

        public override string GetCommandText()
        {
            return string.Format("시장가 {0}개 매도", Parameters[0].ToString());
        }
    }
}
