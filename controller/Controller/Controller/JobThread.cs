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
        private uint _nonce;
        public void KIll()
        {
            _running = false;
        }
        public JobThread(  Worker Worker, MineThreadData MTD)
        {
            
            _worker = Worker;
            dt      = MTD   ;
            _running = true;
        }
        public void sendJob()
        {
            IPAddress ipAddress = IPAddress.Parse(_worker.Ipv4);
            TcpClient tcpClient = new TcpClient(AddressFamily.InterNetwork);
            tcpClient.Connect(ipAddress, Settings.JOBPORT);
            if (tcpClient != null)
            {
                NetworkStream stream = tcpClient.GetStream();
                string dtJson = JsonConvert.SerializeObject(dt);
                byte[] toSend = Encoding.ASCII.GetBytes(dtJson);
                stream.Write(toSend, 0, toSend.Length);
                stream.Flush();
                // receive response
                int byrd = 0;
                byte[] rb = new byte[1024];
                StringBuilder objString = new StringBuilder();
                bool found = false;
                while ((byrd = stream.Read(rb)) > 0)
                {
                    if (!_running)
                        break;
                    found = true;
                    objString.Append(Encoding.ASCII.GetString(rb));
                    Console.Write(objString.ToString());
                }
                if (found)
                {
                    string strNonce = CommParser.removeHT(objString.ToString());
                    if (uint.TryParse(strNonce,out  _nonce)) 
                    {
                        OnFoundNonce();
                    }
                }
                Console.Write("DONE");
            }
        }
        public void OnFoundNonce()
        {
            if (_running)
            {
                foundNonce?.Invoke(this, _nonce);
                _running = false;
            }
        }
    }
}
