using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTools
{
    public class CryptoHelpers
    {
        public static string byteArrToString(byte [] bArr)
        {
            string hStr = BitConverter.ToString(bArr);
            hStr = hStr.Replace("-", "");
            return hStr;

        }
    }
}
