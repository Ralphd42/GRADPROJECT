using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.IO;
namespace Controller
{
    class Program
    {
        public static bool _RUNNING = true;
        public static JobQueue MainJobQueue;
        public static PoolManager pm;
        public static Logger lgr;
        public static JobQueueWatcher jqe;
        public static WorkerManager wmt;



        static void Main(string[] args)
        {
            // TargetTester();
            // PoolConnTest();


            _RUNNING = true;

            


            showIP();

            MainJobQueue = new JobQueue();

            lgr = new Logger();
            // start waiting for Threads.
            wmt = new WorkerManager();
            Thread wmtThread = new Thread(new ThreadStart(wmt.AddWorkerThread));
            wmtThread.IsBackground = true;  // this thread must end when program hits end
                                            // there will be no joining
            wmtThread.Start();

            // wait until there are workers
            Console.WriteLine("waiting for {0} workers to join network", Program.MinWorkerCount);
            Task.Run(() => CommandLoop() );
            //while (wmt.Workers == null || wmt.Workers.Count < MinWorkerCount)
            //{
            //    Thread.Sleep(10000);
                //Console.Write(".");
            //}
            //Console.WriteLine("Workers joined waiting for job from network");
            //Start the queue worker thread.
            jqe = new JobQueueWatcher(MainJobQueue, wmt.Workers);
            Thread jqeThread = new Thread(new ThreadStart(jqe.WatchQueue));
            jqeThread.Start();
            ResponseLoop reLp = new ResponseLoop();
            new Thread(reLp.RunLoop).Start();



            pm = new PoolManager(MainJobQueue);
            monitor m = new monitor();
            new Thread(new ThreadStart(m.runMonitor)).Start();
            if (debug)
            {
                string text = System.IO.File.ReadAllText("TestData.log");

                pm.PoolFuncts.StratumReaderTester(text);
                Console.WriteLine(text);
                Thread.Sleep(1000);

                Console.Write("Adding a test task");
                Thread.Sleep(1000);
                //LoadTestData();
                Console.WriteLine("ADDED");
                /*
                    artificially add item to JobQueue

                */
                Thread.Sleep(1000 * 60);
                Console.WriteLine("KILLING");
                //jqe.killThreads();


            }
            else if (pm.startConnectionToPool())
            {
                /*
                    Need to build thread for listner to add jobs to the Queue
                */
                Thread PoolTh = new Thread(new ThreadStart(pm.StartListner));
                PoolTh.Start();
            }
            else
            {
                Console.Write("Failed to connect to Server Exiting.......");
                lgr.LogMessage("Failed to connect to Server.  Exiting......");
                _RUNNING = false;
                System.Environment.Exit(-1);

            }
            while (_RUNNING)
            {
                Thread.Sleep(0);
                CommandLoop();
            }
            //need to look for jobs
           
            //jqeThread.Join();



            //wmt.sendJob();
            //Console.WriteLine("Press button");
            //Console.Read();

            
        }
        #region USerInterface
        public static string CommandMessage()
        {
            Thread.Sleep(0);
            //String msg  = @"Enter a command: W:";
            StringBuilder msg = new StringBuilder();
            msg.AppendLine();
            msg.AppendLine("Enter a Command:");
            msg.AppendLine("Q - QUit(Exit)");
            msg.AppendLine("W - Show WOrkers");
            msg.AppendLine("S - Show status");
            msg.AppendLine();
            return msg.ToString();
        }
        public static void CommandLoop()
        { Thread.Sleep(0);
            while (_RUNNING)
            {
                //Console.Clear();
                Thread.Sleep(0);
                Console.WriteLine(CommandMessage());
                
                string msg = Console.ReadLine();
                Console.WriteLine("MESSAGE|{0}|",msg);
                msg = msg.Trim().ToUpper();
                char fc = msg[0];
                Console.WriteLine("fc|{0}|", fc);
                switch (fc)
                {
                    case 'K':
                    case 'Q':
                        KIllApp();
                        break;
                    case 'S':
                        dispStatus();
                        break;
                    default:
                        Console.WriteLine("Please enter valid command");
                        break;
                }
            }
        }
        public static void dispStatus()
        {
            wmt.ShowWorkers();
            jqe.ToString();
        }
        public static void KIllApp()
        {
            _RUNNING = false;  // this will give threads a chance to end on own
            Console.WriteLine("TERMINATING");
            wmt.ShutdownWorkers();
            Thread.Sleep(2000);
            Console.WriteLine("Terminating 2");
            //kill
            System.Environment.Exit(1);

        }

