using System;
using System.Threading;
using System.Text;
namespace Controller
{
    class Program
    {   
        public static bool _RUNNING = true;
        public static JobQueue  MainJobQueue;
        
        static void Main(string[] args)
        {
            MainJobQueue = new JobQueue();
            Logger lgr = new Logger();
            // start waiting for Threads.
            WorkerManager wmt = new WorkerManager();
            Thread wmtThread = new Thread(new ThreadStart(wmt.AddWorkerThread));
            wmtThread.IsBackground = true;  // this thread must end when program hits end
                                            // there will be no joining
            wmtThread.Start();
            
            // wait until there are workers
            while (wmt.Workers.Count <= 0)
            {
                Thread.Sleep(500);
            }

            //Start the queue worker thread.
            JobQueueWatcher jqe = new JobQueueWatcher(MainJobQueue,wmt.Workers);
            Thread jqeThread = new Thread(new ThreadStart(jqe.WatchQueue));
            jqeThread.Start();
            PoolManager pm = new PoolManager(MainJobQueue);
            if (pm.startConnectionToPool())
            {
                /*
                    Need to build thread for listner to add jobs to the Queue
                */
                Thread PoolTh = new Thread(new ThreadStart(pm.StartListner));
                PoolTh.Start();
            }
            else
            {
                Console.Write("Failed to connect to Server Exiting.......");
                lgr.LogMessage("Failed to connect to Server.  Exiting......");
                System.Environment.Exit(-1);

            }
            
            //need to look for jobs
            while(_RUNNING)
            {
                Console.WriteLine(CommandMessage());
                string msg =Console.ReadLine();
                

                Thread.Sleep(1);
            }
            //jqeThread.Join();



            //wmt.sendJob();
            //Console.WriteLine("Press button");
            //Console.Read();

            if (false)
            {
                Console.WriteLine("Hello World!");
                WorkerManager wm = new WorkerManager();
                wm.Running = true;
                Thread backgroundThread = new Thread(new ThreadStart(wm.AddWorkerThread));
                // Start thread  
                backgroundThread.Start();
                backgroundThread.Join();
            }
        }

        public static string CommandMessage()
        {
            //String msg  = @"Enter a command: W:";
            StringBuilder msg = new StringBuilder();
            msg.AppendLine("Enter a Command:");
            msg.AppendLine("Q - QUit(Exit)");
            msg.AppendLine("W - Show WOrkers");
            msg.AppendLine("S - Show status");

            return msg.ToString();
        }

        public static void exitApp()
        {
            Console.WriteLine("Exiting Controller");
            System.Environment.Exit(-1);
        }



    }
}
