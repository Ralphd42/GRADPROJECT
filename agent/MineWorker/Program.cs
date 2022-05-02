using System;
using Microsoft.Extensions.Configuration;
using System.IO;
namespace MineWorker
{
    class Program
    {
        public static MineOperations mi;
        static void Main(string[] args)
        {
            welcome();
            Logger lg = new Logger();
            try
            {
                mi = new MineOperations(lg);
                bool joined = mi.joinNetwork(Program.AgentName);
                if (joined)
                {
                    lg.LogMessage("Joined Network");
                    JobListner jl = new JobListner(lg);
                    jl.ListenToController();
                }
                else
                {
                    lg.LogMessage("Failed to join NETWORK");
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error in agent Exiting");
                lg.LogError(exp, "ERROR in agent");
            }
        }

        #region UIFuncts
        
        static void welcome()
        {
            Console.WriteLine("Bitcoin worker starting");
        }
        
        #endregion
        #region Settings
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

        public static bool LogToConsole
        {
            get
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("params.json", optional: true, reloadOnChange: true);
                var rv = builder.Build().GetSection("Parameter").GetSection("LogToConsole").Value;
                bool retval = false;
                if (!bool.TryParse(rv, out retval))
                {
                    retval = false;
                }
                return retval;
            }
        }

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

        public static String ControllerIP
        {
            get
            {
                return Params.GetSection("ControllerIP").Value;
            }
        }

        public static String AgentName
        {
            get
            {
                return Params.GetSection("AgentName").Value;
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

        public static int ThreadCount
        {
            get
            {
                int rv;
                var wmp = Params.GetSection("ThreadCount").Value;
                if (int.TryParse(wmp, out rv))
                {
                    return rv;
                }
                return 1;
            }
        }
        
        public static string LogFile
        {
            get
            {
                var lf = Params.GetSection("LOGFILE").Value;
                return lf;
            }
        }
        #endregion
    }
}
