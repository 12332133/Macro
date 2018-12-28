﻿using Bitmex.Net;
using Bitmex.Net.Model.Param;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketSpecifiedBuyCommand : OrderCommand<MarketSpecifiedBuyCommand>
    {
        public MarketSpecifiedBuyCommand(IBitMexMainAdapter bitmexMain, Action<List<object>> initializer) 
            : base(bitmexMain)
        {
            initializer(this.Parameters);
        }

        public MarketSpecifiedBuyCommand(IBitMexCommand command) : base(command)
        {
        }

        protected override MarketSpecifiedBuyCommand Create()
        {
            return new MarketSpecifiedBuyCommand(this);
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
                OrderPOSTRequestParams.CreateSimpleLimit(
                    symbol,
                    quantity,
                    price,
                    OrderSide.Buy));
        }

        public override string GetCommandText()
        {
            return string.Format("빠른 지정가 {0}% 매수", Parameters[0].ToString());
        }
    }

    public class MarketSpecifiedBuyCustomCommand : MarketSpecifiedBuyCommand
    {
        public MarketSpecifiedBuyCustomCommand(IBitMexMainAdapter bitmexMain, Action<List<object>> initializer) 
            : base(bitmexMain, initializer)
        {
        }

        public override string GetCommandText()
        {
            return string.Format("빠른 지정가 % 설정", Parameters[0].ToString());
        }
    }
}
