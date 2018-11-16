using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class PositionClearCommand : BitMexCommand
    {
        public PositionClearCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose)
            : base(bitmexMain, contentString, isExpose)
        {
        }

        public override object Clone()
        {
            return new PositionClearCommand(BitMexMain, ContentString, IsExpose);
        }

        public override void Execute()
        {
            var symbol = BitMexMain.DriverService.HandleGetCurrentSymbol();
            BitMexMain.DriverService.HandleClearPosition(symbol);
        }
    }
}
