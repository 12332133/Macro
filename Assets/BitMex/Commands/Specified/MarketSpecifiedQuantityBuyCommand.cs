using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketSpecifiedQuantityBuyCommand : BitMexCommand<MarketSpecifiedQuantityBuyCommand>
    {
        public MarketSpecifiedQuantityBuyCommand(IBitMexMainAdapter bitmexMain, Action<List<object>> initializer) 
            : base(bitmexMain)
        {
        }

        public MarketSpecifiedQuantityBuyCommand()
        {
        }

        protected override MarketSpecifiedQuantityBuyCommand Create()
        {
            return new MarketSpecifiedQuantityBuyCommand()
            {
                BitMexMain = this.BitMexMain,
                CommandType = this.CommandType,
                Parameters = new List<object>(this.Parameters)
            };
        }

        /// <summary>
        /// Parameter[0] : 매수/매도 수량
        /// </summary>
        public override void Execute()
        {
            if (BitMexMain.DriverService.HandleOrderSpecifiedQty(
                (decimal)Parameters[0],
                0,
                0,
                0,
                BitMexMain.DriverService.HandleGetCurrentSymbol()
                ) == true)
            {
                BitMexMain.DriverService.HandleBuy();
            }
        }

        public override string GetCommandText()
        {
            return string.Format("빠른 지정가 {0}개 매수", Parameters[0].ToString());
        }
    }
}
