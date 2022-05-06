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

namespace Controller
{
    public class JobThread
    {
        private Worker _worker;
        private MineThreadData dt;
         public event EventHandler<uint> foundNonce;
        private bool _running;
        
        TcpClient tcpClient;

        public void KIll()
        { 
            _running = false;
            try
            {
                IPAddress ipAddress = IPAddress.Parse(_worker.Ipv4);
                tcpClient = new TcpClient(AddressFamily.InterNetwork);
                 
                tcpClient.Connect(ipAddress, Program.JobPort);

                if (tcpClient != null)
                {
                    _stream = tcpClient.GetStream();
                     

                    string doneMsg = "<K>#";
                    byte[] toSend = Encoding.ASCII.GetBytes(doneMsg);
                    _stream.Write(toSend, 0, toSend.Length);
                    _stream.Flush();
                }
            }catch (Exception exp)
            { 
                 Program.lgr.LogError(exp, string.Format("Error Killing stream Agent:{0}", this.dt.id));
            }


        }
        public void KIll_ORIG()
        {
            _running = false;
            _stream = tcpClient.GetStream();
            if (_stream != null)
            {

                string doneMsg = "<K>#";
                byte[] toSend = Encoding.ASCII.GetBytes(doneMsg);
                try
                {
                    Thread.Sleep(10 * 1000);
                    _stream.Write(toSend, 0, toSend.Length);
                    _stream.Flush();
                   // _stream.Close();
                }catch (Exception exp)
                {
                    Program.lgr.LogError(exp, string.Format("Error closing stream thread:{0}", this.dt.id));
                }
            }
            if (tcpClient != null)
            {
                try
                {
                    tcpClient.Close();
                }catch (Exception exp)
                {
                    Program.lgr.LogError(exp, string.Format("Error closing tcpClient thread:{0}", this.dt.id));
                }
            }
        }

        public JobThread(  Worker Worker, MineThreadData MTD)
        {
            _worker  = Worker;
            dt       = MTD   ;
            _running = true  ;
        }

        private NetworkStream _stream;

        public void sendJob()
        {
            IPAddress ipAddress = IPAddress.Parse(_worker.Ipv4);
            tcpClient = new TcpClient(AddressFamily.InterNetwork);
            tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket,
             SocketOptionName.KeepAlive, 2000);
            tcpClient.Connect(ipAddress,Program.JobPort);
            
            if (tcpClient != null)
            {
                _stream = tcpClient.GetStream();
                _stream.ReadTimeout = int.MaxValue;
                string dtJson = JsonConvert.SerializeObject(dt);
                byte[] toSend = Encoding.ASCII.GetBytes(dtJson);
                _stream.Write(toSend, 0, toSend.Length);
                _stream.Flush();
                // receive response
                int byrd = 0;
                byte[] rb = new byte[1024];
                StringBuilder objString = new StringBuilder();
                 
                /* try and indicate dead client   */
                try
                {
                    while ((byrd = _stream.Read(rb)) > 0)
                    {
                        if (!_running)
                            break;
                        // do we want to listem for a message here.
                        objString.Append(Encoding.ASCII.GetString(rb));
                        Console.Write(objString.ToString());
                    }
                }
                catch (Exception exp)
                {
                    if (_running)
                    {
                        Program.lgr.LogError(exp, "sendJob Waiting for data");
                        //Thread died on its own
                    }
                    
                } 
                Console.Write("DONE");
            }
        }
    }
}

#if NotUSED
    // incase needed later
    public void OnFoundNonce()
        {
            if (_running)
            {
                foundNonce?.Invoke(this, _nonce);
                _running = false;
            }
        }
    private uint _nonce;

#endif
