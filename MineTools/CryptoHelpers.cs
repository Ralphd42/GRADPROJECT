using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        /// <summary>
        /// generates hashes coinbase
        /// http://www.righto.com/2014/02/bitcoin-mining-hard-way-algorithms.html
        /// coinb1, extranonce1, extranonce2, and coinb2    
        /// </summary>
        /// <param name="b1Coin"></param>
        /// <param name="b2Coin"></param>
        /// <param name="Extra"></param>
        public static   byte[]     CoinBaseHash(string b1Coin, string b2Coin, string extranonce1, string extranonce2)
        {
            StringBuilder sbCB = new StringBuilder();
            sbCB.Append(b1Coin);
            sbCB.Append(extranonce1);
            sbCB.Append(extranonce2);
            sbCB.Append(b2Coin);
            byte[] bArr1 = Convert.FromHexString(sbCB.ToString());
            SHA256 sha25Obj = SHA256.Create();
            sha25Obj.Initialize();
            var cb= sha25Obj.ComputeHash(bArr1);
            return sha25Obj.ComputeHash(cb);
            
        }
        
        public static string MerkleRoot(string Coinb1, string Coinb2, string ExtraNonce1, string ExtraNonce2, string[] MerkleNumbers)
        {
            byte[] cb;
            byte[] cb2 = CoinBaseHash(Coinb1, Coinb2, ExtraNonce1, ExtraNonce2);
            SHA256 sha25Obj = SHA256.Create();
            sha25Obj.Initialize();
            foreach (string s in MerkleNumbers)
            {
                cb = sha25Obj.ComputeHash(Convert.FromHexString(byteArrToString(cb2) + s));
                cb2 = sha25Obj.ComputeHash(cb);
            }
            string MerkleRoot = byteArrToString(ReverseByteArrayByFours(cb2));
            return MerkleRoot;
        }


        public static byte[] ReverseByteArrayByFours(byte[] byteArray)
        {
            byte temp;

            if (byteArray.Length % 4 != 0)
            {
                throw new ArgumentException(String.Format("The byte array length must be a multiple of 4"));
            }

            for (int index = 0; index < byteArray.Length; index += 4)
            {
                temp = byteArray[index];
                byteArray[index] = byteArray[index + 3];
                byteArray[index + 3] = byteArray[index + 2];
                byteArray[index + 2] = byteArray[index + 1];
                byteArray[index + 1] = byteArray[index + 3];
                byteArray[index + 3] = temp;
            }

            return byteArray;
        }

        public static byte [] GenerateTarget(int Difficulty)
        {


            Difficulty = int.MaxValue;
            byte[] ba = { 
                0x00, 0x00, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

             
             
            int n = ba[0];
            byte[] result = new byte[ba.Length];

            for (int i = 0; i < ba.Length; i++)
            {
                int r = n / Difficulty;
                result[i] = (byte)r;
                int x = n - r * Difficulty;
                n = (x << 8) + ba[i];
            }
            string tarasString = byteArrToString(result);
            Console.WriteLine("DIFFR:{0}  |Target:{1}|",Difficulty, tarasString);
            Array.Reverse((Array)result);
            tarasString = byteArrToString(result);
            Console.WriteLine("DIFF :{0}  |Target:{1}|",Difficulty, tarasString);



            return result;
        }
    }
}
