using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using System.Net;
using System.Net.Sockets;
using MineTools;
using Newtonsoft.Json;





namespace Controller
{
    public class monitor
    {
        private int num;

        public monitor()
        { }
        public void runMonitor()
        {
            byte[] bytes = new Byte[1024];
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Program.MonitorPort);
            Console.WriteLine(
                string.Format(
                    "Currently Monitor is listing at IPADDRESS:{0}|{1}|{2}",
                    IPAddress.Parse(ipAddress.ToString()),
                    ipAddress.ToString(),
                    ipAddress
                )
            );
            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, Program.MonitorPort));
            try
            {
                listener.Start();
                byte[] rb = new byte[1024];
                // Start listening for connections.  
                while (Program._RUNNING)
                {
                    listener.BeginAcceptTcpClient((ar =>
                    {
                        TcpListener listener = (TcpListener)ar.AsyncState;
                        TcpClient client = listener.EndAcceptTcpClient(ar);
                        NetworkStream nsm = client.GetStream();
                        int byrd = 0;
                        StringBuilder objString = new StringBuilder();
                        string svr = string.Empty;
                        while ((byrd = nsm.Read(rb)) > 0)
                        {
                            objString.Append(Encoding.ASCII.GetString(rb, 0, byrd));
                            svr = objString.ToString();
                            svr = svr.Trim();
                            if (svr[svr.Length - 1] == '#')
                            {
                                break;
                            }
                        }
                        if (svr.Contains("<W>"))
                        {
                            string rv = string.Format("<W>{0}#", Program.wmt.ToString());
                            byte[] outB = Encoding.ASCII.GetBytes(rv);
                            nsm.Write(outB, 0, outB.Length);
                            nsm.Flush();
                        }
                        if (svr.Contains("<K>"))
                        { 
                            string rv = string.Format("<K>{0}#", "Aww yah got me...");
                            byte[] outB = Encoding.ASCII.GetBytes(rv);
                            
                            nsm.Write(outB, 0, outB.Length);
                            nsm.Flush();
                            Console.Write("Attempting Kill from monitor");
                            Program.KIllApp();

                        }


                    }), listener);        //   .AcceptTcpClientAsync((ar) => { });
                    rb = new byte[1024];
                }
            }
            catch (Exception e)
            {
                Program.lgr.LogError(e, "Error in AddWorkerThread Listner");
            }
        }
    }
}