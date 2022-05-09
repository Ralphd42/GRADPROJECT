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
                
                // Start listening for connections.  
               listener.BeginAcceptTcpClient(acceptCB, listener);
            }
            catch (Exception e)
            {
                Program.lgr.LogError(e, "Error in Monitor");
            }
        }
        void acceptCB(IAsyncResult ar)
        {
            byte[] rb = new byte[1024];
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
            if (svr.Contains("<E>"))
            { 
                string rv = string.Format("<E>{0}#", Program.lgr.LastExMsg);
                byte[] outB = Encoding.ASCII.GetBytes(rv);
                nsm.Write(outB, 0, outB.Length);
                nsm.Flush();
            }else
            if (svr.Contains("<W>"))
            {
                string rv = string.Format("<W>{0}#", Program.wmt.ToString());
                byte[] outB = Encoding.ASCII.GetBytes(rv);
                nsm.Write(outB, 0, outB.Length);
                nsm.Flush();
            }else if (svr.Contains("<J>"))
            {
                string rv = string.Format(
                    "<J>Jobs in Queue:{0}#",
                    Program.MainJobQueue.count()
                );
                byte[] outB = Encoding.ASCII.GetBytes(rv);
                nsm.Write(outB, 0, outB.Length);
                nsm.Flush();
            }
            else if (svr.Contains("<K>"))
            {
                string rv = string.Format(
                    "<K>{0}#", "Aww yah got me...Shutting Down");
                byte[] outB = Encoding.ASCII.GetBytes(rv);
                nsm.Write(outB, 0, outB.Length);
                nsm.Flush();
                Console.Write("Attempting Kill from monitor");
                Program.KIllApp();
                Program._RUNNING = false;
            }
            if (Program._RUNNING)
            {
                listener.BeginAcceptTcpClient(acceptCB, listener);
            }
            
        }




    }
}