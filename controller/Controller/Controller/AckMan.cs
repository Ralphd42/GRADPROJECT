using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MineTools;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Concurrent;
namespace Controller
{

    /// <summary>
    ///Manages ackknoledegments of jobs
    /// </summary>
    public class AckMan
    {
        private Dictionary<int,string> _acks;
        public void AddAck(int ack, string command)
        {
            if( _acks==null)
            {
                _acks= new Dictionary<int, string>();

            }
            _acks[ack] = command;
        }
        public void removeAck(int ack)
        {
            if( _acks!=null)
            { 
                _acks.Remove(ack);
            }
 
        }

        public void clear()
        {
            if(_acks!=null)
            {
                _acks.Clear();

            }

        }
        public int numAcks()
        {
            int retval =0;
            if( _acks!=null
            ){
                retval =_acks.Count();

            }
            return retval;
        }
        public string getACK(int ackID     )
        {
            string retval = string.Empty;
            if( _acks!=null)
            {
                if(_acks.ContainsKey(ackID))
                {
                    retval = _acks[ackID];
                }
            }
            return retval;
        }
    }
}