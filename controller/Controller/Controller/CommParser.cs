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



    }
}
