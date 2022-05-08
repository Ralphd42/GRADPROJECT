using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
   public  interface IExcelLogger
    {
        public void  LogJsonFromPOOL(string msg);
        public void LogJsonTOPOOL  (String msg);
    }
}