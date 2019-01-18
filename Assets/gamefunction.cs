using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class gamefunction : MonoBehaviour
{

    public GameObject Left, Right, Up, Down , Hit;
    public float time; //宣告浮點數，名稱time
    public int counter = 0;
    public SerialPort sp;
    int baudRate = 115200;
    int readTimeOut = 100;
    int bufferSize = 32; // Device sends 32 bytes per packet
    bool programActive = true;
    bool play_mode = false;
    public enum Swing { Reset, Up, Down, Left, Right };
    Thread thread;
    int arrow_type;
    float arrow_pos;
    Vector3 pos;


    public class Global
    {
        public static Swing chock = Swing.Up;
    }

    void Start()
    {

        Global.chock = (Swing)(0);

        Global.chock = (Swing)(int.Parse("5"));
        Debug.Log(Global.chock + " chock is");

        /*string data;
        // Start is called before the first frame update
        
            Debug.Log("enter tesy");
            Console.WriteLine("Port: ");
            string _port = Console.ReadLine();
            byte[] buffer = new Byte[1024];
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), int.Parse(_port));
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    Socket socket = listener.Accept();
                    data = null;

                    while (true)
                    {
                        int bytesRec = socket.Receive(buffer);
                        data += Encoding.ASCII.GetString(buffer, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    Console.WriteLine("Text received : {0}", data);
                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    socket.Send(msg);
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        */

    }

    void ProcessData()
    {
        Byte[] buffer = new Byte[bufferSize];
        string input_data;
        int bytesRead = 0;
        Debug.Log("Thread started");
        while (programActive)
        {
            Debug.Log("before try");
            try
            {
                Debug.Log("enter try");
                // Attempt to read data from the BT device
                // - will throw an exception if no data is received within the timeout period
                bytesRead = sp.Read(buffer, 0, bufferSize);
                input_data = sp.ReadLine();
                // Use the appropriate SerialPort read method for your BT device e.g. ReadLine(..) for newline terminated packets
                if (bytesRead > 0)
                {
                    // Do something with the data in the buffer
                    Debug.Log("read success");
                    Debug.Log(input_data);
                    programActive = false;
                    if (input_data != "0" )
                    {
                        print("reset is done");
                        play_mode = true;
                        pos = new Vector3(0, -3.2f, 0);
                        sp.Close();
                        Debug.Log(sp.IsOpen);
                        time = 0;
                        while(time > 10f)
                        {
                            time += Time.deltaTime;
                            Debug.Log(time);
                        }
                        time = 0;
                        sp.Open();
                        Debug.Log(sp.IsOpen);
                        Instantiate(Hit, pos, transform.rotation);
                       
                    }
                    
                }
            }
            catch (TimeoutException)
            {
                // Do nothing, the loop will be reset
                Debug.Log("wrong input");
            }
        }
        Debug.Log("Thread stopped");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