        /// <summary>
        /// This gives information about the system
        /// Spcifically it displays the ip address.  This is an easy way to get it for the clients(Agents)
        /// </summary>
        public static void showIP()
        {
            string IPAddress = "";
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    IPAddress = Convert.ToString(IP);
                }
            }
            Console.WriteLine("Bitcoin network miner starting");
            Console.WriteLine("HostName:{0} ", Hostname);
            Console.WriteLine("RUNNING ON IP ADDRESS {0} ", IPAddress);
            Console.WriteLine("Waiting for agents(workers) to join network");
        }
        #endregion
        #region Settings
        /*These are the settings they can probably be moved to separate file*/
        public static IConfigurationSection Params
        {
            get
            {
                var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("params.json", optional: true, reloadOnChange: true);
                return builder.Build().GetSection("Parameter");

            }
        }

        //PoolUser
        public static string PoolUser
        {
            get
            {
                return Params.GetSection("PoolUser").Value;
            }
        }




        public static int WorkerManagerPort
        {
            get
            {
                int rv = -1;
                var wmp = Params.GetSection("WorkerManagerPort").Value;
                if (!int.TryParse(wmp, out rv))
                {
                    rv = -1;
                }
                return rv;
            }
        }

        public static int MinWorkerCount
        {
            get
            {
                int rv = 1;
                var wmp = Params.GetSection("MinWorkerCount").Value;
                if (!int.TryParse(wmp, out rv))
                {
                    rv = 1;
                }
                return rv;
            }
        }


        public static int JobPort
        {
            get
            {
                int rv = -1;
                var wmp = Params.GetSection("JobPort").Value;
                if (!int.TryParse(wmp, out rv))
                {
                    rv = -1;
                }
                return rv;
            }
        }
        //JobRPort
        public static int JobRPort
        {
            get
            {
                int rv = -1;
                var wmp = Params.GetSection("JobRPort").Value;
                if (!int.TryParse(wmp, out rv))
                {
                    rv = -1;
                }
                return rv;
            }
        }

        public static int MonitorPort
        {
            get
            {
                int rv = -1;
                var wmp = Params.GetSection("MonitorPort").Value;
                if (!int.TryParse(wmp, out rv))
                {
                    rv = -1;
                }
                return rv;
            }
        }



        public static bool debug
        {
            get
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("params.json", optional: true, reloadOnChange: true);
                var debug = builder.Build().GetSection("Parameter").GetSection("debug").Value;
                bool retval = false;
                if (!bool.TryParse(debug, out retval))
                {
                    retval = false;
                }
                return retval;
            }
        }

        public static string LogFile
        {
            get
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("params.json", optional: true, reloadOnChange: true);
                var lf = builder.Build().GetSection("Parameter").GetSection("LOGFILE").Value;
                return lf;
            }
        }

        //JsonLog
        public static string JsonLog
        {
            get
            {
                return Params.GetSection("JsonLog").Value;
            }
        }




        #endregion

        public static void exitApp()
        {
            Console.WriteLine("Exiting Controller");
            System.Environment.Exit(-1);
        }
        #region TestFuncts
        /*
            These functions are for testing
            
        */
        public static void PoolConnTest()
        {
            PoolManager pm = new PoolManager(new JobQueue());
            pm.startConnectionToPool();
            Program.exitApp();
        }
        public static void LoadTestData()
        {
            //const int testdiff = 100;
            int difvl;
            if (int.TryParse("E9", System.Globalization.NumberStyles.HexNumber,
             CultureInfo.InvariantCulture, out difvl))
            {
                MineTools.MineJob mjtest = new MineTools.MineJob
                ()
                {
                    /*https://reference.cash/mining/stratum-protocol*/
                    /*
    0                   [“bf”, 
    1                   “4d16b6f85af6e2198f44ae2a6de67f78487ae5611b77c6c0440b921e00000000”,
    2 “01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff20020862062f503253482f04b8864e5008”,
    3 “072f736c7573682f000000000100f2052a010000001976a914d23fcdf86f7e756a64a7a9688ef9903327048ed988ac00000000”, 
    4 [],
    5 “00000002”, 
    6 “1c2ac4af”, 
    7 “504e86b9”, 
    8 false]
    */
                clear = false,
                    CoinPre =
    "072f736c7573682f000000000100f2052a010000001976a914d23fcdf86f7e756a64a7a9688ef9903327048ed988ac00000000",
                    CoinFollow = "01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff20020862062f503253482f04b8864e5008",
                    JobDifficulty = "3E9",
                    ID = "bf",
                    // Merk = aMerk,
                    NetTime = "504e86b9",
                    PrevHash = "4d16b6f85af6e2198f44ae2a6de67f78487ae5611b77c6c0440b921e00000000",
                    Ver = "00000002",
                    target = MineTools.CryptoHelpers.GenerateTarget(difvl)
                    };
                mjtest.Data = mjtest.GenData;
                Console.WriteLine("as INT: |{0}| ", difvl);
                MainJobQueue.AddJob(mjtest);
            }
            else
            {
                Console.WriteLine("FAILED");


            }
            /*MineTools.MineJob mjTest = new MineTools.MineJob() { }
            MainJobQueue.AddJob(
            new MineTools.MineJob(){ 
            }
            )*/
        }
        public static void TargetTester()
        {
            int[] testers = new int[] {
                1,2,3,4,5,6,7,
                100,1000,10000,1000000, int.MaxValue
            };
            foreach (var i in testers)
            {
                MineTools.CryptoHelpers.GenerateTarget(i);
            }
            Program.exitApp();
        }
        #endregion
    }
}
#region OLDCODE
#if UNUSEDCODE

if (false)
            {
                Console.WriteLine("Hello World!");
                WorkerManager wm = new WorkerManager();
                wm.Running = true;
                Thread backgroundThread = new Thread(new ThreadStart(wm.AddWorkerThread));
                // Start thread  
                backgroundThread.Start();
                backgroundThread.Join();
            }
#endif
#endregion