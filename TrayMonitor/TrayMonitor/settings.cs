using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace TrayMonitor
{
    internal class settings
    {
        public static string monitorIP {
            get {
                string retval = string.Empty;
                if (ConfigurationManager.AppSettings["monitorIP"] != null) {
                    retval = Convert.ToString(ConfigurationManager.AppSettings["monitorIP"]);
                }
                return retval;
            }
        }
        public static int monitorPort {
            get {
                int rv = -1;
                if (ConfigurationManager.AppSettings["monitorPort"] != null) {
                    if (!int.TryParse(ConfigurationManager.AppSettings["monitorPort"], out rv)) 
                    {
                        rv = 0;  
                    }
                }
                return rv;
            }
        }



       






    }
}
