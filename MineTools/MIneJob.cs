using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineTools
{
    public class MineJob
    {
        public string ID;

        public string PrevHash;
        
        public string CoinPre;
        
        public string CoinFollow;
        
        public string[] Merk;
        
        public string Ver;
        
        public string Difficulty;
        
        public string NetTime;
        
        public bool clear;
        public string MerkleRoot;
        public byte[] target;
        string Data
        {
            get
            {
                return Ver + PrevHash + MerkleRoot + NetTime + Difficulty;
            }
        }
    }
    public class MineThreadData
    {
        public string id;
        public int numToRun;
        public byte[] thData;
        public byte[] target;
        public uint Nonce;
        public uint increment;
    }
}
