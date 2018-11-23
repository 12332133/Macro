using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketPriceBuyCommand : BitMexCommand<MarketPriceBuyCommand>
    {
        public MarketPriceBuyCommand(IBitMexMainAdapter bitmexMain, Action<List<object>> initializer) 
            : base(bitmexMain)
        {
            //parameter 0 => 퍼센트(100->)
            initializer(this.Parameters);
        }

        public MarketPriceBuyCommand(IBitMexMainAdapter bitmexMain, BitMexCommandType commandType, List<object> paramters)
            : base(bitmexMain, commandType, paramters)
        {
        }

        protected override MarketPriceBuyCommand Create()
        {
            return new MarketPriceBuyCommand(BitMexMain, CommandType, Parameters);
        }

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
                BitMexMain.DriverService.HandleBuy();
            }
        }

        public override string GetCommandText()
        {
            return string.Format("시장가 {0}% 매수", Parameters[0]);
        }
    }
}
