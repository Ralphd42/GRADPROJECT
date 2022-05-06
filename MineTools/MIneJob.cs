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
        
        public string JobDifficulty;
        
        public string NetTime;
        
        public bool clear;
        public string MerkleRoot;
        public byte[] target;
        public string GenData
        {
            get
            {
                MerkleRoot = CryptoHelpers.MerkleRoot(
                    CoinPre,
                    CoinFollow, 
                    ExtraNONCE1,
                   ExtraNONCE2, Merk);
                return Ver + PrevHash + MerkleRoot + NetTime + JobDifficulty;
            }
        }
        public string Data;
        public int NONCE;
        public string ExtraNONCE1;
        public string ExtraNONCE2;

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
