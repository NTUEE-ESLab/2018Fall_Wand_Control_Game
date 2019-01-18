using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Text;
using System.IO.Ports;
using System.Threading;

public class hit : MonoBehaviour
{
    // Start is called before the first frame update


    public GameObject Shine,Start_botton;
    public GameObject s_new,s_chock;
    public GameObject bg;
    public SerialPort sp;
    int baudRate = 115200;
    int readTimeOut = 100;
    int bufferSize = 32; // Device sends 32 bytes per packet
    bool programActive = true;
    bool reset_mode = true;
    bool start_mode = false;
    bool play_mode = false;
    bool is_left;
    public enum Swing { Start , Up, Down, Left, Right, Reset };
    Thread thread1, thread2;
    Swing chock = Swing.Reset;
    string input_data;

    public GameObject Left, Right, Up, Down, Hit;
    public float time; //宣告浮點數，名稱time
    public int counter = 0;
    int arrow_type;
    float arrow_pos;
    Vector3 pos;

    void Start()
    {

        FileStream fileStream = new FileStream(@"D:\TextReader.txt", FileMode.Open, FileAccess.Read);
        try
        {
            // read from file or write to file 
            //FileStream fileStream = new FileStream(@"c:\test1.txt", FileMode.Open, FileAccess.Read);
            //while(line = fileStream.Readline() != -1)
        }
        finally
        {
            fileStream.Close();
        }
        System.IO.StreamReader file = new System.IO.StreamReader(@"D:\TextReader.txt");
        string line;
        int counter = 0;

        while ((line = file.ReadLine()) != null)
        {
            //System.Console.WriteLine(line);
            counter++;
            //System.Console.WriteLine("Hello world");
            Debug.Log("counter = " + counter);
            Debug.Log(line);
        }

        file.Close();
        Debug.Log("There were " + counter + " lines.");

        chock = (Swing)(0);

        chock = (Swing)(int.Parse("5"));
        Debug.Log(chock + " chock in hit");

        sp = new SerialPort("COM4", 115200);
        Debug.Log("sp is new");
        if (!sp.IsOpen)
        {
            print("Opening COM4, baud 115200");
            sp.Open();
            print("open success");
            sp.ReadTimeout = 5000;
            sp.Handshake = Handshake.None;
            if (sp.IsOpen) { print("Open"); }
            pos = new Vector3(0, 0, -1.5f); //start的座標

            
            thread2 = new Thread(new ThreadStart(ProcessData));
            thread2.Start();


        }
    }

    // Update is called once per frame
    void Update()
    {
        is_left = bg.GetComponent<tesy>().is_left;
        Debug.Log((int)chock);
        Debug.Log("is_left" + is_left);
        if (Input.GetKey(KeyCode.Space))
        {
            is_start();
            chock = Swing.Reset;
            time = -5f;
        }
        if ((int)chock == 0 && reset_mode)
        {
            Reset();
            chock = Swing.Reset;
        }
        if((int)chock == 0 && !reset_mode)
        {
            Pause();
            chock = Swing.Reset;
        }
        if ((int)chock == 2 && start_mode)
        {
            is_start();
            chock = Swing.Reset;
            time = -5f;
        }

        time += Time.deltaTime; //時間增加

        if (time > 0.55f&&play_mode) //如果時間大於0.5(秒)
        {
            int arrow_type_new = UnityEngine.Random.Range(0, 4);
            if (arrow_type_new == arrow_type)
            {
                arrow_type_new += 1;
                arrow_type_new %= 4;
            }
            arrow_type = arrow_type_new;
            //float arrow_pos = UnityEngine.Random.Range(0, 4)*1f-1.5f;
            arrow_pos = UnityEngine.Random.Range(0, 4);
            switch (arrow_type)
            {
                case 0:
                    pos = new Vector3(0, 2.5f, -0.5f);
                    GameObject Left_arrow = (GameObject)Instantiate(Left, pos, transform.rotation); //產生左箭頭
                    //Left_arrow.transform.position += new Vector3(arrow_pos*1f-1.5f, -2f, 0);
                    break;
                case 1:
                    pos = new Vector3(0, 2.5f, -0.5f);
                    GameObject Up_arrow = (GameObject)Instantiate(Up, pos, transform.rotation); //產生上箭頭
                    //Up_arrow.transform.position += new Vector3(arrow_pos * 1f - 1.5f, -2f, 0);

                    break;
                case 2:
                    pos = new Vector3(0, 2.5f, -0.5f);
                    //Instantiate(Down , pos, transform.rotation); //產生下箭頭
                    GameObject Down_arrow = (GameObject)Instantiate(Down, pos, transform.rotation); //產生下箭頭
                    //Down_arrow.transform.position += new Vector3(arrow_pos * 1f - 1.5f, -2f, 0);

                    break;
                case 3:
                    pos = new Vector3(0, 2.5f, -0.5f);
                    //Instantiate(Right, pos, transform.rotation); //產生右箭頭
                    GameObject Right_arrow = (GameObject)Instantiate(Right, pos, transform.rotation); //產生右箭頭
                    //Right_arrow.transform.position += new Vector3(arrow_pos * 1f - 1.5f, -2f, 0);

                    break;

            }
            time = 0f; //時間歸零
        }
    }

