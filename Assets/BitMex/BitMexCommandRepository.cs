using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex
{
    public class BitMexCommandRepository
    {
        private Dictionary<BitMexCommandType, IBitMexActionCommand> commands;

        public BitMexCommandRepository()
        {
            this.commands = new Dictionary<BitMexCommandType, IBitMexActionCommand>();
        }

        public void Resister(BitMexCommandType type, IBitMexActionCommand command)
        {
            this.commands.Add(type, command);
        }

        public IBitMexActionCommand CreateCommand(BitMexCommandType type)
        {
            var command = this.commands[type];
            //copy?
            return command;
        }

        public Dictionary<BitMexCommandType, IBitMexActionCommand> GetCommands()
        {
            return this.commands;
        }
    }
}
