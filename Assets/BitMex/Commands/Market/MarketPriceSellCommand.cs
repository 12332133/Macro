using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketPriceSellCommand : BitMexCommand<MarketPriceSellCommand>
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
        /// </summary>
        public override void Execute()
        {
            var coinName = BitMexMain.DriverService.HandleGetCurrentSymbol();
            var coin = BitMexMain.DriverService.CoinTable.GetCoin(coinName);

            if (BitMexMain.DriverService.HandleOrderMarketQty(
                0,
                (int)Parameters[0],
                coin.FixedAvailableXbt,
                coinName
                ) == true)
            {
                BitMexMain.DriverService.HandleSell();
            }
        }

        public override string GetCommandText()
        {
            return string.Format("시장가 {0}% 매도", Parameters[0].ToString());
        }
    }
}
