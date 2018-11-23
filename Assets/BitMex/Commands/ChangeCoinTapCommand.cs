using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class ChangeCoinTapCommand : BitMexCommand<ChangeCoinTapCommand>
    {
        public ChangeCoinTapCommand(IBitMexMainAdapter bitmexMain, Action<List<object>> initializer) 
            : base(bitmexMain)
        {
            initializer(Parameters);
        }

        public ChangeCoinTapCommand(IBitMexMainAdapter bitmexMain, BitMexCommandType commandType, List<object> paramters)
            : base(bitmexMain, commandType, paramters)
        {
        }

        protected override ChangeCoinTapCommand Create()
        {
            return new ChangeCoinTapCommand(BitMexMain, CommandType, Parameters);
        }

        public override void Execute()
        {
            var coin = this.BitMexMain.DriverService.CoinTable.GetCoin((string)Parameters[1]);
            this.BitMexMain.DriverService.HandleChangeCoinTab((string)Parameters[0], (string)Parameters[1]);
        }

        public override string GetCommandText()
        {
            return string.Format("코인({0}) 탭 이동", Parameters[0]);
        }
    }
}
