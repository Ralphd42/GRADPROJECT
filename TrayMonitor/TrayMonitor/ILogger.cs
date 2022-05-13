
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrayMonitor
{
    public interface ILogger
    {
        public void LogError(Exception exp, string message);
        public void LogMessage(string Message);
        public void LogWIthDate(string Message);


    }
}
