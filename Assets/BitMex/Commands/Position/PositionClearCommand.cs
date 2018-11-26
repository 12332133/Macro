using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class PositionClearCommand : BitMexCommand<PositionClearCommand>
    {
        public PositionClearCommand(IBitMexMainAdapter bitmexMain) 
            : base(bitmexMain)
        {
        }

        public PositionClearCommand()
        {
        }

        protected override PositionClearCommand Create()
        {
            return new PositionClearCommand()
            {
                BitMexMain = this.BitMexMain,
                CommandType = this.CommandType,
                Parameters = new List<object>(this.Parameters)
            };
        }

        public override void Execute()
        {
            BitMexMain.DriverService.HandleClearPosition(BitMexMain.DriverService.HandleGetCurrentSymbol());
        }

        public override string GetCommandText()
        {
            return "해당 포지션 청산";
        }
    }
}
