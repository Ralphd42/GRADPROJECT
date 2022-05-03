using System;

namespace MineTools
{
    public class Miner
    {
        const int maxHASH =   int.MaxValue;
        public event EventHandler<uint> foundNonce;
        private volatile bool _done;  // this will allow job to stop
        public void KillProc()
        {
            _done = true;
           
        }
        private MineThreadData _job;
        private ILogger _logger;
        public Miner(MineThreadData mtd, ILogger lgr =null)
        {
            _done = false;
            _job = mtd;
            if( lgr!=null)
            {
                _logger =lgr;

            }

        }
        public void OnFoundNonce()
        {   
            if (!_done)
            {
                foundNonce?.Invoke(this, _job.Nonce);
                _done = true;
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
                byte[] SHAHash = new byte[32];

                // Loop until done is set or we meet the target
                while (!_done)
                {
                    Databyte[76] = (byte)(_job.Nonce  >> 0);
                    Databyte[77] = (byte)(_job.Nonce >> 8);
                    Databyte[78] = (byte)(_job.Nonce >> 16);
                    Databyte[79] = (byte)(_job.Nonce >> 24);
                    SHAHash = CryptoHelpers.ReHash(Databyte);
                    Hashcount++;
                    if ( Hashcount>maxHASH ||   meetsTarget(SHAHash, _job.target))  // Did we meet the target?
                    {
                        OnFoundNonce();
                    }
                    else
                    {
                        _job.Nonce += _job.increment; // If not, increment the nonce and try again
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"runJob");
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
