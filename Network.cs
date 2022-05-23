using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Text;

//binaryReader binaryWriter

public class Network : MonoBehaviour
{
    private TcpClient client = new TcpClient();
    public NetworkStream stream;
    public Level myLevel;
    public Player myPlayer;
    public int size;
    private byte Net_ID;
    

    const string IP = "192.168.2.9";
    const int PORT = 8080;
    const double memoryAmount = 5e+6;
    const int waitTime = 5000;

    public byte[] data = new byte[(int)memoryAmount];
    public bool running = false;

    // Start is called before the first frame update
    void Start()
    {
       
        Debug.Log("gia pame ligo");
        connect((bool result) =>
        {
            if (result == true)
            {
                Debug.Log("connected");
                stream = client.GetStream();
                stream.Flush();
                running = true;

                myLevel = GameObject.Find("Level").GetComponent<Level>();
                myPlayer = GameObject.Find("Player").GetComponent<Player>();
            }
            else
            {
                myLevel.myKoumpia.SetActive(true);
                Debug.Log("not_connected");
            }
        });
    }

    public void pare_Karta()
    {
        // allakse thn karta toy player
    }

    private void connect(Action<bool> callback)
    {
        bool result = client.ConnectAsync(IP, PORT).Wait(waitTime);
        callback(result);
    }

    public void send_epilogh(byte stage)
    {
        byte[] x = new byte[3];
        x[0] = myLevel.stage;
        x[1] = Net_ID;
        x[2] = Convert.ToByte(myPlayer.epilogh);
        stream.Write(x, 0, 3);
        myLevel.stage = 4;
    }

    public string parse(string message, ref int a,ref int b)
    {
        b = message.IndexOf('.', a);
        string temp = message.Substring(a, b - a);
        a = b+1;
        return temp;
    }
    private void execute(string command)
    {
        Debug.Log(command);
        switch (command[0])
        {
            case 'A':
                //moirasma arxhs
            
                int begin = 2;
                int telos = 2;
                size = Convert.ToInt32(command.Substring(1, 1));
                myLevel.sinolo_paixtwn = size;
                int count = 0;
                for (int i=0; i<size; i++)
                {               
                    //-------------PARSE
                    telos = command.IndexOf('.', begin);
                    string temp_name = command.Substring(begin, telos-begin);           //name
                    begin = telos+1;
                                                                                                    //-------------------
                    telos = command.IndexOf('.', begin);
                    int temp_xroma = Convert.ToInt32(command.Substring(begin, telos-begin));    // xroma
                    begin = telos+3;
                    
                    if (i != Net_ID)
                    {
                        myLevel.enemies[i+count].name = temp_name;
                        myLevel.enemies[i+count].xroma = temp_xroma;
                    }
                    else
                    {
                        count = -1;
                    }
                    
                }
                //1os afigitis
                myLevel.afigitis = Convert.ToInt32(command.Substring(begin - 2, 1));
                //moirasma 6 karton
                for (int i=0; i<6; i++)
                {
                    telos = command.IndexOf('.', begin);
                    myPlayer.xeri[i].set_karta(Convert.ToInt32(command.Substring(begin, telos-begin)));
                    begin = telos + 1;
                }
                myLevel.neos_giros();
                Debug.Log("Arxh");

                break;
            case 'P':
                //pontoi
                begin = 1;
                telos = 1;
                count = 0;
                for(int i=0; i<size; i++)
                {
                    telos = command.IndexOf('.', begin);
                    if (i == Net_ID)
                    {
                        myPlayer.vathmos = Convert.ToInt32(command.Substring(begin, telos-begin));
                        count = -1;
                        begin = telos + 1;
                        continue;
                    }
                    myLevel.enemies[i+count].vathmos = Convert.ToInt32(command.Substring(begin, telos-begin));
                    begin = telos + 1;
                }
                myLevel.neos_giros();
                Debug.Log("pontoi");

                break;
            case 'K':
                //draw Karta
                begin = 1;
                telos = 1;
                string new_card = parse(command,ref begin,ref telos);
                myPlayer.GetComponent<Player>().xeri[5].set_karta(Convert.ToInt32(new_card));
                //myLevel.GetComponent<Level>().neos_giros();
                Debug.Log("Karta");
                break;

            case 'G':
                // perigrafi afigiti
                int afigitis = myLevel.afigitis;
                if (afigitis >= Net_ID)
                {
                    afigitis--;
                }
                //myLevel.GetComponent<Level>().berdemata();
                myLevel.enemies[afigitis].karta.set_karta(Convert.ToInt32(command.Substring(1,2),16));           // an einai hex  , 16
                myLevel.epelekse();
                break;
            case 'F':
                // berdemata
                begin = 1;
                telos = 1;
                for (int i=0; i<size;  i++)
                {
                    string part =  parse(command,ref begin,ref telos);
                    Debug.Log(part);
                    myLevel.enemies[i].karta.set_karta(Convert.ToInt32(part,16));     // Lathos
                }
                myLevel.berdemata();
                Debug.Log("Perigrafi");
                break;
           
            case 'N':
                //Telos paixnidiou - Nikitis
                myLevel.telos();
                Debug.Log("Nikitis");
                break;

            case 'I':
                // ID Paixth
                Net_ID = Convert.ToByte(command.Substring(1, 1));
                Net_ID--;
                myLevel.Net_ID = this.Net_ID;
                sendToServer(myPlayer.onoma, 1, Net_ID);
                sendToServer(myPlayer.xroma.ToString(), 2, Net_ID);
                break;
            default:
                Debug.Log("kati paixtike");
                break;
        }
    }

    public void sendToServer(string a, byte b1, byte b2)                    // SEND----------------
    {
        byte[] x = Encoding.UTF8.GetBytes(a);
        byte[] b = new byte[x.Length + 2];
        b[0] = b1;                                      // logos pou stelno
        b[1] = b2;                                      // 
        for (int i=0; i<x.Length; i++)
        {
            b[i + 2] = x[i];
        }
        stream.Write(b,0,b.Length);
    }
  
    // Update is called once per frame
    void Update()
    {
        // check gia message
        if (running == true)            // To running de ginetai pote True              // RECEIVE --------------
        {
            if (stream.DataAvailable)
            {
               Debug.Log("exo");
               int dataLeng = stream.Read(data, 0, data.Length);
                
               string message = Encoding.UTF8.GetString(data, 0, data.Length);
                stream.Flush();
               execute(message);
                Debug.Log(message);
                stream.Flush();
                Debug.Log(message);
            }
        }
    }
}
