﻿using Assets.BitMex.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex
{
    public class BitMexCommandRepository
    {
        private Dictionary<BitMexCommandType, IBitMexCommand> commands;

        public BitMexCommandRepository()
        {
            this.commands = new Dictionary<BitMexCommandType, IBitMexCommand>();
        }

        public void Resister(BitMexCommandType type, IBitMexCommand command)
        {
            this.commands.Add(type, command);
        }

        public IBitMexCommand CreateCommand(BitMexCommandType type)
        {
            var command = this.commands[type];
            return command.Clone() as IBitMexCommand;
        }

        public Dictionary<BitMexCommandType, IBitMexCommand> GetCommands()
        {
            return this.commands;
        }
    }
}
