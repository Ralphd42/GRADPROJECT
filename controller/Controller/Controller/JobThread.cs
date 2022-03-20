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

        public JobThread(  Worker Worker, MineThreadData MTD)
        {
            
            _worker = Worker;
            dt      = MTD   ;
        
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
                while ((byrd = stream.Read(rb)) > 0)
                {
                    objString.Append(Encoding.ASCII.GetString(rb));
                    Console.Write(objString.ToString());
                }
                Console.Write("DONE");
            }
        }
    }
}
