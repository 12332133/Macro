using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class SampleCommand : BitMexCommand
    {
        public SampleCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose)
            : base(bitmexMain, contentString, isExpose)
        {
        }

        public override object Clone()
        {
            return new SampleCommand(BitMexMain, ContentString, IsExpose);
        }

        public override void Execute()
        {
        }
    }
}
