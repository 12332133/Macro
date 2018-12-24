using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public abstract class OrderCommand<T> : BitMexCommand<T> where T : IBitMexCommand
    {
        public OrderCommand(IBitMexMainAdapter bitmexMain)
            : base(bitmexMain)
        {
        }

        public OrderCommand(IBitMexCommand command) : base(command)
        {
        }

        public decimal CalculationQuantity(string symbol, int percent, decimal price)
        {
            var position = BitMexMain.Session.Positions[symbol];
            decimal quantity = 0;

            if (position.CurrentQty.HasValue == true && position.CurrentQty.Value > 0)
            {
                quantity = Math.Floor(position.CurrentQty.Value * ((decimal)percent / 100));
            }
            else
            {
                var xbt = BitMexMain.CoinTable.GetCoin(symbol).FixedAvailableXbt;
                // 고정 xbt
                if (xbt == 0)
                {
                    xbt = BitMexMain.Session.AvailableXbt;
                }

                // 교차 선택
                var leverage = BitMexMain.Session.Leverage;

                if (symbol.Equals(BitMexDriverService.MainSymbol) == true) // 비트코인만 다르게 사용
                {
                    quantity = Math.Floor(xbt * leverage * price * ((decimal)percent / 100));
                }
                else
                {
                    quantity = Math.Floor(xbt * leverage * ((decimal)percent / 100) / price);
                }
            }

            return quantity;
        }
    }
}
