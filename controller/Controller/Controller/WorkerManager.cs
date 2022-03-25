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
    /// 
    /// </summary>
    public class WorkerManager
    {
        private List<Worker> _workers;
        private bool _running;
        private int _addPort;// 11000
        public WorkerManager()
        {
            Running = true;
            _addPort = 11000;
        }

        public List<Worker> Workers
        {
            get 
            {
                return _workers;
            }
        }

        public bool Running { get => _running; set => _running = value; }
       

        public bool addWorker(Worker w)
        {
            bool retval = false;
            if (_workers == null)
            {
                _workers = new List<Worker>();
            }
            var ex = from r in _workers where 
                //r.Ipv4 == w.Ipv4 || 
                r.MachineName.Trim().ToLower() ==
                w.MachineName.Trim().ToLower() select r;
            if (ex.Count() <= 0)
            {
                _workers.Add(w);
                retval = true;
            }
            return retval;        
        }

        public void ShowWorkers()
        {
            foreach (Worker w in this._workers)
            {
                Console.WriteLine("Name:{0} |IP: {1} ", w.MachineName, w.Ipv4);
            }
        }


        public void sendJob()
        {
            byte[] ta = Encoding.ASCII.GetBytes("THIS IS THE TH DATA");
            byte[] tar = Encoding.ASCII.GetBytes("TARGET -- TARGET");

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];



            MineThreadData dt = new MineThreadData()
            {
                thData = ta,
                id = "AA1",
                increment = 100,
                Nonce = 50,
                numToRun = 5,
                target = tar
            };
            TcpClient tcpClient = new TcpClient(AddressFamily.InterNetworkV6);
            tcpClient.Connect(ipAddress, 13001);
            if (tcpClient != null)
            {
                NetworkStream stream = tcpClient.GetStream();
                string dtJson = JsonConvert.SerializeObject(dt);
                byte[] toSend = Encoding.ASCII.GetBytes(dtJson);
                stream.Write(toSend, 0, toSend.Length);
                stream.Flush();
                //stream.Close();
              //  stream = tcpClient.GetStream();
                //NetworkStream nsm = tcpClient.GetStream();
                
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
        
        public void AddWorkerThread()
        {
            byte[] bytes = new Byte[1024];
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _addPort);
            Console.WriteLine(string.Format("IPADDRESS:{0}|{1}|{2}  "    , 
                IPAddress.Parse(ipAddress.ToString(    )),
                ipAddress.ToString(),ipAddress
                ));
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.  
                while (Running)
                {
                    Socket handler = listener.Accept();
                    IPAddress remAddy = IPAddress.Parse(((IPEndPoint)handler.RemoteEndPoint).Address.ToString());
                    //handler.RemoteEndPoint.
                    var rmp = (IPEndPoint)handler.RemoteEndPoint;
                    var af = rmp.Address;
                    string data = string.Empty;
                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("#") > -1)
                        {
                            break;
                        }
                    }

                    // Show the data on the console.  
                    Console.WriteLine("Text received : {0}", data);
                    // parse AND PROCESS
                    if (data.ToUpper().Contains("<A>"))
                    {
                        data = CommParser.removeHT(data);
                        bool rv = addWorker(new Worker(remAddy.ToString(), data,5));
                        ShowWorkers();
                        //byte[] msg = Encoding.ASCII.GetBytes(data);
                        handler.Send(Encoding.ASCII.GetBytes("<A>1#"));
                    }
                    else
                    {
                        handler.Send(Encoding.ASCII.GetBytes("<A>2Already Registered#"));
                    }
                    // Echo the data back to the client.  
                    
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

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
