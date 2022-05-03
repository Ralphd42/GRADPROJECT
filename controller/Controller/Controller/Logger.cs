using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineTools;
using System.IO;
namespace Controller
{
    public class Logger : MineTools.ILogger
    {
        public static string LOGFILE
        {
            get {
                return Program.LogFile;
            }
        }

        public object LogLock = new object();
        /// <summary>
        /// Generic error handler.  Will handle all types of errors
        /// </summary>
        /// <param name="exp">the is an exception object</param>
        /// <param name="message">this is the message</param>
        public void  LogError(Exception exp, string message)
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendLine(message);
            msg.AppendLine(exp.Message);
            if (exp.InnerException != null)
            {
                msg.AppendLine(exp.InnerException.Message);
            }

            string specMsg = string.Empty;
            if (exp is ArgumentNullException)
            {
                specMsg = (exp as ArgumentException).Message;

            }
            else if (exp is System.Net.Sockets.SocketException)
            {
                specMsg = (exp as System.Net.Sockets.SocketException).Message;

            }
            else if(exp is System.IO.IOException)
            { 
                specMsg = (exp as System.Net.Sockets.SocketException).Message;

            }
            msg.AppendLine(specMsg);
            lock (LogLock)
            {
                using StreamWriter file = new(LOGFILE, append: true);
                {
                    file.Write(msg);
                }
            }
            Console.WriteLine(msg);
        }

        public void  LogMessage(string Message)
        {
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
