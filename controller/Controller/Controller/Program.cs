using System;
using System.Threading;

namespace Controller
{
    class Program
    {   
        private static bool _RUNNING = true;
        static void Main(string[] args)
        {

            // start waiting for Threads.
            WorkerManager wmt = new WorkerManager();
            Thread wmtThread = new Thread(new ThreadStart(wmt.AddWorkerThread));
            wmtThread.Start();

            //need to look for jobs
            while(_RUNNING)
            { 






                
            }




            wmt.sendJob();
            Console.WriteLine("Press button");
            Console.Read();

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
