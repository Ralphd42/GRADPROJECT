using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    interface ILogger
    {
        public void LogMessage(string msg);
        public void LogMessage(Exception exp, String msg);
    }
}
