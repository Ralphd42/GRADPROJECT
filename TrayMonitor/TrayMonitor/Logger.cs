using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace TrayMonitor
{
    internal class Logger : ILogger
    {


        private Logger()
        { }
        private static Logger _logger;
        public static Logger Log {
            get {
                if (_logger == null)
                    _logger = new Logger();
                return _logger;

            }
        }




        public static string LOGFILE {
            get {
                string retval = string.Empty;
                if (ConfigurationManager.AppSettings["LOGFILE"] != null) {
                    retval = ConfigurationManager.AppSettings["LOGFILE"];
                }
                return retval;
            }
        }



        public void LogError(Exception exp, string message)
        {
            Console.WriteLine(exp);
        }

        public void LogMessage(string Message)
        {
            throw new NotImplementedException();
        }

        public void LogWIthDate(string Message)
        {
            throw new NotImplementedException();
        }
    }
}
