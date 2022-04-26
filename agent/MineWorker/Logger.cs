using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineTools;
using System.IO;
namespace MineWorker
{
    public class Logger : ILogger
    {
        public static string LOGFILE
        {
            get 
            {
                return Program.LogFile;
            }
        }

        public object LogLock = new object();
        public void  LogError(Exception exp, string message)
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendLine(message);
            msg.AppendLine(exp.Message);
            string specMsg= string.Empty;
            if (exp is ArgumentNullException) 
            {
                specMsg = (exp as ArgumentException).Message;

            }else if (exp is System.Net.Sockets.SocketException) 
            { 
                specMsg = (exp as System.Net.Sockets.SocketException).Message;
            }
            if (exp.InnerException != null)
            {
                msg.AppendLine(exp.InnerException.Message);
            }
            msg.AppendLine(specMsg);
            if( Program.LogToConsole)
            {
                Console.WriteLine(specMsg);

            }
            lock (LogLock)
            {
                using StreamWriter file = new(LOGFILE, append: true);
                {
                    file.Write(msg);
                }
            }
        }

        public void  LogMessage(string Message)
        {
            if( Program.LogToConsole)
            {
                Console.WriteLine(Message);
            }
            lock (LogLock)
            {
                using StreamWriter file = new(LOGFILE, append: true);
                {
                    file.WriteLine(Message);
                }
            }
        }

        public void  LogWIthDate(string Message)
        {
            Message = string.Format("{0}:{1}", DateTime.Now, Message);
            if( Program.LogToConsole)
            {
                Console.WriteLine(Message);
            }
            lock (LogLock)
            {
                using StreamWriter file = new(LOGFILE, append: true);
                {
                    file.WriteLine(Message);
                }
            }
        }
    }
}
