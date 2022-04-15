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
    public string temperature { get; set; }
    public string humidity { get; set; }
}

public class DeviceData
{
    public string name { get; set; }
    public bool isOn { get; set; }
}