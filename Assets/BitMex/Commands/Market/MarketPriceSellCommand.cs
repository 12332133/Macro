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

        public MarketPriceSellCommand()
        {
        }

        protected override MarketPriceSellCommand Create()
        {
            return new MarketPriceSellCommand()
            {
                BitMexMain = this.BitMexMain,
                CommandType = this.CommandType,
                Parameters = new List<object>(this.Parameters)
            };
        }

        /// <summary>
        /// Parameter[0] : 거래 퍼센트
        /// </summary>
        public override void Execute()
        {
            var currentCoinName = BitMexMain.DriverService.HandleGetCurrentSymbol();
            var coin = BitMexMain.DriverService.CoinTable.GetCoin(currentCoinName);

            if (BitMexMain.DriverService.HandleOrderMarketQty(
                0,
                (int)Parameters[0],
                coin.FixedAvailableXbt,
                currentCoinName
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
