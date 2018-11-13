using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class SpecifiedAditionalCommand : BitMexCommand
    {
        public decimal SpecifiedAditional { get; set; }

        public SpecifiedAditionalCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose)
            : base(bitmexMain, contentString, isExpose)
        {
        }

        public override void Execute()
        {
            //BitMexMain.Session.SpecifiedAditional = SpecifiedAditional;
        }
    }
}
