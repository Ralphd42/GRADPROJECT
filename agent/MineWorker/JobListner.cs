using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MineTools;
using Newtonsoft.Json;

namespace MineWorker
{
    class JobListner
    {
        private bool Running;
        private TcpListener tcpListener;
        private ILogger _logger;
        public JobListner(ILogger lger)
        {
            _logger = lger;
            Running = true;
        }
        public void ListenToController()
        {
            byte[] bytes = new Byte[1024];
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];
           // IPEndPoint ep = new IPEndPoint()
            Console.WriteLine(string.Format("IPADDRESS:{0}|{1}|{2}  ",
                IPAddress.Parse(ipAddress.ToString()),
                ipAddress.ToString(), ipAddress
                ));
            Int32 port = Program.JobPort;
            tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            
           
            try
            {
                tcpListener.Start();
                byte[] rb = new byte[1024];
               
                while (Running)
                {
                    TcpClient cli = tcpListener.AcceptTcpClient();
                    //cli.Client.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.KeepAlive,true);
                    NetworkStream nsm = cli.GetStream();
                    nsm.ReadTimeout = int.MaxValue;
                    nsm.WriteTimeout = int.MaxValue;
                    int byrd = 0;
                    StringBuilder objString = new StringBuilder();
                    while ((byrd = nsm.Read(rb)) > 0)
                    {
                        objString.Append(Encoding.ASCII.GetString(rb, 0, byrd));
                        string svr = objString.ToString();
                        svr = svr.Trim();
                        Console.WriteLine("received json {0}", svr);
                         
                        if (svr[svr.Length - 1] == '}')
                        {
                            break;
                        }
                    }
                    MineThreadData dt = JsonConvert.DeserializeObject<MineThreadData>(objString.ToString());
                    
                     
                    MineBatch mb = new(dt, nsm);
                    //mb.testwb();
                    mb.LaunchThreads();
                    
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,"ListenToController");
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }






    }
}
