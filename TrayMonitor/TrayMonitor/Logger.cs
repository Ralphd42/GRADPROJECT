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
        static object ouLock = new object();
        public static Logger Log {
            get {
                if (_logger == null)
                    lock (ouLock)
                    {
                        if (_logger == null)
                            _logger = new Logger();
                    }
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
        object LockObj = new object();
        public string LogError(Exception exp, string message)
        {
            Console.WriteLine(exp);
            StringBuilder msg = new StringBuilder();
            msg.AppendLine(message);
            msg.AppendLine(exp.ToString());
            if (exp.InnerException != null)
            {
                msg.AppendLine("=======================");
                msg.AppendLine(exp.InnerException.ToString());
            }
            lock (LockObj)
            {
                File.AppendAllText(LOGFILE, msg.ToString());
            }
            return msg.ToString();
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
