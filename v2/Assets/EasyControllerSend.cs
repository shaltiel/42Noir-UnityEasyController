
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class EasyControllerSend : MonoBehaviour
{
    private static int localPort;

    // prefs
    private string IP;  // define in init
    public int port = 8052;  // define in init

    // "connection" things
    IPEndPoint remoteEndPoint;
    UdpClient client;

    // gui
    public string easymsg= "";
    // Use this for initialization
    void Start()
    {
    
        // define
        IP = "127.0.0.1";
        port = 4459;

        // ----------------------------
        // Send
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();

        // status
        print("Sending to " + IP + " : " + port);
        print("Testing: nc -lu " + IP + " : " + port);

        //Send_data("200 1"); //200 is preset
        Send_data("160 1"); //160 initial sync
    }

    private void OnApplicationQuit()
    {
        Send_data("160 0");
    }

    public void Send_data(string str)
    {
        try
        {
            //nt length = c*1000+v;
            byte[] data = Encoding.UTF8.GetBytes(str);
          
                client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    public void Send_data(int index,int value)
    {
        Send_data(index.ToString() + " " + value.ToString());
        EasyController.escon.sync(index, value);
    }

    public void Send_data(string sindex, int value)
    {
        int index = EasyController.escon.string_to_index(sindex);
        Send_data(index.ToString() + " " + value.ToString());
        EasyController.escon.sync(index, value);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            Send_data(easymsg);
    }

}

