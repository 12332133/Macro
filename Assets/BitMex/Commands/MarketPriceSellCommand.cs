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
            //parameter 0 => 퍼센트(100->)
            initializer(this.Parameters);
        }

        public MarketPriceSellCommand(IBitMexMainAdapter bitmexMain, BitMexCommandType commandType, List<object> paramters)
            : base(bitmexMain, commandType, paramters)
        {
        }
      
        protected override MarketPriceSellCommand Create()
        {
            return new MarketPriceSellCommand(BitMexMain, CommandType, Parameters);
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
                BitMexMain.DriverService.HandleSell();
            }
        }

        public override string GetCommandText()
        {
            return string.Format("시장가 {0}% 매도", Parameters[0]);
        }
    }
}
