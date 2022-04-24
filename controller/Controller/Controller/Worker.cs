using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    /// <summary>
    /// This is a worker
    /// </summary>
    public class Worker
    {
        private string _ipv4;
        private string _machineName;
        private int _procCount;
        public string MachineName { get => _machineName; set => _machineName = value; }
        public string Ipv4 { get => _ipv4; }
        public int Processors {
            get => _procCount; set => _procCount = value;
        }
        /// <summary>
        ///  This can be used later want to know client type 
        /// </summary>
        public string ClientType{
            get;set;
        }

        public Worker(string ip)
        {
            _ipv4 = ip;
        
        }


        public Worker(string ip, string name, int pCnt)
        {
            _ipv4 = ip;
            _machineName = name;
            _procCount = pCnt;



        }


    }
}
