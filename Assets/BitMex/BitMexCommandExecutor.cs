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
        //private BlockingCollection<IBitMexActionCommand> commands;
        private ConcurrentQueue<IBitMexActionCommand> commands;

        public BitMexCommandExecutor()
        {
            this.commands = new ConcurrentQueue<IBitMexActionCommand>();

            this.thread = new Thread(DoWork);
            this.thread.IsBackground = true;
            this.thread.Start();
        }

        private void DoWork()
        {
            IBitMexActionCommand command = null;

            while (true)
            {
                try
                {
                    if (this.commands.TryDequeue(out command) == true)
                    {
                        command.Execute();
                    }
                    Thread.Sleep(5);
                }
                catch (BitMexDriverServiceException exception)
                {
                    Debug.Log(exception.ToString());
                }
            }
        }

        public bool AddCommand(IBitMexActionCommand command)
        {
            this.commands.Enqueue(command);
            return true;
            //return this.commands.TryAdd(command, 100);
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
