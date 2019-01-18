using UnityEngine;                        // These are the librarys being used
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

public class tesy : MonoBehaviour
{

    bool socketReady = false;                // global variables are setup here
    TcpClient mySocket;
    public NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;
    public String Host = "192.168.43.157";
    public Int32 Port = 12345;
    public bool is_left = true;


    void Start()
    {
        setupSocket();                        // setup the server connection when the program starts
        Thread thread = new Thread(new ThreadStart(read));
        thread.Start();
    }

    // Update is called once per frame
    void Update()
    {

        read();
    }

    void read()
    {
        Debug.Log("enter read");
        // if new data is recieved from Arduino
        
        {
            try
            {
                if (theStream.DataAvailable)
                {
                    Debug.Log("data available");
                    string recievedData = readSocket();            // write it to a string
                    if (recievedData == "1")
                        is_left = true;
                    else
                        is_left = false;

                    Debug.Log(recievedData);
                }
            }
            
            catch (TimeoutException)
            {
                // Do nothing, the loop will be reset
                Debug.Log("no input");
            }
            Debug.Log("in read");
        }
    }

    public void setupSocket()
    {                            // Socket setup here
        try
        {
            mySocket = new TcpClient(Host, Port);
            theStream = mySocket.GetStream();
            theWriter = new StreamWriter(theStream);
            theReader = new StreamReader(theStream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error:" + e);                // catch any exceptions
        }
    }

    public void writeSocket(string theLine)
    {            // function to write data out
        if (!socketReady)
            return;
        String tmpString = theLine;
        theWriter.Write(tmpString);
        theWriter.Flush();


    }

    public String readSocket()
    {                        // function to read data in
        if (!socketReady)
            return "";
        if (theStream.DataAvailable)
            return theReader.ReadLine();
        return "NoData";
    }

    public void closeSocket()
    {                            // function to close the socket
        if (!socketReady)
            return;
        theWriter.Close();
        theReader.Close();
        mySocket.Close();
        socketReady = false;
    }

    public void maintainConnection()
    {                    // function to maintain the connection (not sure why! but Im sure it will become a solution to a problem at somestage)
        if (!theStream.CanRead)
        {
            setupSocket();
        }
    }


} // end class ClientSocket