using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public class OderCommandCreator : BitMexCommand<OderCommandCreator>
    {
        public OderCommandCreator(IBitMexMainAdapter bitmexMain, string commandText) : base(bitmexMain)
        {
            Parameters.Add(commandText);
        }

        public override void Execute()
        {
        }

        public override string GetCommandText()
        {
            return Parameters[0].ToString();
        }

        protected override OderCommandCreator Create()
        {
            throw new NotImplementedException();
        }
    }
}
