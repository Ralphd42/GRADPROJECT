package com.example.andworker;

import java.io.OutputStream;
import java.util.Arrays;

public class MineBatch {
    private MineThreadData _mtd;
    private OutputStream _outs;
    boolean     _done ;
    public MineBatch(MineThreadData MineData, OutputStream outputStream)
    {
        _mtd=MineData;
        _outs=outputStream;
        _done=false;
    }
    public void runJob()
    {
        double Hashcount = 0;

        byte[] Databyte = Arrays.copyOf(_mtd.thData, 76);
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
                ScryptResult = new byte[8];// Replicon.Cryptography.SCrypt.SCrypt.DeriveKey(Databyte, Databyte, 1024, 1, 1, 32);
                Hashcount++;
                if (meetsTarget(ScryptResult, _job.target))  // Did we meet the target?
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
            Console.WriteLine(ex);
        }
    }



}
