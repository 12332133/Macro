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

        public PositionClearCommand(IBitMexMainAdapter bitmexMain, BitMexCommandType commandType, List<object> parameters) 
            : base(bitmexMain, commandType, parameters)
        {
        }

        protected override PositionClearCommand Create()
        {
            return new PositionClearCommand(BitMexMain, CommandType, Parameters);
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
