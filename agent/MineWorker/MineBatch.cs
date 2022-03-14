using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MineTools;
namespace MineWorker
{
    
    /// <summary>
/// This manages the minining
/// 1) launches threads with received job
/// 2) notifies thread to stop
/// 3) notifies miner is complete
/// 4) handles stopping theread
/// </summary>
    class MineBatch
    {
        private Thread[] _threads;
        private Miner[] _jobs;
        private MineThreadData _thData;
        private bool _running ;
        public MineBatch(MineThreadData thedata)
        {
            _running = true;
            _thData = thedata;
        
        
        }
        void LaunchThreads()
        {
            _threads = new Thread        [_thData.numToRun];
            _jobs = new Miner[_thData.numToRun];

            for (uint i = 0; i < _thData.numToRun; ++i)
            {
                MineThreadData jobData = new MineThreadData();
                jobData.id = (i + 1).ToString() + ":";
                jobData.increment = _thData.increment + i;
                jobData.Nonce = _thData.Nonce + i;
                jobData.target = _thData.target;
                jobData.thData = _thData.thData;
                _jobs[i] = new Miner(jobData);
                _threads[i] = new Thread(new ThreadStart(_jobs[i].runJob));
                _threads[i].Start();
            }
            for (uint i = 0; i < _thData.numToRun; ++i)
            {
                _threads[i].Join();
            }
        }





    }
}
