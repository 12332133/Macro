using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class MarketSpecifiedQuantitySellCommand : BitMexCommand<MarketSpecifiedQuantitySellCommand>
    {
        public MarketSpecifiedQuantitySellCommand(IBitMexMainAdapter bitmexMain, Action<List<object>> initializer) 
            : base(bitmexMain)
        {
            initializer(Parameters);
        }

        public MarketSpecifiedQuantitySellCommand(IBitMexCommand command) : base(command)
        {
        }

        protected override MarketSpecifiedQuantitySellCommand Create()
        {
            return new MarketSpecifiedQuantitySellCommand(this);
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
                BitMexMain.DriverService.HandleSell();
            }
        }

        public override string GetCommandText()
        {
            return string.Format("빠른 지정가 {0}개 매도", Parameters[0].ToString());
        }
    }
}
