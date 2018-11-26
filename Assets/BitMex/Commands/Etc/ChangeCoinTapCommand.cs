using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class ChangeCoinTapCommand : BitMexCommand<ChangeCoinTapCommand>
    {
        public ChangeCoinTapCommand(IBitMexMainAdapter bitmexMain) 
            : base(bitmexMain)
        {
        }

        public ChangeCoinTapCommand()
        {
        }

        protected override ChangeCoinTapCommand Create()
        {
            return new ChangeCoinTapCommand()
            {
                BitMexMain = this.BitMexMain,
                CommandType = this.CommandType,
                Parameters = new List<object>(this.Parameters)
            };
        }

        /// <summary>
        /// Parameter[0] : 변경 할 대표 코인 이름
        /// Parameter[1] : 변경 할 코인 이름
        /// </summary>
        public override void Execute()
        {
            var coin = this.BitMexMain.DriverService.CoinTable.GetCoin((string)Parameters[1]);
            this.BitMexMain.DriverService.HandleChangeCoinTab((string)Parameters[0], (string)Parameters[1]);
        }

        public override string GetCommandText()
        {
            return string.Format("거래 코인 이동");
        }
    }
}
