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
import com.google.gson.GsonBuilder;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonPrimitive;
import com.google.gson.JsonSerializer;

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
    //constants
    public final String IPAddress = "192.168.1.12";
    public final String ClientName ="TEST ANDROID";
    public final int WMPort = 8005;
    public final int JOBPort = 8006;
    boolean running = false;
    public final String CHANNEL_ID = "AACCDD";
    TextView StatusView;
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
        // runOnUiThread(new JoinNetwork());
        new Thread(new JoinNetwork()).start();
        Set_Channel();

    }

    public void Set_Channel() {
        final String description = "bbb";
        final CharSequence name = "AAA";
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

    public void Simple_Notification(String Msg) {
        Simple_Notification(Msg, Msg);

    }

    public void Simple_Notification(String LngMsg, String SrtMssg) {
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
                if (StatusView != null) {
                    StatusView.post(new Runnable() {
                        @Override
                        public void run() {
                            String Sta = String.format("Trying to connect to : %s",IPAddress);

                            StatusView.setText(Sta);
                        }
                    });
                }
                InetAddress address2
                        = InetAddress.getByName(IPAddress);

                Socket client = new Socket(IPAddress, WMPort);

                OutputStream outS = client.getOutputStream();
                PrintWriter pw = new PrintWriter(outS, true);
                String JoinDistProc = String.format("<A>%s#",ClientName );
                pw.print(JoinDistProc);
                pw.flush();
                InputStream is = client.getInputStream();
                InputStreamReader ISReader = new InputStreamReader(is);
                char input = (char) ISReader.read();
                String StrtaskID = "";
                while (input != '#') {
                    StrtaskID += input;
                    input = (char) ISReader.read();
                }

                if (StatusView != null) {
                    String tskID = StrtaskID;
                    StatusView.post(new Runnable() {
                        @Override
                        public void run() {
                            String Sta = String.format("ADDED : %s", tskID);

                            StatusView.setText(Sta);
                        }
                    });
                }




                Simple_Notification(StrtaskID);
                running = true;

                serverSocket = new ServerSocket(JOBPort);
                while (running) {
                    Thread.yield();
                    Thread.sleep(0);
                    Thread.yield();
                    runList();
                    Thread.yield();
                    Thread.sleep(1);
                    Thread.yield();
                }

                //(new Thread(new Listner())).start();


            } catch (Exception exp) {
                exp.printStackTrace();
                String msg = "**Error join network \n";
                msg += exp.getMessage();
                msg += exp.toString();
                Simple_Notification("ERROR");
                if (StatusView != null) {
                    StatusView.setText(msg);
                } else {
                    Simple_Notification(msg);
                }
            }
        }

        public void runList() {
            String JSN = "";
            try {
                //serverSocket = new ServerSocket(JOBPort);
                //while(running) {
                Thread.sleep(1000);
                Thread.yield();
                socket = serverSocket.accept();
                InputStream ins = socket.getInputStream();
                InputStreamReader inRd = new InputStreamReader(ins);
                char in1 = (char) inRd.read();
                StringBuilder jsonB = new StringBuilder();
                while (in1 != '}') {
                    jsonB.append(in1);
                    in1 = (char) inRd.read();
                }
                jsonB.append("}");
                //Gson gson = new Gson();
                Gson gson = new GsonBuilder().setDateFormat("yyyy-MM-dd HH:mm:ss")
                        .registerTypeAdapter(byte[].class, (JsonSerializer<byte[]>) (src, typeOfSrc, context) -> new JsonPrimitive(new String(src)))
                        .registerTypeAdapter(byte[].class, (JsonDeserializer<byte[]>) (json, typeOfT, context) -> json == null ? null : json.getAsString() == null ? null : json.getAsString().getBytes())
                        .create();
                JSN = jsonB.toString();
                MineThreadData mtd = gson.fromJson(JSN, MineThreadData.class);
                OutputStream outputStream = socket.getOutputStream();
                // need to call runner
                MineBatch mb = new MineBatch(mtd, outputStream);
                mb.runJob();

                if (StatusView != null) {
                    StatusView.post(new Runnable() {
                        @Override
                        public void run() {
                            StatusView.setText("Sent Nonce To Network");
                        }
                    });
                }
                Simple_Notification("DONE");
                //  }

            } catch (IOException ioe) {
                System.out.print(ioe);
                if (StatusView != null) {
                    StatusView.setText("IOException : " + ioe.getMessage());
                }

            } catch (Exception exp) {
                System.out.print(exp);
                if (StatusView != null) {
                    StatusView.setText("JSON :" + JSN + "| Exception : " + exp.getMessage());

                }
                exp.printStackTrace();

            }
        }

        Socket socket;

        class Listner implements Runnable {
            public String JSN = "";


            @Override
            public void run() {
                try {
                    serverSocket = new ServerSocket(JOBPort);
                    //while(running) {
                    Thread.sleep(1000);
                    socket = serverSocket.accept();
                    InputStream ins = socket.getInputStream();
                    InputStreamReader inRd = new InputStreamReader(ins);
                    char in1 = (char) inRd.read();
                    StringBuilder jsonB = new StringBuilder();
                    while (in1 != '}') {
                        jsonB.append(in1);
                        in1 = (char) inRd.read();
                    }
                    jsonB.append("}");
                    //Gson gson = new Gson();
                    Gson gson = new GsonBuilder().setDateFormat("yyyy-MM-dd HH:mm:ss")
                            .registerTypeAdapter(byte[].class, (JsonSerializer<byte[]>) (src, typeOfSrc, context) -> new JsonPrimitive(new String(src)))
                            .registerTypeAdapter(byte[].class, (JsonDeserializer<byte[]>) (json, typeOfT, context) -> json == null ? null : json.getAsString() == null ? null : json.getAsString().getBytes())
                            .create();
                    JSN = jsonB.toString();
                    MineThreadData mtd = gson.fromJson(JSN, MineThreadData.class);
                    OutputStream outputStream = socket.getOutputStream();
                    // need to call runner
                    MineBatch mb = new MineBatch(mtd, outputStream);
                    mb.runJob();
                    if (StatusView != null) {
                        //running=false;
                        StatusView.setText("FOUND RETURNED");
                    }
                    Simple_Notification("DONE");
                    //  }

                } catch (IOException ioe) {
                    System.out.print(ioe);
                    if (StatusView != null) {
                        StatusView.setText("IOException : " + ioe.getMessage());
                    }

                } catch (Exception exp) {
                    System.out.print(exp);
                    if (StatusView != null) {
                        StatusView.setText("JSON :" + JSN + "| Exception : " + exp.getMessage());

                    }
                    exp.printStackTrace();

                }
            }
        }


    }


}
