using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class NoneCommand : BitMexCommand<OderCommandCreator>
    {
        public NoneCommand(IBitMexMainAdapter bitmexMain) : base(bitmexMain)
        {
        }

        public NoneCommand(IBitMexCommand command) : base(command)
        {
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }

        public override string GetCommandText()
        {
            return string.Empty;
        }

        protected override OderCommandCreator Create()
        {
            throw new NotImplementedException();
        }
    }
}
