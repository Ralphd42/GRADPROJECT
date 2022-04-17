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
    public class JobQueue
    {
        private ConcurrentQueue<MineJob> _jobs;

        public void AddJob(MineJob jb)
        {
            if (_jobs == null)
            {
                _jobs = new ConcurrentQueue<MineJob>();
            }
            _jobs.Enqueue(jb);
        }

        public int count()
        {
            int retval = _jobs!=null ?_jobs.Count :0;
            return retval;
        }

        public MineJob  GetJob()
        {
            MineJob ret = null;
            if (_jobs.TryDequeue(out ret))
            {
                return ret;
            }
            return ret;
        }
    }
}