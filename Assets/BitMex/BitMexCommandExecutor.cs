using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.BitMex
{
    public class BitMexCommandExecutor
    {
        private Thread thread;
        private BlockingCollection<IBitMexActionCommand> commands;

        public BitMexCommandExecutor()
        {
            this.commands = new BlockingCollection<IBitMexActionCommand>();

            this.thread = new Thread(DoWork);
            this.thread.IsBackground = true;
            this.thread.Start();
        }

        private void DoWork()
        {
            while (true)
            {
                var command = this.commands.Take();
                command.Execute();
            }
        }

        public bool AddCommand(IBitMexActionCommand command)
        {
            return this.commands.TryAdd(command, 100);
        }
    }
}
