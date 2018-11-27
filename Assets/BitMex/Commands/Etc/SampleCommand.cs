using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class SampleCommand : BitMexCommand<SampleCommand>
    {
        public SampleCommand(IBitMexMainAdapter bitmexMain) 
            : base(bitmexMain)
        {
        }

        public SampleCommand(IBitMexCommand command) : base(command)
        {
        }

        protected override SampleCommand Create()
        {
            return new SampleCommand(this);
        }

        public override void Execute()
        {
        }

        public override string GetCommandText()
        {
            return "sample command";
        }
    }
}
