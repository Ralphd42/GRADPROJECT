package com.example.andworker;

import java.io.IOException;
import java.io.OutputStream;
import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.util.Arrays;
import java.util.Date;

public class MineBatch {
    private MineThreadData _mtd;
    private OutputStream _nsm;
    boolean _done;

    public MineBatch(MineThreadData MineData, OutputStream outputStream) {
        _mtd = MineData;
        _nsm = outputStream;
        _done = false;
    }

    public void runJob() {
        double Hashcount = 0;
        byte[] Databyte = new byte[80];
        System.arraycopy(_mtd.thData, 0, Databyte, 0, 76);
        Date StartTime = new Date();
        try {
            byte[] crpt = new byte[32];
            MessageDigest digest = MessageDigest.getInstance("SHA-256");

            // Loop until done is set or we meet the target
            while (!_done) {
                //databyte is the data in a byte array
                // the below adds Nonce to byte as array
                Databyte[76] = (byte) (_mtd.Nonce >> 0);
                Databyte[77] = (byte) (_mtd.Nonce >> 8);
                Databyte[78] = (byte) (_mtd.Nonce >> 16);
                Databyte[79] = (byte) (_mtd.Nonce >> 24);
                crpt = digest.digest(Databyte);

                if (meetsTarget(crpt, _mtd.target))  // Did we meet the target?
                {
                    OnFoundNonce();
                } else {
                    _mtd.Nonce += _mtd.increment; // If not, increment the nonce and try again
                }
            }
        } catch (Exception ex) {
            ex.printStackTrace();
        }
    }

    public boolean meetsTarget(byte[] hash, byte[] target) {
        for (int i = hash.length - 1; i >= 0; i--) {
            if ((hash[i] & 0xff) > (target[i] & 0xff))
                return false;
            if ((hash[i] & 0xff) < (target[i] & 0xff))
                return true;
        }
        return false;
    }

    void OnFoundNonce() {
        //one notify all threads of done
        // send to controller that we won
        if (!_done) {
            _done = true;
            String msg = String.format("<F>%i#", _mtd.Nonce);
            byte[] bytes = msg.getBytes(StandardCharsets.US_ASCII);//   Encoding.ASCII.GetBytes(msg);
            if (_nsm != null) {
                try {
                    _nsm.write(bytes, 0, bytes.length);
                    _nsm.flush();
                    _nsm.close();
                } catch (IOException exp) {
                    exp.printStackTrace();
                }
            }
        }

    }
}



