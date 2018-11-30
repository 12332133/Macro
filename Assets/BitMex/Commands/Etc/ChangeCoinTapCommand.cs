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

        public ChangeCoinTapCommand(IBitMexCommand command) : base(command)
        {
        }

        protected override ChangeCoinTapCommand Create()
        {
            return new ChangeCoinTapCommand(this);
        }

        /// <summary>
        /// Parameter[0] : 변경 할 대표 코인 이름
        /// </summary>
        public override void Execute()
        {
            var coin = this.BitMexMain.DriverService.CoinTable.GetCoin(Parameters[0].ToString());

            //if (this.BitMexMain.DriverService.HandleGetCurrentSymbol().Equals(coin.CoinName) == true)
            //{
            //    return;
            //}

            this.BitMexMain.DriverService.HandleChangeCoinTab(coin.RootCoinName, coin.CoinName);
        }

        public override string GetCommandText()
        {
            return string.Format("{0} 이동", Parameters[0].ToString());
        }
    }
}
