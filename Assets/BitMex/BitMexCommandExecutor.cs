using Assets.BitMex.Commands;
using Bitmex.Net;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.BitMex
{
    public class BitMexCommandExecutor
    {
        private Thread thread;
        private BlockingCollection<IBitMexCommand> commands;
        private readonly string dir = Resource.Dir + "errors.txt";

        public BitMexCommandExecutor()
        {
            this.commands = new BlockingCollection<IBitMexCommand>();

            this.thread = new Thread(DoWork);
            this.thread.IsBackground = true;
            this.thread.Start();
        }

        private void DoWork()
        {
            while (true)
            {
                var command = this.commands.Take();

                try
                {
                    command.Execute();
                }
                catch (ThreadAbortException)
                {
                }
                catch (BitmexApiOverloadException e)
                {
                    Task.Run(() => {
                        File.AppendAllLines(this.dir,
                            new[] { string.Format("[{0}] Message : {1} StackTrace : {2}",
                            DateTime.Now.ToString(),
                            e.Message,
                            e.StackTrace)});
                    });
                }
                catch (BitmexApiException e)
                {
                    Task.Run(() => {
                        File.AppendAllLines(this.dir,
                            new[] { string.Format("[{0}] Message : {1} StackTrace : {2}",
                            DateTime.Now.ToString(),
                            e.Message,
                            e.StackTrace)});
                    });
                }
                catch (Exception e)
                {
                    Task.Run(() => {
                        File.AppendAllLines(this.dir,
                            new[] { string.Format("[{0}] Message : {1} StackTrace : {2}",
                            DateTime.Now.ToString(),
                            e.Message,
                            e.StackTrace)});
                    });
                }
            }
        }

        public bool AddCommand(IBitMexCommand command, int timeOut = 100)
        {
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
