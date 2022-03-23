using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTools
{
    public class Settings
    {
        public static int JOBPORT
        {
            get
            {
                return 13001;
            }
        }

        public static int WORKERMANAGERPORT
        {
            get
            {
                return 13002;
            }
        }
        public static string CONTROLLERIPV4
        {
            get
            {
                return "127.0.0.1";
            }
        
        }


    }
}
