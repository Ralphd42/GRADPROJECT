using System;
using System.Linq;

using System.Text;

using System.Net.Sockets;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MineTools;
namespace Controller
{
    public class PoolConnector
    {
        private TcpClient _Client;
        private string _Address;
        private int _Port;
        private string _User;
        private string _Pwd;
        private ILogger _Logger;
        private JobQueue _Queue;
        private string _extraNonce;
         


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
             
            _extraNonce = "";
            _currCommandID =0;
        }
        private int? _difficulty;

        public int? Difficulty
        {
            get { return _difficulty; }

        }


        public TcpClient poolTCPClient
        {
            get
            {
                return _Client;
            }
        }


        private int  _currCommandID =0;
        private object cmdlock = new object();
        private int NextID()
        {
            lock (cmdlock)
            {
                return ++_currCommandID;
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
                PoolSubscriber ps = new PoolSubscriber(NextID());
                string json = JsonConvert.SerializeObject(ps, Formatting.None);
                json = json + "\n";
                byte[] bytes = Encoding.ASCII.GetBytes(json);
                _Client.GetStream().Write(bytes, 0, bytes.Length);
                _Client.GetStream().Flush();
                AckMan.Acks.AddAck(ps.id, ps.method);
                ExcelLogger.ExcelLog.LogJsonTOPOOL(json);
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
                PoolAuth pa = new PoolAuth(NextID(), PoolManager.PoolUser, PoolManager.PoolPwd);
                string json = JsonConvert.SerializeObject(pa, Formatting.None);
                json = json + "\n";
                byte[] bytes = Encoding.ASCII.GetBytes(json);
                _Client.GetStream().Write(bytes, 0, bytes.Length);
                _Client.GetStream().Flush();
                AckMan.Acks.AddAck(pa.id, pa.method);
                ExcelLogger.ExcelLog.LogJsonTOPOOL(json);
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
            public PoolSubscriber(int ID)
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

        public class miningsubmitjson
        {
            public miningsubmitjson(string JobID, String ExtraNonce, String time, String NonceHS, int id)
            {
                method = "mining.submit";
                parameters.Add(Program.PoolUser);
                parameters.Add(JobID);
                parameters.Add(ExtraNonce);
                parameters.Add(time);
                parameters.Add(NonceHS);
                this.id = id;
            }

            public int id;
            public string method;
            [JsonProperty(PropertyName = "params")]
            public ArrayList parameters;
        }
        public void runListner()  // chnage from thread to async
        {
            if (Program._RUNNING)
            {
                NetworkStream ns = _Client.GetStream();
                byte[] dataBytes = new byte[_Client.ReceiveBufferSize];
                StateObject so = new StateObject() { buffer = dataBytes, NS = ns };
                ns.BeginRead(dataBytes, 0, dataBytes.Length, new AsyncCallback(StratumReaderCallBack), so );

            }
        }
        private class StateObject
        {
            public NetworkStream NS;
            public byte[] buffer;

        }
        void StratumReaderCallBack(IAsyncResult ar){     
        if (Program._RUNNING)
        {
                string AllData = string.Empty;
                StateObject StateOb = (StateObject)ar.AsyncState;
                NetworkStream ns = StateOb.NS;
                int read = ns.EndRead(ar);
                AllData = ASCIIEncoding.ASCII.GetString(StateOb.buffer, 0, read);
                // parse response run any rules
                int cmdEnd = AllData.IndexOf("}");
                string[] commands = AllData.Trim().Split("}");

                _Logger.LogMessage(String.Format("ALL DATA:{0}", AllData));
                
                foreach( String json in commands)
                {
                    string cmdTxt = json.Trim() + "}";
                    ExcelLogger.ExcelLog.LogJsonFromPOOL(json);
                    _Logger.LogMessage(String.Format("Command Text:{0}", cmdTxt));
                    Console.WriteLine("Command Text:{0}", cmdTxt);
                    //parse and process
                    bool parsed = false;
                    JObject Robj=null;
                    if (cmdTxt.Length < 3)
                    {
                        continue;
                    }
                    try
                    {
                        Robj = JObject.Parse(cmdTxt);
                        parsed = true;
                    }
                    catch (  Exception exp )
                    {
                        parsed = false;
                        Logger.LogError(exp, string.Format("Failed to parse:{0}",cmdTxt));

                    }if (parsed)
                    {
                        if (Robj.ContainsKey("method") && Robj["method"] != null)
                        {
                            string method = Robj["method"].ToString();// Robj.ToString();
                            if (String.Compare(method, "mining.notify") == 0)
                            {
                                MiningNotify(cmdTxt);
                            }
                            else
                            if (String.Compare(method, "mining.set_difficulty") == 0)
                            {
                                MiningSetDifficulty(cmdTxt);
                            }
                        }
                        else
                        {
                            if (
                                (Robj.ContainsKey("error") && Robj["error"] != null && Robj["error"].ToString().Trim().Length > 0)
                                    ||
                                (Robj.ContainsKey("result") && Robj["result"] != null && Robj["result"].ToString().Trim().Length > 0)
                            )
                            {

                                int id;
                                if (int.TryParse(Robj["id"].ToString(), out id))
                                {
                                    string method = AckMan.Acks.getACK(id);
                                    if (String.Compare(method, "mining.authorize") == 0)
                                    {
                                        miningauthorizeACK(cmdTxt);
                                    }
                                    else if (String.Compare(method, "mining.subscribe") == 0)
                                    {
                                        miningsubscribeACK(cmdTxt);

                                    }
                                    else if (String.Compare(method, "mining.submit") == 0)
                                    {
                                        miningsubmitACK(cmdTxt);
                                    }
                                }
                            }
                        }
                    }
                    // parse next 


                }
                if (Program._RUNNING)
                { 
                    ns.BeginRead(StateOb.buffer, 0, StateOb.buffer.Length, new AsyncCallback(StratumReaderCallBack), StateOb );
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
            if (Difficulty != null)
            {
               
                var obj = JObject.Parse(notifyJson);
                JArray prms = (JArray)obj["params"];
                JArray MA = (JArray)prms[4];
                string[] aMerk = new string[MA.Count];
                for (int i = 0; i < aMerk.Length; i++)
                {
                    aMerk[i] = Convert.ToString(MA[i]);
                }

                MineTools.MineJob m = new MineJob()
                {
                    clear = bool.Parse(Convert.ToString(prms[8])),
                    CoinFollow = Convert.ToString(prms[3]),
                    CoinPre = Convert.ToString(prms[2]),
                    JobDifficulty = Convert.ToString(prms[6]),
                    ID = Convert.ToString(prms[0]),
                    Merk = aMerk,
                    NetTime = Convert.ToString(prms[7]),
                    PrevHash = Convert.ToString(prms[1]),
                    Ver = Convert.ToString(prms[5]),
                    target = MineTools.CryptoHelpers.GenerateTarget((int)Difficulty),
                    ExtraNONCE1 = _extraNonce.ToString(),
                    ExtraNONCE2 =_Queue.ExtraNonce2.ToString("x8") 

            };
                m.Data = m.GenData;
                //Handle Clear first
                if (m.clear)
                {
                    _Queue.ClearExtraNonce();
                    Program.jqe.killThreads();
                    AckMan.Acks.clear();
                    _Queue.Clear();
                }
                _Queue.AddJob(m);
                //handle clear


            }
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
            if (int.TryParse(prms[0].ToString(), out dval))
            {
                retval = true;
                _difficulty = dval;
            }
            else
            {
                _difficulty = null;
            }
            return retval;
        }
        public void miningauthorizeACK(string json)
        {
            var obj = JObject.Parse(json);
            if (obj.ContainsKey("result")
                && (bool)obj["result"]
            )
            {
                _Logger.LogMessage("Worker accepted");
            }
            else
            {
                _Logger.LogMessage("Worker rejected");
                _Logger.LogMessage(string.Format("Full JSON:{} ", json));
                Environment.Exit(-1);
            }
        }
        public void miningsubscribeACK(string json)
        {
            var obj = JObject.Parse(json);
            if (obj.ContainsKey("result"))
            {
                JArray arr = obj["result"] as JArray;
                var en = arr[1];
                _extraNonce = Convert.ToString(en);
            }
        }

        public void miningsubmitACK(string json)
        {
            var obj = JObject.Parse(json);
            if (obj.ContainsKey("result"))
            {
                bool rv;
                var res = obj["result"];
                if (bool.TryParse(res.ToString(), out rv))
                {
                    Console.WriteLine("We Won!!!!!");
                }
                else
                {
                    Console.WriteLine("Not a winner this time");
                }
            }
        }

        public bool miningSubmit(String jobid,string extranonce, string Time, string nonceHxSt )
        {
            bool retval = false;
            if (Program.debug)
            {
                Console.Write("Debugging not sending to POOL");
                return true;
            }
            try
            {
                miningsubmitjson minSub = new miningsubmitjson(JobID: jobid
                , ExtraNonce: extranonce, time: Time, NonceHS: nonceHxSt, id: NextID()
                );
                string json = JsonConvert.SerializeObject(minSub, Formatting.None);
                json = json + "\n";
                byte[] bytes = Encoding.ASCII.GetBytes(json);
                _Client.GetStream().Write(bytes, 0, bytes.Length);
                _Client.GetStream().Flush();
                AckMan.Acks.AddAck(minSub.id, "mining.submit");
            }catch (Exception exp)
            {
                retval = false;
                _Logger.LogError(exp, String.Format("MINING SUBMIT FAILED FOR JOBID:{0}" , jobid));
            }
            return retval;
        }

        
        public void StratumReaderTester(string AllData){     
        if (Program._RUNNING)
        {
            int cmdEnd = AllData.IndexOf("}");
            string[] commands = AllData.Split("}");

                commands = commands.Where(x => x.Length > 10).ToArray();
                foreach( String json in commands  )
            {
                    string cmdTxt = json.Trim() + "}";
                    
                    Console.WriteLine("Command Text:{0}", cmdTxt);
                    //parse and process
                    bool parsed = false;
                    JObject Robj=null;

                    try
                    {
                        Robj = JObject.Parse(cmdTxt);
                        parsed = true;
                    }
                    catch (  Exception exp )
                    {
                        parsed = false;
                        Logger.LogError(exp, string.Format("Failed to parse:{0}",cmdTxt));

                    }if (parsed)
                    {
                        if (Robj.ContainsKey("method") && Robj["method"] != null)
                        {
                            string method = Robj["method"].ToString();
                            if (String.Compare(method, "mining.notify") == 0)
                            {
                                MiningNotify(cmdTxt);
                            }
                            else
                            if (String.Compare(method, "mining.set_difficulty") == 0)
                            {
                                MiningSetDifficulty(cmdTxt);
                            }
                        }
                        else
                        {
                            if (
                                (Robj.ContainsKey("error") && Robj["error"] != null && Robj["error"].ToString().Trim().Length > 0)
                                    ||
                                (Robj.ContainsKey("result") && Robj["result"] != null && Robj["result"].ToString().Trim().Length > 0)
                            )
                            {

                                int id;
                                if (int.TryParse(Robj["id"].ToString(), out id))
                                {
                                    string method = AckMan.Acks.getACK(id);
                                    if (String.Compare(method, "mining.authorize") == 0)
                                    {
                                        miningauthorizeACK(cmdTxt);
                                    }
                                    else if (String.Compare(method, "mining.subscribe") == 0)
                                    {
                                        miningsubscribeACK(cmdTxt);

                                    }
                                    else if (String.Compare(method, "mining.submit") == 0)
                                    {
                                        miningsubmitACK(cmdTxt);
                                    }
                                }
                            }
                        }
                    }
                    // parse next 


                }
                 
            }
            
        }

        







    }
}
