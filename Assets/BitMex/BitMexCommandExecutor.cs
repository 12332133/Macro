using Assets.BitMex.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.BitMex
{
    public class BitMexCommandExecutor
    {
        private Thread thread;
        private BlockingCollection<IBitMexCommand> commands;
        //private ConcurrentQueue<IBitMexCommand> commands;

        public BitMexCommandExecutor()
        {
            this.commands = new BlockingCollection<IBitMexCommand>();

            this.thread = new Thread(DoWork);
            this.thread.IsBackground = true;
            this.thread.Start();
        }

        private void DoWork()
        {
            //IBitMexCommand command = null;

            while (true)
            {
                try
                {
                    //if (this.commands.TryDequeue(out command) == true)
                    //{
                    //    command.Execute();
                    //}

                    var command = this.commands.Take();
                    command.Execute();

                    //Thread.Sleep(5);
                }
                catch (BitMexDriverServiceException exception)
                {
                }
            }
        }

        public bool AddCommand(IBitMexCommand command, int timeOut = 100)
        {
            //this.commands.Enqueue(command);
            //return true;
            return this.commands.TryAdd(command, timeOut);
        }

        public void Stop()
        {
            if (this.thread != null)
            {
                this.thread.Abort();
            }
        }
    }
}
