using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace Controller
{
    /// <summary>
    /// controller class for managing workers
    /// 
    /// </summary>
    public class WorkerManager
    {
        private List<Worker> _workers;
        public bool addWorker(Worker w)
        {
            bool retval = false;
            if (_workers == null)
            {
                _workers = new List<Worker>();
            }
            var ex = from r in _workers where r.Ipv4 == w.Ipv4 
                     || r.MachineName.Trim().ToLower() ==
                     w.MachineName.Trim().ToLower() select r;
            if (ex.Count() <= 0)
            {
                _workers.Add(w);
                retval = true;
            }
            return retval;        
        }

        public void AddWorkerThread()
        {
            byte[] bytes = new Byte[1024];
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

        }

    }
}
