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
        public string MachineName { get => _machineName;  }
        public string Ipv4 { get => _ipv4; }

        public Worker(string ip, string name, int pCnt)
        {
            _ipv4 = ip;
            _machineName = name;
            _procCount = pCnt;



        }


    }
}
