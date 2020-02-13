using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


//at the moment the first object that use a value turns it into inactive.
public class EasyController : MonoBehaviour
{
    public int port = 4242;
    private UdpClient client;
    private IPEndPoint RemoteIpEndPoint;
    private Thread t_udp;
    //public ArrayList tmpList;
    public int[] mmaxvalues;
    private SortedList easyconl, easyactive;
    public static EasyController escon;
    public static EasyControllerSend esconsend;
    public int a, b;
    public string returnData; //didn't use to be public
    ArrayList bufferList;
    ArrayList prebuffer;
    int iter = 0;
    int pre_index = -1;
    private bool is_updated;

    void Start()
    {
        bufferList = new ArrayList();
        prebuffer = new ArrayList();
        easyconl = new SortedList();
        easyactive = new SortedList();
        populate_list(easyconl);
        populate_list(easyactive);
        escon = GameObject.FindWithTag("Interaction").GetComponent(typeof(EasyController)) as EasyController; //connect to easy_controller
        esconsend = GameObject.FindWithTag("Interaction").GetComponent(typeof(EasyControllerSend)) as EasyControllerSend; //connect to easy_controller

        client = new UdpClient(port);
        RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        t_udp = new Thread(new ThreadStart(UDPRead));
        t_udp.Name = "UDP thread";
        t_udp.Start();
      
        //FilterData(test);
    }

    public void UDPRead()
    {
        while (true)
        {
            try
            {
                byte[] receiveBytes = client.Receive(ref RemoteIpEndPoint);
                //				byte[] sendBytes = client.Send(ref RemoteIpEndPoint);
                returnData = Encoding.ASCII.GetString(receiveBytes);

                // parsing //
                if (receiveBytes.Length > 0)
                {
                    bufferList.Add(returnData);
                    iter++;
                }
              

            }
            catch (Exception e)
            {
                Debug.Log("Not so good " + e.ToString());
            }
            Thread.Sleep(20);
        }
    }

    private void Update()
    {
        while (iter > 0)
        {
            FilterData(bufferList[0].ToString());
            iter--;
            prebuffer.Add(mmaxvalues[0]);
            bufferList.RemoveAt(0);
            easyactive[mmaxvalues[0]] = easyconl[mmaxvalues[0]];
        }

    }

    void LateUpdate()
    {
        while(prebuffer.Count>0)
        {
            easyactive[prebuffer[0]] = -1;
            prebuffer.RemoveAt(0);
        }
    }

    void OnDisable()
    {
        if (t_udp != null) t_udp.Abort();
        client.Close();
    }

    public float MaxValue(int index)
    {
        return mmaxvalues[index];
    }

    private void FilterData(string returnData)
    {
        mmaxvalues = new int[0];
        string[] separators = { ",", " ", "" };
        string[] values = returnData.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        mmaxvalues = new int[values.Length - 1];
        for (int i = 0; i < values.Length - 1; i++)
        {
            mmaxvalues[i] = int.Parse(values[i]);
        }
        easyactive[mmaxvalues[0]] = easyconl[mmaxvalues[0]];
        easyconl[mmaxvalues[0]] = mmaxvalues[1];


    }

    public float scale(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }


    

    public bool is_triggered(int index)
    {
     
        if ((int)easyactive[index]==1)
        {
            return true;
        }
        return false;
    }

    public bool bang(int index)
    {

        if ((int)easyactive[index] == 1)
        {
            esconsend.Send_data(index,0);
            return true;
        }
        return false;
    }

    public int get_state(int index)
    {
        return (int)easyconl[index];
    }

    public float get_state(int index, float from, float to)
    {
        float value = (int)easyconl[index];
        return scale(value, 0.0f, 127.0f, from, to);
    }


    public int get_newstate(int index)
    {
        //Debug.Log((int)easyconl[index]);
        if ((int)easyactive[index]!=-1)
        {
            //Debug.Log(easyactive[index]);
            return (int)easyconl[index];
        }
        return -1;
    }

    public float get_newstate(int index, float from, float to)
    {
        if ((int)easyactive[index] != (int)easyconl[index])
        {
            float value = (float)escon.easyconl[index];
            return scale(value, 0.0f, 127.0f, from, to);
        }

        return -1.0f;
    }



    private void populate_list(SortedList easyconlist)
    {
        easyconlist.Add(0, -1);
        easyconlist.Add(1, -1);
        easyconlist.Add(2, -1);
        easyconlist.Add(3, -1);
        easyconlist.Add(4, -1);
        easyconlist.Add(5, -1);
        easyconlist.Add(6, -1);
        easyconlist.Add(7, -1);
        easyconlist.Add(8, -1);

        easyconlist.Add(10, -1);
        easyconlist.Add(11, -1);
        easyconlist.Add(12, -1);
        easyconlist.Add(13, -1);
        easyconlist.Add(14, -1);
        easyconlist.Add(15, -1);
        easyconlist.Add(16, -1);
        easyconlist.Add(17, -1);
        easyconlist.Add(18, -1);

        easyconlist.Add(101, -1);
        easyconlist.Add(102, -1);
        easyconlist.Add(103, -1);
        easyconlist.Add(104, -1);
        easyconlist.Add(105, -1);
        easyconlist.Add(106, -1);
        easyconlist.Add(107, -1);
        easyconlist.Add(108, -1);

        easyconlist.Add(111, -1);
        easyconlist.Add(112, -1);
        easyconlist.Add(113, -1);
        easyconlist.Add(114, -1);
        easyconlist.Add(115, -1);
        easyconlist.Add(116, -1);
        easyconlist.Add(117, -1);
        easyconlist.Add(118, -1);


        easyconlist.Add(99, -1);
    }
}


