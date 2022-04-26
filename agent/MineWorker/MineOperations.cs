using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MineTools;
using Settings = MineTools.Settings;
namespace MineWorker
{
    public class MineOperations
    {
        /// <summary>
        /// joinNetwork
        /// joins the distributed miner network
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns>true if succesful.   Errors written to log </returns>
        public bool joinNetwork(string clientName)
        {
            bool retval = false;
            byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPAddress conIPA = IPAddress.Parse(Program.ControllerIP);


                IPEndPoint remoteEP = new IPEndPoint(conIPA, Settings.WORKERMANAGERPORT);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);
                    string smsg = string.Format("<A>{0}#", clientName);
                    byte[] msg = Encoding.ASCII.GetBytes(smsg);
                    int bytesSent = sender.Send(msg);
                    int bytesRec = sender.Receive(bytes);
                    string returnmsg = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (returnmsg.Contains("<A>1#"))
                    {
                        retval = true;
                    }
                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                
                catch (Exception exp)
                {
                    if (_logger != null)
                    {
                        _logger.LogError(exp, "Error in joining network");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return retval;
        }
        private ILogger _logger;



        public MineOperations(ILogger lgger =null)
        {
            if (lgger != null)
            {
                _logger = lgger;
            }
        }

    }
}
