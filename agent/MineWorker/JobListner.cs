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
                MineBatch mb=null;
                while (Running)
                {
                    TcpClient cli = tcpListener.AcceptTcpClient();
                    //cli.Client.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.KeepAlive,true);
                    NetworkStream nsm = cli.GetStream();
                    nsm.ReadTimeout = int.MaxValue;
                    nsm.WriteTimeout = int.MaxValue;
                    int byrd = 0;
                    StringBuilder objString = new StringBuilder();
                    string command ="";
                    while ((byrd = nsm.Read(rb)) > 0)
                    {
                        objString.Append(Encoding.ASCII.GetString(rb, 0, byrd));
                        string svr = objString.ToString();
                        svr = svr.Trim();
                        Console.WriteLine("received json {0}", svr);
                        command =svr; 
                        if(command.Contains("<K>")|| command.Contains("<T>") )
                        {
                            break;
                        } 
                        if (svr[svr.Length - 1] == '}')
                        {
                            break;
                        }
                    }
                    if( command.Contains("<K>")  || command.Contains("<T>") )
                    {
                        Console.WriteLine("Received Kill at{0}", DateTime.Now);
                        if(mb!=null)
                        { 
                            mb.KillJobs();
                            mb.notifyAvail();
                        }
                    }
                    if( command.Contains("<T>"))
                    {
                        Running=false;
                        if(mb!=null)
                        {
                            mb.KillJobs(false);
                        }
                        //Terminate app
                        System.Environment.Exit(1);
                    }
                    else
                    {
                        try
                        {
                            MineThreadData dt = JsonConvert.DeserializeObject<MineThreadData>(command);
                            mb = new(dt, nsm);
                            new Thread( new ThreadStart(   mb.LaunchThreads)).Start();
                        }catch(Exception exp)
                        {
                            _logger.LogError(exp,"Parsing Json");            
                            Console.WriteLine(exp.Message);
                            Console.WriteLine(exp.StackTrace);
                            Console.WriteLine(exp.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,"ListenToController");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }






    }
}
 