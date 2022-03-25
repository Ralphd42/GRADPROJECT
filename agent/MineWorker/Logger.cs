﻿using System;
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
            get {
                return @"C:\logs\minerlog.log";
            }
        }

        public object LogLock = new object();
            


        public void  LogError(Exception exp, string message)
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendLine(message);
            msg.AppendLine(exp.Message);
            if (exp.InnerException != null)
            {
                msg.AppendLine(exp.InnerException.Message);
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
