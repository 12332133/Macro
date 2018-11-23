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

        public SampleCommand(IBitMexMainAdapter bitmexMain, BitMexCommandType commandType, List<object> parameters) 
            : base(bitmexMain, commandType, parameters)
        {
        }

        protected override SampleCommand Create()
        {
            return new SampleCommand(BitMexMain, CommandType, Parameters);
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
