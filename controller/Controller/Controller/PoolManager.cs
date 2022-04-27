using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MineTools;
namespace Controller
{
    public class PoolManager
    {
        public static string PoolAddress
        {
            get
            {
                //return "stratum+tcp://us-east.stratum.slushpool.com";
                return "us-east.stratum.slushpool.com";
            }
        }
        public static int PoolPort
        {
            get
            {
                return 3333;

            }

        }

        public static string PoolUser
        {
            get
            {
                return "ralphd42.GRADPROJ";

            }

        }
        public static string PoolPwd
        {
            get
            {
                return "anything123";

            }

        }

        private JobQueue _queue;
        private PoolConnector _poolConn;
        public PoolManager(JobQueue queue)
        {
            _queue = queue;
            _poolConn = new PoolConnector(PoolAddress, PoolPort, PoolUser, PoolPwd, new Logger(), _queue);
        }

        public bool startConnectionToPool()
        {
            bool retval = false;
            if (_poolConn.ConnectPool())
            {
                if (_poolConn.subscribe())
                {
                    if (_poolConn.authorize())
                    {
                        retval = true;
                    }
                }
            }
            return retval;
        }
        public void StartListner()
        {
            _poolConn.runListner();


        }

    }
}