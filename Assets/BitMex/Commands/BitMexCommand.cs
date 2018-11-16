using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.BitMex.Commands
{
    public interface IBitMexCommand
    {
        IBitMexMainAdapter BitMexMain { get; }
        string ContentString { get; }
        bool IsExpose { get; }
        void Execute();
        List<int> Parameters { get; }
        object Clone();
    }

    public abstract class BitMexCommand : IBitMexCommand
    {
        public IBitMexMainAdapter BitMexMain { get; private set; }
        public string ContentString { get; private set; }
        public bool IsExpose { get; private set; }
        public List<int> Parameters { get; private set; }

        public BitMexCommand(IBitMexMainAdapter bitmexMain, string contentString, bool isExpose)
        {
            Parameters = new List<int> { 0, 0 };
            BitMexMain = bitmexMain;
            ContentString = contentString;
            IsExpose = isExpose;
        }

        public abstract object Clone();
        public abstract void Execute();
    }
}
