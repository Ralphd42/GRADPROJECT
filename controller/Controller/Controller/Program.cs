using System;
using System.Threading;

namespace Controller
{



    class Program
    {   
        public static bool _RUNNING = true;
        public static JobQueue  MainJobQueue;
        
        static void Main(string[] args)
        {
            MainJobQueue = new JobQueue();
            // start waiting for Threads.
            WorkerManager wmt = new WorkerManager();
            Thread wmtThread = new Thread(new ThreadStart(wmt.AddWorkerThread));
            wmtThread.Start();
            





            //need to look for jobs
            while(_RUNNING)
            {
                if (wmt.Workers.Count > 0)
                {
                        

                    JobQueueWatcher jqe = new JobQueueWatcher(MainJobQueue,wmt.Workers);
                    Thread thRunJobs = new Thread(new ThreadStart(jqe.WatchQueue));
                    thRunJobs.Start();
                    thRunJobs.Join();
                }
                Thread.Sleep(500);
            }




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
    }
}
