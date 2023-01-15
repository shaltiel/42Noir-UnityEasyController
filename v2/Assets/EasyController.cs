using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

//at the moment the first object that use a value turns it into inactive.

public class EasyController : MonoBehaviour
{
    public int port = 4242;
    private UdpClient client;
    private IPEndPoint RemoteIpEndPoint;
    private Thread t_udp;
    //public ArrayList tmpList;
    public int[] mmaxvalues;
    //private SortedList easyconl, easyactive;

    public static EasyController escon;
    public static EasyControllerSend esconsend;

    public int a, b;
    public string returnData; //didn't use to be public
    ArrayList bufferList;
    ArrayList prebuffer;
    int iter = 0;
    int pre_index = -1;
    private bool is_updated;

    //parameters for counting new changes and waiting one loop
    bool count_new_states = false;
    int count_new = 0;
    int final_n_new = -1;
    int count = 0;

    System.Collections.Generic.IDictionary<string, int> dict_s_i = new System.Collections.Generic.Dictionary<string, int>(){
    {"m1", 0},{"s1a",1},{"s2a",2},{"s3a",3},{"s4a",4},{"s5a",5},{"s6a",6},{"s7a",7},{"s8a",8},
    {"m2", 10},{"s1b",11},{"s2b",12},{"s3b",13},{"s4b",14},{"s5b",15},{"s6b",16},{"s7b",17},{"s8b",18},
    {"m3", 20},{"s1c",21},{"s2c",22},{"s3c",23},{"s4c",24},{"s5c",25},{"s6c",26},{"s7c",27},{"s8c",28},
    {"m4", 30},{"s1d",31},{"s2d",32},{"s3d",33},{"s4d",34},{"s5d",35},{"s6d",36},{"s7d",37},{"s8d",38},

    {"m5", 100},{"p1a",101},{"p2a",102},{"p3a",103},{"p4a",104},{"p5a",105},{"p6a",106},{"p7a",107},{"p8a",108},
    {"m6", 110},{"p1b",111},{"p2b",112},{"p3b",113},{"p4b",114},{"p5b",115},{"p6b",116},{"p7b",117},{"p8b",118},
    {"m7", 120},{"p1c",121},{"p2c",122},{"p3c",123},{"p4c",124},{"p5c",125},{"p6c",126},{"p7c",127},{"p8c",128},
    {"m8", 130},{"p1d",131},{"p2d",132},{"p3d",133},{"p4d",134},{"p5d",135},{"p6d",136},{"p7d",137},{"p8d",138}
    };

    IDictionary<int, int> notes_active = new Dictionary<int, int>() { };

    int[] easyconl = new int[400];
    int[] easyactive = new int[400];


    void Start()
    {
        bufferList = new ArrayList();
        prebuffer = new ArrayList();
          
        populate_list(easyconl);
        populate_list(easyactive);
        escon = FindObjectOfType<EasyController>() as EasyController; //connect to easy_controller
        esconsend = FindObjectOfType<EasyControllerSend>() as EasyControllerSend;

        client = new UdpClient(port);
        RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        t_udp = new Thread(new ThreadStart(UDPRead));
        t_udp.Name = "UDP thread";
        t_udp.Start();
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
            Thread.Sleep(2);
        }
    }

    private void Update()
    {

        if (final_n_new == -1)
        {
            if (count_new_states == false) count_new_states = true;
            else
            {
                final_n_new = count_new;
                count_new_states = false;
                count = 0;
            }
        }


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
        //while (prebuffer.Count > 0)
        //{
        //    easyactive[prebuffer[0]] = -1;
        //    prebuffer.RemoveAt(0);
        //}
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
        //UnityEngine.Debug.Log(easyactive[101]);


    }

    public float scale(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }


    public bool is_triggered(int index)
    {
        if ((int)easyactive[index] == 1)
        {
            counting();
            return true;
        }
        counting();
        return false;
    }

    public bool bang(int index)
    {
        if ((int)easyactive[index] == 1)
        {
            esconsend.Send_data(index, 0);
            counting();
            return true;
        }
        counting();
        return false;
    }

    public int get(int index)
    {
        return (int)easyconl[index];
    }


    public int get(string s)
    {
        int index = dict_s_i[s];
        return (int)easyconl[index];
    }

    public void sync(int index, int value)
    {
       
        easyconl[index] = value;
    }

    public float get(int index, float from, float to)
    {
        float value = (int)easyconl[index];
        return scale(value, 0.0f, 127.0f, from, to);
    }

    public float get(string s, float from, float to)
    {
        int index = dict_s_i[s];
        float value = (int)easyconl[index];
        return scale(value, 0.0f, 127.0f, from, to);
    }

    public IDictionary<int, int> get_active_notes()
    {
        for (int i = 0; i < prebuffer.Count; i++)
        {
            int note = (int)prebuffer[i];
            if (note > 200)
            {
                int vel = (int)easyconl[note];
                if (vel>0)
                {
                    notes_active.Add(note, vel);
                }
                else
                {
                    notes_active.Remove(note);
                }
            }

        }
            counting();
            return notes_active;
    }

    public List<int[]> get_new_notes()
    {
        List<int[]> tempnotes = new List<int[]>();
        for (int i = 0; i < prebuffer.Count; i++)
        {
            if ((int)prebuffer[i] > 200)
            {
                int[] note_vel = { (int)prebuffer[i], (int)easyconl[(int)prebuffer[i]] };
                tempnotes.Add(note_vel);
            }

        }
        counting();
        return tempnotes;
    }


    private void counting()
    {
        //first time we count no. of new.
        if (count_new_states) count_new += 1;
        else count += 1;

        if (count == final_n_new)
        {
            while (prebuffer.Count > 0)
            {
                easyactive[(int)prebuffer[0]] = -1;
                prebuffer.RemoveAt(0);
            }
            count = 0;
        }
    }

    public int get_new(int index)
    {
        if ((int)easyactive[index]!=-1)
        {
            int temp = (int)easyconl[index];

            counting();
            return temp;
        }
        counting();
        return -1;
    }

    public int get_new(string s)
    {
        int index = dict_s_i[s];
        if ((int)easyactive[index] != -1)
        {
            int temp = (int)easyconl[index];

            counting();
            return temp;
        }
        counting();
        return -1;
    }


    public void set(string str)
    {
        esconsend.Send_data(str);
    }

    public void set(int index, int value)
    {
        esconsend.Send_data(index, value);
    }

    public void set(string sindex, int value)
    {
        esconsend.Send_data(sindex, value);
    }


    public float get_new(int index, float from, float to)
    {
        if ((int)easyactive[index] != (int)easyconl[index])
        {
            float value = (float)escon.easyconl[index];
            counting();
            return scale(value, 0.0f, 127.0f, from, to);
        }
        counting();
        return -1.0f;
    }

    public float get_new(string s, float from, float to)

    {
        int index = dict_s_i[s];
        if ((int)easyactive[index] != (int)easyconl[index])
        {
            float value = (float)escon.easyconl[index];
            counting();
            return scale(value, 0.0f, 127.0f, from, to);
        }
        counting();
        return -1.0f;
    }



    private void populate_list(int[] easyconlist)
    {
        //sliders
        for  (int i=0;i<400; i++)
        {
            easyconlist[i] = -1;
        }
    }




    public int string_to_index(String s)
    {
        //sliders
        return dict_s_i[s];
    }

}


