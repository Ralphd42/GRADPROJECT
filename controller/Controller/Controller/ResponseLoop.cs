using System;
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
    /// <summary>
    /// This will handle the responses from the agents
    /// It will tell  use pool connector to notify POOL of sucess
    /// It will notify 
    /// </summary>
    public class ResponseLoop
    {
        public void RunLoop()
        {
            byte[] bytes = new Byte[1024];
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Program.JobRPort);
            Console.WriteLine(
                string.Format(
                    "Currently listiening at IPADDRESS:{0}|{1}|{2} for responses from agents",
                    IPAddress.Parse(ipAddress.ToString()),
                    ipAddress.ToString(),
                    ipAddress
                )
            );
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);//   ..Tcp);
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.  
                while (Program._RUNNING)
                {
                    Socket handler = listener.Accept();
                    IPAddress remAddy = IPAddress.Parse(((IPEndPoint)handler.RemoteEndPoint).Address.ToString());
                    //handler.RemoteEndPoint.
                    var rmp = (IPEndPoint)handler.RemoteEndPoint;
                    var af = rmp.Address;
                    string data = string.Empty;
                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("#") > -1)
                        {
                            ProcResponse(data);
                            //next need to kill threads for this job
                            Program.jqe.killThreads();
                            break;
                            
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Program.lgr.LogError(exp, "Error in RunLoop Response processing");
            }
        }
        private void ProcResponse(string response)
        {
            string clean = MineTools.CommParser.removeHT(response);
            string[] itms = clean.Split(":");
            string ids = itms[0];
            string nonces = itms[1];
            int nonce = int.Parse(itms[1]);
            int id = int.Parse(ids);
            MineJob cur = Program.MainJobQueue.CurrentJob;
            Program.pm.PoolFuncts.miningSubmit(
                 jobid: cur.ID,
                 extranonce: Program.MainJobQueue.ExtraNonce2.ToString("x8"),
                 Time: cur.Data,
                 nonceHxSt: nonce.ToString("x8"),
                 ID: id
            );
        }
    }
}
