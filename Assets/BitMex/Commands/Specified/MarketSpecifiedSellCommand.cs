using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketSpecifiedSellCommand : BitMexCommand<MarketSpecifiedSellCommand>
    {
        //public int Magnification { get; set; }

        public MarketSpecifiedSellCommand(IBitMexMainAdapter bitmexMain, Action<List<object>> initializer) 
            : base(bitmexMain)
        {
            initializer(this.Parameters);
        }

        public MarketSpecifiedSellCommand()
        {
        }

        protected override MarketSpecifiedSellCommand Create()
        {
            return new MarketSpecifiedSellCommand()
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
                coin.SpecifiedAditional,
                coin.FixedAvailableXbt,
                currentCoinName) == true)
            {
                BitMexMain.DriverService.HandleSell();
            }
        }

        public override string GetCommandText()
        {
            return string.Format("빠른 지정가 {0}% 매도", Parameters[0].ToString());
        }
    }
}
