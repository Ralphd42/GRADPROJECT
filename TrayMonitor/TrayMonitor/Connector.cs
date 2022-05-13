using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

using System.Threading;
using System.Threading.Tasks;

using System.Net;
namespace TrayMonitor
{
    /// <summary>
    /// handles communications to monitor
    /// </summary>
    public class Connector
    {
        public async Task<String> SendCommand(String msg)
        {
            var tsk = await Task<String>.Run(() =>
            {
                string svr = String.Empty;
                try
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(msg);
                    IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress conIPA = IPAddress.Parse(settings.monitorIP);
                    TcpClient tcpClient = new TcpClient(AddressFamily.InterNetwork);
                    tcpClient.Connect(conIPA, settings.monitorPort);
                    var _nsm = tcpClient.GetStream();
                    _nsm.Write(bytes, 0, bytes.Length);
                    _nsm.Flush();
                    StringBuilder objString = new StringBuilder();
                    string resp = String.Empty;
                    int bytesread = 0;
                    byte[] rb = new byte[1024];
                    while ((bytesread = _nsm.Read(rb)) > 0)
                    {
                        objString.Append(Encoding.ASCII.GetString(rb, 0, bytesread));
                        svr = objString.ToString();
                        svr = svr.Trim();
                        if (svr.Contains("#")) { break; }
                    }
                }
                catch (Exception exp)
                {
                    svr += Logger.Log.LogError(exp, String.Format("SendCommand:{0}", msg));
                    svr = "Error return check if server is running";
                }
                return svr;
            });
            return tsk;
        }

        public async Task<String> Kill( )
        {
            string msg = "<K>#";
            var tsk = await Task<String>.Run(() =>
            {
                string svr = String.Empty;
                try
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(msg);
                    IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress conIPA = IPAddress.Parse(settings.monitorIP);
                    TcpClient tcpClient = new TcpClient(AddressFamily.InterNetwork);
                    tcpClient.Connect(conIPA, settings.monitorPort);
                    var _nsm = tcpClient.GetStream();
                    _nsm.Write(bytes, 0, bytes.Length);
                    _nsm.Flush();
                    _nsm.Close();
                }
                catch (Exception exp)
                {
                    svr += Logger.Log.LogError(exp, String.Format("SendCommand:{0}", msg));
                    svr = "Error return check if server is running";
                }
                return svr;
            });
            return tsk;
        }


    }
}
