using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketPriceSpecifiedQuantityBuyCommand : BitMexCommand<MarketPriceSpecifiedQuantityBuyCommand>
    {
        public MarketPriceSpecifiedQuantityBuyCommand(IBitMexMainAdapter bitmexMain, Action<List<object>> initializer) 
            : base(bitmexMain)
        {
            initializer(Parameters);
        }

        public MarketPriceSpecifiedQuantityBuyCommand(IBitMexCommand command) : base(command)
        {
        }

        protected override MarketPriceSpecifiedQuantityBuyCommand Create()
        {
            return new MarketPriceSpecifiedQuantityBuyCommand(this);
        }

        /// <summary>
        /// Parameter[0] : 매수/매도 수량
        /// </summary>
        public override void Execute()
        {
            if (BitMexMain.DriverService.HandleOrderMarketQty(
                (decimal)Parameters[0], 
                0, 
                0, 
                BitMexMain.DriverService.HandleGetCurrentSymbol()) == true)
            {
                BitMexMain.DriverService.HandleBuy();
            }
        }

        public override string GetCommandText()
        {
            return string.Format("시장가 {0}개 매수", Parameters[0].ToString());
        }
    }
}
