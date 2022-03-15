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
        private int _addPort;
        private bool Running;
        private TcpListener tcpListener;

        public JobListner()
        {
            Running = true;
        }
        public void ListenToController()
        {
            byte[] bytes = new Byte[1024];
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
           // IPEndPoint ep = new IPEndPoint()
            Console.WriteLine(string.Format("IPADDRESS:{0}|{1}|{2}  ",
                IPAddress.Parse(ipAddress.ToString()),
                ipAddress.ToString(), ipAddress
                ));
            Int32 port = 13001;
            tcpListener = new TcpListener(new IPEndPoint(IPAddress.IPv6Any, port));//      new(ipAddress, port);
            
            //tcpListener.
            try
            {
                tcpListener.Start();
                byte[] rb = new byte[1024];
                TcpClient cli = tcpListener.AcceptTcpClient();
                NetworkStream nsm = cli.GetStream();
                int byrd = 0;
                StringBuilder objString = new StringBuilder();
                while ((byrd = nsm.Read(rb)) > 0)
                {
                    objString.Append(Encoding.ASCII.GetString(rb, 0, byrd));
                    string svr = objString.ToString();
                    svr = svr.Trim();
                    Console.WriteLine(svr);
                    Console.WriteLine("LAST CHAR {0}", svr[svr.Length-1]);
                    if (svr[svr.Length - 1] == '}')
                    {
                        break;
                    }

                }
                MineThreadData dt = JsonConvert.DeserializeObject<MineThreadData>(objString.ToString());
                Console.WriteLine(dt.id);
                MineBatch mb = new MineBatch(dt,nsm);
                mb.testwb();
                // Start listening for connections.  

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }






    }
}
