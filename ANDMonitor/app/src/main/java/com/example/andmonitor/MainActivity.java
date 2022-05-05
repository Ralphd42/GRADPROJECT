package com.example.andmonitor;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.TextView;

import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.PrintWriter;
import java.net.InetAddress;
import java.net.Socket;

public class MainActivity extends AppCompatActivity {
    // need way to make these settings variables
    public final String MonitorIP ="192.168.1.12";
    public final int MonitorPort =13004;

    TextView txtOutput;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        txtOutput = findViewById(R.id.txtOutput);
    }
     public void getStauts(View view)
     {

         txtOutput.setText("Attempting to get Agent Status");
         new Thread(new KillNetwork("<W>#")).start();

     }
    public void getJobs(View view)
    {

        txtOutput.setText("Attempting to get JobCount");
        new Thread(new KillNetwork("<J>#")).start();

    }


    public void sendMessage(View view) {
        //Intent intent = new Intent(this, DisplayMessageActivity.class);
        //EditText editText = (EditText) findViewById(R.id.xxEdit);

        txtOutput.setText("Attempting to Kill Process");
        new Thread(new KillNetwork("<K>#")).start();
    }


    //kill code
    public class KillNetwork implements Runnable {
        private String  _cmdTxt;
        public KillNetwork( String cmdTxt)
        {
            if(cmdTxt==null  || cmdTxt.length()<1)
            {
                _cmdTxt ="<K>#";

            }else
            {
                _cmdTxt =cmdTxt;
            }

        }

        @Override
        public void run() {
            try {
                if (txtOutput != null) {
                    txtOutput.post(new Runnable() {
                        @Override
                        public void run() {
                            String Sta = String.format("Trying to connect to : %s",MonitorIP);
                            txtOutput.setText(Sta);
                        }
                    });
                }
                InetAddress address2
                        = InetAddress.getByName(MonitorIP);
                Socket client = new Socket(MonitorIP, MonitorPort);
                OutputStream outS = client.getOutputStream();
                PrintWriter pw = new PrintWriter(outS, true);
                String JoinDistProc = _cmdTxt;
                pw.print(JoinDistProc);
                pw.flush();
                InputStream is = client.getInputStream();
                InputStreamReader ISReader = new InputStreamReader(is);
                char input = (char) ISReader.read();
                String returnData = "";
                while (input != '#') {
                    returnData += input;
                    input = (char) ISReader.read();
                }
                if (txtOutput != null) {
                    String tskID = returnData;
                    txtOutput.post(new Runnable() {
                        @Override
                        public void run() {
                            String Sta = tskID;
                            txtOutput.setText(Sta);
                        }
                    });
                }
            } catch (Exception exp) {
                exp.printStackTrace();
                String msg = "**Error join network \n";
                msg += exp.getMessage();
                msg += exp.toString();
                if (txtOutput != null) {
                    String tskID = msg;
                    txtOutput.post(new Runnable() {
                        @Override
                        public void run() {
                            String Sta = tskID;
                            txtOutput.setText(Sta);
                        }
                    });
                }
            }
        }


    }


}