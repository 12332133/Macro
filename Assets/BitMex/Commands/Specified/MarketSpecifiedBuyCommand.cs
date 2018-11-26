using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketSpecifiedBuyCommand : BitMexCommand<MarketSpecifiedBuyCommand>
    {
        public MarketSpecifiedBuyCommand(IBitMexMainAdapter bitmexMain, Action<List<object>> initializer) 
            : base(bitmexMain)
        {
            initializer(this.Parameters);
        }

        public MarketSpecifiedBuyCommand()
        {
        }

        protected override MarketSpecifiedBuyCommand Create()
        {
            return new MarketSpecifiedBuyCommand()
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

            if (BitMexMain.DriverService.HandleOrderSpecifiedQty(
                0,
                (int)Parameters[0],
                -coin.SpecifiedAditional,
                coin.FixedAvailableXbt,
                currentCoinName
                ) == true)
            {
                BitMexMain.DriverService.HandleBuy();
            }
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
