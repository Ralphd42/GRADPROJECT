﻿using System;
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
        private bool _running;
        private NetworkStream _nsm;
        private ILogger _logger;
        public MineBatch(MineThreadData thedata, NetworkStream nsm, ILogger lgr = null)
        {
            _running = true;
            _thData = thedata;
            _nsm = nsm;
            if (lgr != null)
            {
                _logger = lgr;
            }
        }

        void isActive()
        {
           
            try
            {
                while (_running)
                {
                    Thread.Sleep(1000);
                    if (_nsm != null)
                    {
                        if (IsSocketConnected(_nsm.Socket))
                        {
                             byte[] bytes = Encoding.ASCII.GetBytes(".");
                            _nsm.Write(bytes, 0, bytes.Length);
                             _nsm.Flush();
                            _running = true;
                            continue;
                        }
                        else
                        {
                            try
            {
                Console.WriteLine("--AAA--");
                byte[] bytes = Encoding.ASCII.GetBytes("FUCKING YEAH");
                _nsm.Write(bytes, 0, bytes.Length);
                Console.WriteLine("--1--");
                _nsm.Flush();
                Console.WriteLine("--2--");
                //_nsm.Close();
                Console.WriteLine("--3--");
               // _nsm.Dispose();
                Console.WriteLine("--4--");
            }
            catch (Exception exp)
            {
                Console.WriteLine(":{0}:",exp.Message);
            }
                            _running = false;

                        }
                    }
                    _running = false;
                    KillJobs();

                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);

            }
            Console.WriteLine("Watcher stopped");
        }






        //IsSocketConnected
        void isActive_OLD()
        {
            try
            {
                while (_running)
                {
                    Thread.Sleep(1);
                    if (_nsm != null)
                    {
                        String stats =
                            String.Format(
                                "CANWRITE:{0}|CONNECTED:{1}|SW:{2}|SW:{3}|sr:{4}|sr:{5}",
                                _nsm.CanWrite,
                                _nsm.Socket.Connected,
                                _nsm.Socket.Poll(-1, SelectMode.SelectWrite),
                                _nsm.Socket.Poll(1000, SelectMode.SelectWrite),
                                _nsm.Socket.Poll(-1, SelectMode.SelectRead), 0//,
                                                                              //_nsm.Socket.Poll(1000, SelectMode.SelectRead)
                                );


                        Console.WriteLine(stats);
                        if (_nsm.CanWrite
                            && _nsm.Socket.Connected
                            && !_nsm.Socket.Poll(1000, SelectMode.SelectError))
                        {
                            _running = true;
                            continue;
                        }
                    }
                    KillJobs();
                    _running = false;
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);

            }
            Console.WriteLine("Watcher stopped");
        }






        private static object foundObj = new object();
        public void handleFound(object sender, uint newNonce)
        {
            //one notify all threads of done
            // send to controller that we won
            lock (foundObj)
            {
                if (_running)
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
                           // _nsm.Close();
                            //_nsm.Dispose();


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
            _threads = new Thread[_thData.numToRun];
            _jobs = new Miner[_thData.numToRun];
            for (uint i = 0; i < _thData.numToRun; ++i)
            {
                MineThreadData jobData = new MineThreadData();
                jobData.id = (i + 1).ToString() + ":";
                jobData.increment = _thData.increment + i;
                jobData.Nonce = _thData.Nonce + i;
                jobData.target = _thData.target;
                jobData.thData = _thData.thData;
                _jobs[i] = new Miner(jobData, _logger);
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

        static bool IsSocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if ((part1 && part2) || !s.Connected)
                return false;
            else
                return true;


        }



        //----------------------------------------------------------------------------------------
        //unused code
        #region  UNUSEDCODE
 
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
 
        #endregion
    }
}
