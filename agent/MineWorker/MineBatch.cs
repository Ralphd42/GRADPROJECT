using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
        private NetworkStream _nsm;
        public MineBatch(MineThreadData thedata, NetworkStream nsm)
        {
            _running = true;
            _thData = thedata;
            _nsm = nsm;
        }



        

         

        void isActive()
        {
            while (_running)
            {
                bool retval = false;
                if (_nsm != null)
                {
                    if ( _nsm.CanWrite    /*_nsm.Socket.Poll(100, SelectMode.SelectWrite)*/)
                    {
                        retval = true;
                    }
                }
                KillJobs();
                _running = retval;
                Thread.Sleep(1);
            }
        }






        private static object foundObj = new object();
        public void handleFound(object sender, uint newNonce)
        {
            //one notify all threads of done
            // send to controller that we won
            lock (foundObj)
            {   if (_running)
                {
                    _running = false;
                    string msg = string.Format("<F>{0}#", newNonce);
                    byte[] bytes = Encoding.ASCII.GetBytes(msg);
                    if (_nsm.CanWrite)
                    {
                        _nsm.Write(bytes, 0, bytes.Length);
                        _nsm.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback((IAsyncResult ar) => 
                        { 
                             _nsm.Flush();
                            _nsm.Close();
                            _nsm.Dispose();    


                        }), null);
                       
                    }
                    KillJobs();
                }
                
            }
        }

        public void KillJobs()
        {
            foreach (var jb in _jobs)
            {
                jb.KillProc();
            }
        }


        public void LaunchThreads()
        {
            _running = true;
            _threads = new Thread [_thData.numToRun];
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
                _jobs[i].foundNonce += this.handleFound;
                _threads[i] = new Thread(new ThreadStart(_jobs[i].runJob));
                _threads[i].Start();
            }
            Thread thStats = new Thread(new ThreadStart(this.isActive));
            thStats.IsBackground = true;
            thStats.Start();
            for (uint i = 0; i < _thData.numToRun; ++i)
            {
                _threads[i].Join();
            }
        }

        //----------------------------------------------------------------------------------------
        //unused code
        #region  UNUSEDCODE
        #if UNUSED
        public void testwb()
        {
            Console.WriteLine("--ZZZZZZZZ--");
            try
            {
                Console.WriteLine("--AAA--");
                byte[] bytes = Encoding.ASCII.GetBytes("FUCKING YEAH");
                _nsm.Write(bytes, 0, bytes.Length);
                Console.WriteLine("--1--");
                _nsm.Flush();
                Console.WriteLine("--2--");
                _nsm.Close();
                Console.WriteLine("--3--");
                _nsm.Dispose();
                Console.WriteLine("--4--");
            }
            catch (Exception exp)
            {
                Console.WriteLine(":{0}:",exp.Message);
            }
        }
        #endif
        #endregion
    }
}
