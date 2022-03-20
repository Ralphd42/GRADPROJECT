﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MineTools;
using Newtonsoft.Json;
namespace Controller
{
    public class JobManager
    {
        private MineTools.MineJob _job;
        private List<Worker> _workers;
        private List<MineThreadData> _mineThData;
        public JobManager(MineJob job, List<Worker> Workers)
        {
            _job = job;
            _workers = Workers;
            _mineThData = new List<MineThreadData>();
        }


        public void startJob()
        {
            uint thNonce = 0;
            int totalThreads = (from w in _workers select w.Processors).Sum();
             
            for (int dJobid = 0; dJobid < _workers.Count; ++dJobid)
            {
                MineThreadData thd = new MineThreadData()
                {
                    id = string.Format("{0}", dJobid),
                    increment = (uint)totalThreads,
                    Nonce = (uint)thNonce,
                    numToRun = _workers[dJobid].Processors,
                    target = _job.target,
                    thData = Encoding.ASCII.GetBytes(_job.Data)  
                    
                };
                // launch agents( not threads)






                thNonce += (uint)_workers[dJobid].Processors;
            }
        }
    }
}