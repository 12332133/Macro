using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class FixedAvailableXbtCommand : BitMexCommand
    {
        //public decimal FixedAvailableXbt { get; set; }

        public FixedAvailableXbtCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose)
            : base(bitmexMain, contentString, isExpose)
        {
        }

        public override object Clone()
        {
            return new FixedAvailableXbtCommand(BitMexMain, ContentString, IsExpose);
        }

        public override void Execute()
        {
            //BitMexMain.Session.FixedAvailableXbt = FixedAvailableXbt;
        }
    }
}
