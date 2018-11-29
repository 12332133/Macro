using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public interface IBitMexCommand
    {
        IBitMexMainAdapter BitMexMain { get; set; }
        BitMexCommandTableType CommandTableType { get; set; }
        BitMexCommandType CommandType { get; set; }
        int RefCommandTableIndex { get; set; }
        List<object> Parameters { get; set; }

        IBitMexCommand Clone();
        void Execute();
        string GetCommandText();
    }

    public abstract class BitMexCommand : IBitMexCommand
    {
        public IBitMexMainAdapter BitMexMain { get; set; }
        public BitMexCommandTableType CommandTableType { get; set; }
        public BitMexCommandType CommandType { get; set; }
        public int RefCommandTableIndex { get; set; }
        public List<object> Parameters { get; set; }

        public BitMexCommand(IBitMexMainAdapter bitmexMain)
        {
            BitMexMain = bitmexMain;
            Parameters = new List<object>();
        }

        public BitMexCommand(IBitMexCommand command)
        {
            BitMexMain = command.BitMexMain;
            CommandTableType = command.CommandTableType;
            CommandType = command.CommandType;
            RefCommandTableIndex = command.RefCommandTableIndex;
            Parameters = new List<object>(command.Parameters);
        }

        public abstract IBitMexCommand Clone();
        public abstract string GetCommandText();
        public abstract void Execute();
    }

    public abstract class BitMexCommand<T> : BitMexCommand where T : IBitMexCommand
    {
        public BitMexCommand(IBitMexMainAdapter bitmexMain) 
            : base(bitmexMain)
        {
        }

        public BitMexCommand(IBitMexCommand command) : base(command)
        {
        }

        public override IBitMexCommand Clone()
        {
            return Create();
        }

        protected abstract T Create();
    }
}
