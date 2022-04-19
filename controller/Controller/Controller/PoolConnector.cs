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
    class PoolConnector
    {
        private TcpClient _Client;
        private string _Address;
        private int _Port;
        private string _User;
        private string _Pwd;
        private ILogger _Logger;
        private JobQueue _Queue;
        private int _Nonce;
        private int _extraNonce;
        public PoolConnector(
            string Address,
            int Port,
            string user,
            string pwd,
            ILogger logger, JobQueue queue)
        {

            _Logger = logger;
            _Address = Address;
            _Port = Port;
            _User = user;
            _Pwd = pwd;
            _Queue = queue;

        }
/*
        private Hashtable _htcmdIDS;
        public Hashtable htcmdIDS
        {
            get 
            { 
                return _htcmdIDS; 
            }
        }*/
        private int? _difficulty;

        public int? Difficulty{
            get{return _difficulty;}

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

        public bool ConnectPool()
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
                _Logger.LogError(exp, "Failed to connect");
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
                _Client.GetStream().Flush();
                Program.Acks.AddAck(ps.id,ps.method);
                retval = true;
            }
            catch (Exception exp) 
            {
                _Logger.LogError(exp, "failed to Subscribe");
            }
            return retval;
        }

        public bool authorize()
        {
            bool retval = false;
            try
            {
                PoolAuth pa = new PoolAuth(1, "uname", "pwd");
                string json = JsonConvert.SerializeObject(pa, Formatting.None);
                json = json + "\n";
                byte[] bytes = Encoding.ASCII.GetBytes(json);
                _Client.GetStream().Write(bytes, 0, bytes.Length);
                _Client.GetStream().Flush();
                Program.Acks.AddAck(pa.id, pa.method);
                retval = true;
            }
            catch (Exception exp)
            {
                _Logger.LogError(exp, "failed to Authorize");
            }
            return retval;
        }
        public class PoolSubscriber
        {
            public PoolSubscriber( int ID)
            {
                this.id = ID;
                method = "mining.subscribe";
                parameters = new ArrayList();
            }
            /*{"id": 1, "method": "mining.subscribe", "params": []}\n;*/
            public int id;
            public string method;
            [JsonProperty(PropertyName = "params")]
            public ArrayList parameters;
        }
        public class PoolAuth
        {
            public PoolAuth(int ID, string uname, string pwd)
            {
                this.id = ID;
                method = "mining.authorize";
                parameters = new ArrayList();
                parameters.Add(uname);
                parameters.Add(pwd);
            }
            /*{"id": 1, "method": "mining.subscribe", "params": []}\n;*/
            public int id;
            public string method;
            [JsonProperty(PropertyName = "params")]
            public ArrayList parameters;
        }

        public void runListner()
        {
            string AllData = string.Empty;

            NetworkStream ns = _Client.GetStream();
            byte[] buffer = new byte[_Client.ReceiveBufferSize];

            int read = ns.Read(buffer, 0, buffer.Length);
            AllData = ASCIIEncoding.ASCII.GetString(buffer, 0, read);
            // parse response run any rules
            int cmdEnd = AllData.IndexOf("}");
            while (cmdEnd > 0)
            {
                string cmdTxt = AllData.Substring(0, cmdEnd + 1);
                //parse and process
                JObject Robj = JObject.Parse(cmdTxt);
                if (Robj.ContainsKey("method") && Robj["method"] != null)
                {
                    string method = Robj.ToString();
                    if (String.Compare(method, "mining.notify") == 0)
                    {
                        MiningNotify(cmdTxt);
                    }
                    else
                    if (String.Compare(method, "mining.set_difficulty") == 0)
                    { 
                        MiningNotify(cmdTxt);
                    
                    }
                }
                /*
                    public class StratumResponse
                    {
                        [DataMember]
        public ArrayList error;
        [DataMember]
        public System.Nullable<int> id;
        [DataMember]
        public object result;
    }
                Response.error != null || Response.result != null
                
                */



                else //if (Robj.ContainsKey("method"))
                {
                    if(  
                        (Robj.ContainsKey("error") && Robj["error"] != null  && Robj["error"].ToString().Trim().Length>0   )
                            ||    
                        (Robj.ContainsKey("result") && Robj["result"] != null  && Robj["result"].ToString().Trim().Length>0   )
                    )  
                    {
                         
                        int id;// = Robj["ID"];
                        if( int.TryParse( Robj["ID"].ToString(), out id  ))
                        {
                            string method = Program.Acks.getACK(id);
                            if (String.Compare(method, "mining.authorize") == 0)
                            {
                                miningauthorizeACK(cmdTxt);
                            }
                            else if (String.Compare(method, "mining.subscribe") == 0)
                            { 
                                miningsubscribeACK(cmdTxt);
                    
                            } else if (String.Compare(method, "mining.submit") == 0)
                    { 
                                    miningsubmitACK(cmdTxt);
                    
                    }        








                        }

                    /*Handle the acks                  */
                    
                    }





                }
            }
        }

        /// <summary>
        /// THis handles the notify. from the pool
        /// </summary>
        /// <param name="notifyJson">the JSON</param>
        /// <returns></returns>
        public bool MiningNotify(string notifyJson)
        {/*params[0]	string	The job ID for the job being sent in this message.
params[1]	string	The hex-encoded previous block hash.
params[2]	string	The hex-encoded prefix of the coinbase transaction (to precede extra nonce 2).
params[3]	string	The hex-encoded suffix of the coinbase transaction (to follow extra nonce 2).
params[4]	array	A JSON array containing the hex-encoded hashes needed to compute the merkle root. See Merkle Tree Hash Array.
params[5]	string	The hex-encoded block version.
params[6]	string	The hex-encoded network difficulty required for the block.
params[7]	string	The hex-encoded current time for the block.
params[8]*/
            bool retval = false;
            var obj = JObject.Parse(notifyJson);
            JArray prms = (JArray)obj["params"];
            JArray MA = (JArray)prms[4];
            string[] aMerk = new string[MA.Count];
            MineTools.MineJob m = new MineJob() 
            { 
                clear      = bool.Parse(Convert.ToString(prms[8])),
                CoinFollow = Convert.ToString(prms[3]),
                CoinPre    = Convert.ToString(prms[4]),
                Difficulty = Convert.ToString(prms[6]),
                ID         = Convert.ToString(prms[0]),
                Merk       = aMerk,
                NetTime    = Convert.ToString(prms[7]),
                PrevHash   = Convert.ToString(prms[2]),
                Ver        = Convert.ToString(prms[5])
            };
            _Queue.AddJob(m);
            return retval;
        }


        public bool MiningSetDifficulty(string difJson)
        {
            bool retval = false;
            var nObj = JObject.Parse(difJson);
            //var obj = JObject.Parse(notifyJson);
            JArray prms = (JArray)nObj["params"];
            //string sval =prms[0];
            int dval;
            if(int.TryParse(prms[0].ToString(),out dval))
            {
                retval=true;
                _difficulty= dval;
            }else
            {
                _difficulty=null;
            }
            return retval;
        }
        public void miningauthorizeACK(string json)
        {
            var obj = JObject.Parse(json);
            if(obj.ContainsKey("result")
                && (bool) obj["result"] 
            )
            {
                _Logger.LogMessage("Worker accepted");

            }else
            {
                _Logger.LogMessage("Worker rejected");
                _Logger.LogMessage(string.Format("Full JSON:{} ", json   ));
                Environment.Exit(-1);
            }}
         public void miningsubscribeACK(string json)
        {
            var obj = JObject.Parse(json);
            if( obj.ContainsKey("result")       )
            {
                JArray arr =     obj["result"] as JArray;
                var en = arr[1];
                if(int.TryParse(Convert.ToString(en), out _extraNonce))
                {
                    Console.WriteLine("Set ExtraNonce");
                }else
                {
                    _Logger.LogMessage(String.Format("Failed to Subscribe:{0}", json  ));
                }

            }
        }

        public void miningsubmitACK(string json)
        {
            var obj = JObject.Parse(json);
            if( obj.ContainsKey("result")       )
            {


            }
            if (Response.result != null && (bool)Response.result)
                    {
                        SharesAccepted++;
                        Console.WriteLine("Share accepted ({0} of {1})", SharesAccepted, SharesSubmitted);
                    }
                    else
                        Console.WriteLine("Share rejected. {0}", Response.error[1]);


        }
    }
}
