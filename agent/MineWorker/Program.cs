using System;
using Microsoft.Extensions.Configuration;
using System.IO;
namespace MineWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger lg = new Logger();
            try
            {
                MineOperations mi = new MineOperations(lg);
                bool joined = mi.joinNetwork(args[0]);
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
            }catch (Exception exp)
            {
                lg.LogError(exp, "ERROR in agent");


            }
        }

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

        public static IConfigurationSection Params{ 
        get{
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

        public static int WorkerManagerPort
        {
            get
            {
                int rv =-1;
                var wmp = Params.GetSection("WorkerManagerPort").Value;
                if(!int.TryParse(wmp,out rv))
                {
                    rv=-1;
                }
                return rv;
            }
        }







        
        public static string LogFile
        {
            get
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Params.json", optional: true, reloadOnChange: true);
                var lf = builder.Build().GetSection("Parameter").GetSection("LOGFILE").Value;
                return lf;
            }
        }
        #endregion




    }
}
