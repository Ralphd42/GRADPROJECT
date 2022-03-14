using System;

namespace MineTools
{
    public class Miner
    {
        private bool _done;  // this will allow job to stop
        public void KillProc()
        {
            _done = false;
            // must do notifications to all
        }
        private MineThreadData _job;
        public Miner(MineThreadData mtd)
        {
            _done = false;
            _job = mtd;

        }
        public void sendFinalNonce()
        {
            KillProc();
            if (!_done)
            { 
                // need to kill all procs in this and send back to controller
                // send nonce()
            }

        }


        public void runJob()
        {
            double Hashcount = 0;
            byte[] Databyte = new byte[80];
            Array.Copy(_job.thData, 0, Databyte, 0, 76);
            DateTime StartTime = DateTime.Now;
            try
            {
                byte[] ScryptResult = new byte[32];

                // Loop until done is set or we meet the target
                while (!_done)
                {
                    Databyte[76] = (byte)(_job.Nonce  >> 0);
                    Databyte[77] = (byte)(_job.Nonce >> 8);
                    Databyte[78] = (byte)(_job.Nonce >> 16);
                    Databyte[79] = (byte)(_job.Nonce >> 24);
                    ScryptResult = null; //Replicon.Cryptography.SCrypt.SCrypt.DeriveKey(Databyte, Databyte, 1024, 1, 1, 32);
                    Hashcount++;
                    if (meetsTarget(ScryptResult, _job.target))  // Did we meet the target?
                    {
                        sendFinalNonce();
                    }
                    else
                    {
                        _job.Nonce += _job.increment; // If not, increment the nonce and try again
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public bool meetsTarget(byte[] hash, byte[] target)
        {
            for (int i = hash.Length - 1; i >= 0; i--)
            {
                if ((hash[i] & 0xff) > (target[i] & 0xff))
                    return false;
                if ((hash[i] & 0xff) < (target[i] & 0xff))
                    return true;
            }
            return false;
        }






    }
}
