using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSession : MonoBehaviour
{
    public static string brokerAddress = "mqttserver.tk";
    public static string username = "bkiot";
    public static string password = "12345678";
    public static bool isConnected = false;
}

public class StatusData
{
    public class __data
    {
        public int temperature { get; set; }
        public int humidity { get; set; }
    }
    public class __device
    {
        public class __led
        {
            public bool isOn { get; set; }
        }
        public class __pump
        {
            public bool isOn { get; set; }
        }
        public __led LED;
        public __pump Pump;
        public __device()
        {
            this.LED = new __led();
            this.Pump = new __pump();
        }
    }
    public __data Data;
    public __device Device;

    public StatusData()
    {
        this.Data = new __data();
        this.Device = new __device();
    }
}