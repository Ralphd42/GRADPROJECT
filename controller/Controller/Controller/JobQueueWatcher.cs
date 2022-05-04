using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MineTools;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Concurrent;
namespace Controller
{

    /// <summary>
    /// This class is the JOB QUEUE
    /// It will maintain the job Queue that will be populated by the pool manager
    /// It will be consumed by the job thread.
    /// </summary>
    public class JobQueueWatcher
    {
        private JobQueue _queue;
        private JobManager jm;
        private List<Worker> _workers;
        public JobQueueWatcher(JobQueue queue, List<Worker> workers)
        {
            _queue = queue;
            _workers = workers;
        }

        int availWorkers()
        {
            int retval = 0;
            if( _workers!=null){ 
                int cnt =_workers.Where(x => x.Available == Worker.WorkerState.Available).Count();
            }
            return retval;
        }


        public JobQueueWatcher()
        {
            _queue = Program.MainJobQueue;
        }
        /// <summary>
        /// This will watch and launch jobs
        /// </summary>
        public void WatchQueue()
        {
            while (Program._RUNNING)
            {
                while (_queue.count() == 0  || availWorkers() < Program.MinWorkerCount )
                {
                    Thread.Sleep(500);
                }
                MineJob jb = _queue.GetJob();
                if (jb != null)
                {
                    jm= new JobManager(jb, _workers); 
                    jm.startJobs();
                }
            }
            if (jm != null)
            {
                jm.killThreads();
                
            }
        }
        public void killThreads()
        {
            if (jm != null)
            {
                jm.killThreads();
            }
        }
        public override string ToString()
        {
            string msg = String.Format(
                "Queue Count : {0}|Active:{1} ", _queue.count(),
                 _queue.CurrentJob !=null ? _queue.CurrentJob.ID : "none"
            );
            return msg;
        }
    }
}