using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MineTools;
using System.Net;
 
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
        
        private static object foundObj = new object();
        
        public void handleFound(object sender, uint newNonce)
        {
            lock (foundObj)
            {
                if (_running)
                {
                    _running = false;
                    string msg = string.Format("<F>{0}:{1}#","1", newNonce);
                    byte[] bytes = Encoding.ASCII.GetBytes(msg);
                    IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                      
                    IPAddress conIPA = IPAddress.Parse(Program.ControllerIP);
                     
                    TcpClient tcpClient = new TcpClient(AddressFamily.InterNetwork);
                    tcpClient.Connect(conIPA,Program.JobRPort);
                    var _nsm = tcpClient.GetStream();
                    _nsm.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback((IAsyncResult ar) =>
                    {

                            _nsm.Flush();
                            _nsm.Close();
                    }), null);
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
            notifyAvail();
        }


        public void notifyAvail()
        {
            bool joined = Program.mi.joinNetwork(Program.AgentName);
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
                Console.WriteLine("Launched Thread:{0}|Nonce:{1}|Increment:{2}, ", jobData.id,
                  jobData.Nonce ,jobData.increment   );


            }
            /*Thread thStats = new Thread(new ThreadStart(this.isActive));
            thStats.IsBackground = true;
            thStats.Start();*/
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
    }
}
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

        #endif
        #endregion
    