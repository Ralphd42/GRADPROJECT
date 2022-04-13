package com.example.andworker;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.net.ConnectivityManager;
import android.os.Bundle;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;

import java.net.Inet4Address;
import java.net.InetAddress;
import java.net.NetworkInterface;
import java.util.Enumeration;

public class MainActivity extends AppCompatActivity {
    public static final String EXTRA_MESSAGE = "com.example.myfirstapp.MESSAGE";
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        TextView txtIP = findViewById(R.id.txtVIP);
        IpCommI mt = new IpCommI (txtIP);
        runOnUiThread(mt);
    }
    public void sendMessage(View view) {
        Intent intent = new Intent(this, DisplayMessageActivity.class);
        //EditText editText = (EditText) findViewById(R.id.xxEdit);
        TextView txtIP = findViewById(R.id.txtVIP);
        String message = txtIP.getText().toString();
        intent.putExtra(EXTRA_MESSAGE, message);
        startActivity(intent);
    }
    class IpCommI  extends Thread
    {
        TextView Test;
        public IpCommI(TextView tv)
        {
            Test =tv;
        }

        public void run ()
        {
            try {
                StringBuilder sbIAddress = new StringBuilder();
                Enumeration e = NetworkInterface.getNetworkInterfaces();
                int cnt =1;
                if(e!=null)
                {
                    while(e.hasMoreElements())
                    {
                       // ConnectivityManager cn = new ConnectivityManager.NetworkCallback()  .bindProcessToNetwork(null);
                        NetworkInterface n = (NetworkInterface) e.nextElement();

                        Enumeration ee = n.getInetAddresses();
                        while (ee.hasMoreElements())
                        {
                            InetAddress i = (InetAddress) ee.nextElement();
                            if(!i.isLoopbackAddress()   && i instanceof Inet4Address)
                            {
                                sbIAddress.append(Integer.toString(cnt )+ ":"+
                                        i.getHostAddress()+"\n");
                                ++cnt;
                            }
                        }
                    }
                    Test.setText(sbIAddress.toString());
                }
                else
                {
                    Test.setText("NULL ADAPT");
                }
            }
            catch (Exception exp)
            {
                exp.printStackTrace();
                Test.setText("Other" + exp.toString());
            }
        }
    }

}