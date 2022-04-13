package com.example.andworker;

import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.NotificationCompat;
import androidx.core.app.NotificationManagerCompat;

import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.os.StrictMode;
import android.widget.TextView;

import com.google.gson.Gson;

import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.PrintWriter;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;

public class DisplayMessageActivity extends AppCompatActivity {
    ServerSocket serverSocket;
    public final String IPAddress = "192.168.1.28";
    public final int WMPort = 8005;
    public final int JOBPort = 8005;
    boolean running =false;
    public final String CHANNEL_ID = "AACCDD";
    TextView textView;
    private Socket client;// = new Socket(IPAddress, WMPort);
    private OutputStream outS = null;
    TextView StatusView;
    private PrintWriter pw;
    private InputStream is;
    private InputStreamReader ISReader;
    private AppCompatActivity theActivity;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().permitAll().build();
        StrictMode.setThreadPolicy(policy);

        setContentView(R.layout.activity_display_message);
        // Get the Intent that started this activity and extract the string
        Intent intent = getIntent();
        String message = intent.getStringExtra(MainActivity.EXTRA_MESSAGE);

        // Capture the layout's TextView and set the string as its text
        StatusView = findViewById(R.id.textView);
        StatusView.setText(message);
        theActivity = this;
        runOnUiThread(new JoinNetwork());
        Set_Channel();

    }
    public void Set_Channel()
    {
        final String description ="bbb";
        final CharSequence name ="AAA";
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            int importance = NotificationManager.IMPORTANCE_DEFAULT;
            NotificationChannel channel = new NotificationChannel(CHANNEL_ID, name, importance);
            channel.setDescription(description);
            // Register the channel with the system; you can't change the importance
            // or other notification behaviors after this
            NotificationManager notificationManager = getSystemService(NotificationManager.class);
            notificationManager.createNotificationChannel(channel);
        }
    }
    public void Simple_Notification(String Msg)
    {
        Simple_Notification(Msg,Msg);

    }

    public void Simple_Notification(String LngMsg, String SrtMssg)
    {
        NotificationCompat.Builder builder = new NotificationCompat.Builder(theActivity, CHANNEL_ID)
                .setSmallIcon(R.drawable.ic_launcher_background)
                .setContentTitle(SrtMssg)
                .setContentText(LngMsg)
                .setStyle(new NotificationCompat.BigTextStyle()
                        .bigText(LngMsg))
                .setPriority(NotificationCompat.PRIORITY_DEFAULT);
        NotificationManagerCompat notificationManager =
                NotificationManagerCompat.from(theActivity);
        notificationManager.notify(5, builder.build());


    }



    public class JoinNetwork implements Runnable {


        @Override
        public void run() {

            try {
                if( StatusView !=null) {
                    StatusView.setText("1");
                }
                InetAddress address2
                        = InetAddress.getByName(IPAddress);
                client = new Socket(IPAddress, WMPort);

                outS = client.getOutputStream();
                pw = new PrintWriter(outS, true);
                String JoinDistProc = String.format("<A>%s#", "TEST ANDROID");
                pw.print(JoinDistProc);
                pw.flush();
                is = client.getInputStream();
                ISReader = new InputStreamReader(is);
                char input = (char) ISReader.read();
                String StrtaskID = "";
                while (input != '#') {
                    StrtaskID += input;
                    input = (char) ISReader.read();
                }
                if( StatusView !=null) {
                    StatusView.setText("ADDED : " + StrtaskID);
                }
                Simple_Notification(StrtaskID);
                boolean running =true;
            } catch (Exception exp) {
                exp.printStackTrace();
                String msg = "**Error join network \n";
                msg += exp.getMessage();
                msg += exp.toString();
                Simple_Notification("ERROR");
                if (StatusView != null) {
                    StatusView.setText(msg);
                }else{
                    Simple_Notification(msg);
                }
            }
        }
        Socket socket;
        class Listner implements  Runnable{
            @Override
            public void run() {
                try{
                    serverSocket = new ServerSocket(8005);
                    while(running) {
                        socket = serverSocket.accept();
                        InputStream ins =socket.getInputStream();
                        InputStreamReader inRd = new InputStreamReader(ins);
                        char in1 =(char)inRd.read();
                        StringBuilder jsonB =new StringBuilder();
                        while(in1!='}')
                        {
                            jsonB.append(in1);
                            in1 =(char)inRd.read();
                        }
                        Gson gson = new Gson();
                        MineThreadData mtd = gson.fromJson(jsonB.toString(), MineThreadData.class);
                        OutputStream outputStream = client.getOutputStream();
                    }

                }catch(IOException  ioe)
                {



                }
            }
        }





    }


}
