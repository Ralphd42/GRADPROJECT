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
        /// <summary>
        /// this is a threadsafe QUEUE object
        /// </summary>
        private ConcurrentQueue<MineJob> _jobs;
        private int _extraNonce2 = 0;
        /// <summary>
        /// This adds a job to the QUEUE
        /// </summary>
        /// <param name="jb"></param>
        public void AddJob(MineJob jb)
        {
            if (_jobs == null)
            {
                _jobs = new ConcurrentQueue<MineJob>();
            }
            _jobs.Enqueue(jb);
        }

        /// <summary>
        /// A count of items in the Job Queue
        /// </summary>
        /// <returns></returns>
        public int count()
        {
            int retval = _jobs != null ? _jobs.Count : 0;
            return retval;
        }

        /// <summary>
        /// removes a job from the QUEUE
        /// This should only happen when job is being worked
        /// </summary>
        /// <returns>The job to run</returns>
        public MineJob GetJob()
        {
            MineJob ret = null;
            if (_jobs.TryDequeue(out ret))
            {
                _currJob = ret;
                ++_extraNonce2;
                return ret;
            }
            return ret;
        }
        private MineJob _currJob;
        public MineJob CurrentJob { get => _currJob; } 
        public int ExtraNonce2 {get => _extraNonce2; }

        public void ClearCurrent()
        {
            _currJob = null;

        }
    }
}