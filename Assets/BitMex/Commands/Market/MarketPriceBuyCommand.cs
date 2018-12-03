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
            initializer(this.Parameters);
        }

        public MarketPriceBuyCommand(IBitMexCommand command) : base(command)
        {
        }

        protected override MarketPriceBuyCommand Create()
        {
            return new MarketPriceBuyCommand(this);
        }

        /// <summary>
        /// Parameter[0] : 거래 퍼센트
        /// Parameter[1] : 거래 지정 대표 코인 이름 (탭을 강재로 변경 해줘야함)
        /// Parameter[2] : 거래 지정 코인 이름 (탭을 강재로 변경 해줘야함)
        /// </summary>
        public override void Execute()
        {
            if (Parameters.Count > 1)
            {
                BitMexMain.DriverService.HandleChangeCoinTab(Parameters[1].ToString(), Parameters[2].ToString());
            }

            var coinName = BitMexMain.DriverService.HandleGetCurrentSymbol();
            var coin = BitMexMain.DriverService.CoinTable.GetCoin(coinName);

            if (BitMexMain.DriverService.HandleOrderMarketQty(
                0,
                (int)Parameters[0],
                coin.FixedAvailableXbt,
                coinName
                ) == true)
            {
                BitMexMain.DriverService.HandleBuy();
            }
        }

        public override string GetCommandText()
        {
            return string.Format("시장가 {0}% 매수", Parameters[0].ToString());
        }
    }
}
