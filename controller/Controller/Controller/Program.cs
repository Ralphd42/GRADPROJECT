using System;
using System.Threading;

namespace Controller
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            WorkerManager wmt = new WorkerManager();
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
