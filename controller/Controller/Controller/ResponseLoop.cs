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
    /// controller class for managing workers
    /// Handles adds and removes
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
                            break;
                            // parse it out

                        }
                    }
                }
            }
            catch (Exception exp)
            {




            }
        }
        private void ProcResponse(string response)
        {
            /*response format   */
            /*<R>ID:newNONCE    */
            /*
                parse it
                sent back to network
                Kill all threads for this run
            */
            string clean = MineTools.CommParser.removeHT(response);
            string[] itms = clean.Split(":");
            string ids = itms[0];
            string nonces = itms[1];
            int nonce = int.Parse(itms[1]);
            int id = int.Parse(ids);

            // need way to have all of this available
            // need way to get current job
            MineJob cur = Program.MainJobQueue.CurrentJob;


            Program.pm.PoolFuncts.miningSubmit(
                 jobid: cur.ID    , 
                 extranonce: Program.MainJobQueue.ExtraNonce2  , 
                 Time: cur.Data  , 
                 nonceHxSt:   nonce.ToString("x8")          , ID  :id
                 ) ;





        }


    }
}