    void Reset()
    {
                //s.SetActive(true);
                //Instantiate(Shine, pos, transform.rotation); //產生揮擊圖案
                start_mode = true;
                Debug.Log("play_mode in Reset" + play_mode);
                chock = Swing.Reset;
                reset_mode = false;

    }

    void is_start()
    {
        s_new.SetActive(false);
        pos = new Vector3(0, -0.58f, -1);
        Instantiate(s_chock, pos, transform.rotation); //產生揮擊圖案
        play_mode = true;
        start_mode = false;
        
    }

    void Pause()
    {
        play_mode = false;
        reset_mode = true;
        
    }

    void Gameover()
    {
        play_mode = false;
        Application.Quit();
    }
    void ProcessData()
    {
        Byte[] buffer = new Byte[bufferSize];
        //string input_data;
        int bytesRead = 0;
        Debug.Log("Thread started");
        while (programActive)
        {
            //Debug.Log("before try");
            try
            {
                //Debug.Log("enter try");
                // Attempt to read data from the BT device
                // - will throw an exception if no data is received within the timeout period
                bytesRead = sp.Read(buffer, 0, bufferSize);
                input_data = sp.ReadLine();
                // Use the appropriate SerialPort read method for your BT device e.g. ReadLine(..) for newline terminated packets
                if (bytesRead > 0)
                {
                    // Do something with the data in the buffer
                    //Debug.Log("read success");

                    if (input_data.Length > 0)
                    {
                        Debug.Log(input_data);
                        chock = (Swing)int.Parse(input_data);

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


    void OnTriggerStay2D(Collider2D col)
    {
        Vector3 pos = new Vector3(0f, 0f, -1.1f);


        if (col.tag == "Up") //如果碰撞的標籤是上箭頭
        {
            if (is_left == col.gameObject.GetComponent<Up>().left )
            {
                //Destroy(col.gameObject);
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    Destroy(col.gameObject); //消滅碰撞的物件
                    Instantiate(Shine, pos, transform.rotation); //產生揮擊圖案

                }
                if ((int)chock == 1)
                {
                    Instantiate(Shine, pos, transform.rotation); //產生揮擊圖案
                    Destroy(col.gameObject);
                    chock = Swing.Reset;
                }
            }
                

        }
        else if (col.tag == "Down")
        {
            if (is_left == col.gameObject.GetComponent<Down>().left )
            { 

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    Destroy(col.gameObject); //消滅碰撞的物件
                    Instantiate(Shine, pos, transform.rotation); //產生揮擊圖案

                }
                if ((int)chock == 2)
                {
                    Destroy(col.gameObject);
                    Instantiate(Shine, pos, transform.rotation); //產生揮擊圖案

                    chock = Swing.Reset;
                }
            }
        }
        else if (col.tag == "Left")
        {
            if (is_left == col.gameObject.GetComponent<Left>().left )
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    Destroy(col.gameObject); //消滅碰撞的物件
                    Instantiate(Shine, pos, transform.rotation); //產生揮擊圖案

                }
                if ((int)chock == 3)
                {
                    Destroy(col.gameObject);
                    Instantiate(Shine, pos, transform.rotation); //產生揮擊圖案

                    chock = Swing.Reset;
                }
            }

                
        }
        else if (col.tag == "Right")
        {
            if (is_left == col.gameObject.GetComponent<Right>().left )
            {

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    Destroy(col.gameObject); //消滅碰撞的物件
                    Instantiate(Shine, pos, transform.rotation); //產生揮擊圖案

                }
                if ((int)chock == 4)
                {
                    Destroy(col.gameObject);
                    Instantiate(Shine, pos, transform.rotation); //產生揮擊圖案

                    chock = Swing.Reset;
                }
            }
            
        }
    }

}

