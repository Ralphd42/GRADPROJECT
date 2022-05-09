using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controller
{
    class CommParser
    {
        /// <summary>
        /// returns string without head(command) and terminator tail
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string removeHT(string data)
        {
            data = data.Trim();
            data = data.Substring(3);
            data = data.Replace("#", "");
            return data;
        }

        public static Worker ParseWorker(string data, Worker worker)
        {
            data = removeHT(data);
            string[] arrdt = data.Split(":");
            worker.MachineName = arrdt[1];
            int numthreads =1;
            if ( 
                arrdt.Length<2 ||
                !int.TryParse(arrdt[0], out numthreads    )
                )
            {
                numthreads = 1;
            }
            worker.Processors = numthreads;
            return worker;
        }
    }
}
