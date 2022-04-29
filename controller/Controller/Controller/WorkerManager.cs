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
    public class WorkerManager
    {
        private List<Worker> _workers;
        private bool _running;
        private int _addPort; 
        public WorkerManager()
        {
            Running = true;
            _addPort = Program.WorkerManagerPort; 
        }

/// <summary>
///  returns workers.  This probably should be changed to a copy
/// </summary>
        public List<Worker> Workers{get => _workers;}

        public bool Running { get => _running; set => _running = value; }
       
/// <summary>
///     adds a worker to workers
/// </summary>
/// <param name="w">A worker object</param>
/// <returns>true if added</returns>
        public bool addWorker(Worker w)
        {
            bool retval = false;
            if (_workers == null)
            {
                _workers = new List<Worker>();
            }
            var ex = from r in _workers where 
                 
                r.MachineName.Trim().ToLower() ==
                w.MachineName.Trim().ToLower() select r;
            if (ex.Count() <= 0)
            {
                _workers.Add(w);
                retval = true;
            }
            return retval;        
        }

        public bool removeWorker(string MachineName)
        {
            MachineName = MachineName.Trim().ToLower();
            bool retval = false;
            if (_workers == null)
            {
                _workers = new List<Worker>();
            }
            var ex = from r in _workers where 
                //r.Ipv4 == w.Ipv4 || 
                r.MachineName.Trim().ToLower() ==
                MachineName select r;
            if (ex.Count() > 0)
            {
                _workers.Remove(ex.FirstOrDefault());
                retval = true;
            }
            return retval;        
        }





        public void ShowWorkers(string msg ="")
        {
            if (msg.Length > 0)
            {
                Console.WriteLine(msg);

            }
            Console.WriteLine("Current WORKERS(Agents)");
            foreach (Worker w in this._workers)
            {
                Console.WriteLine("Name:{0} |IP: {1} ", w.MachineName, w.Ipv4);
            }
        }


        /**
            this is a tester
        
        */
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
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _addPort);
            Console.WriteLine(
                string.Format(
                    "Currently listiening at IPADDRESS:{0}|{1}|{2}"    , 
                    IPAddress.Parse(ipAddress.ToString()),
                    ipAddress.ToString(),
                    ipAddress
                )
            );
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp );//   ..Tcp);
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
                        Worker wk = new Worker(remAddy.ToString());
                        CommParser.ParseWorker(data, wk);
                        bool rv = addWorker(wk);
                        ShowWorkers(String.Format("ADDED: {0}",data));
                        //byte[] msg = Encoding.ASCII.GetBytes(data);
                        handler.Send(Encoding.ASCII.GetBytes("<A>1#"));
                    }else if (data.ToUpper().Contains("<B>"))
                    {
                        data = CommParser.removeHT(data);
                        bool rv = removeWorker(  data );
                        ShowWorkers(String.Format("Removed: {0}",data));
                        //byte[] msg = Encoding.ASCII.GetBytes(data);
                        handler.Send(Encoding.ASCII.GetBytes("<B>1#"));
                    }
                    else
                    {
                        handler.Send(Encoding.ASCII.GetBytes("<A>2INVALID MESSAGE#"));
                    }
                    // Echo the data back to the client.  
                    
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Program.lgr.LogError(e, "Error in AddWorkerThread Listner");
            }
        }

    }
}
