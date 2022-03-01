using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections;
using Newtonsoft.Json;

namespace Controller
{
    class PoolConnector
    {
        private TcpClient _Client;
        private string _Address;
        private int _Port;
        private string _User;
        private string _Pwd;
        private ILogger _Logger;
        public PoolConnector(
            string Address,
            int Port,
            string user,
            string pwd,
            ILogger logger)
        {

            _Logger = logger;
            _Address = Address;
            _Port = Port;
            _User = user;
            _Pwd = pwd;

        }



        public TcpClient poolTCPClient
        {
            get
            {
                return _Client;
            }
        }

        public string Address { get => _Address; set => _Address = value; }
        public int Port { get => _Port; set => _Port = value; }
        public string User { get => _User; set => _User = value; }
        public string Pwd { get => _Pwd; set => _Pwd = value; }
        internal ILogger Logger { get => _Logger; set => _Logger = value; }

        bool ConnectPool()
        {
            bool retval = false;
            try
            {
                _Client = new TcpClient(AddressFamily.InterNetwork);
                _Client.Connect(_Address, _Port);
                if (_Client.Connected)
                {
                    retval = true;
                }
                else
                {
                    _Logger.LogMessage("FAILED TO CONNECT: NO Exception thrown");

                }
            }
            catch (Exception exp)
            {
                _Logger.LogMessage(exp, "Failed to connect");
            }
            return retval;
        }

        public bool subscribe()
        {
            bool retval = false;
            try
            {
                PoolSubscriber ps = new PoolSubscriber(1);
                string json = JsonConvert.SerializeObject(ps, Formatting.None);
                json = json + "\n";
                byte [] bytes = Encoding.ASCII.GetBytes(json);
                _Client.GetStream().Write(bytes, 0, bytes.Length);
            }
            catch (Exception exp) 
            {
                _Logger.LogMessage(exp, "failed to Subscribe");
            }


            return retval;
        }






        public class PoolSubscriber
        {
            public PoolSubscriber( int id)
            {
                method = "mining.subscribe";
                parameters = new ArrayList();
            }
            /*{"id": 1, "method": "mining.subscribe", "params": []}\n;*/
            public int id;
            public string method;
            [JsonProperty(PropertyName = "params")]
            public ArrayList parameters;
        }

    }
}
