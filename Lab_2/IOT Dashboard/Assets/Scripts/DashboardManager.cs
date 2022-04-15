using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dashboard
{
    public class DashboardManager : MonoBehaviour
    {
        public Text temperature;
        public Text humidity;
        public ToggleSwitch led;
        public ToggleSwitch pump;
        public Window_Graph graph;
        private List<int> listHumidity = new List<int>();
        void Start()
        {
            temperature = GameObject.Find("/Canvas/LayerDashboard/Temperature/Data").GetComponent<Text>();
            humidity = GameObject.Find("/Canvas/LayerDashboard/Humidity/Data").GetComponent<Text>();
            led = GameObject.Find("/Canvas/LayerDashboard/LED").GetComponent<ToggleSwitch>();
            pump = GameObject.Find("/Canvas/LayerDashboard/Pump").GetComponent<ToggleSwitch>();
            led.interactable = false;
            pump.interactable = false;
            led.onPress = (i) => GetComponent<Mqtt>().PublishLED();
            pump.onPress = (i) => GetComponent<Mqtt>().PublishPump();
        }

        public void UpdateStatus(StatusData status)
        {
            temperature.text = status.temperature;
            humidity.text = status.humidity;
            listHumidity.Add(System.Int32.Parse(status.humidity));
            if (listHumidity.Count > 8)
            {
                listHumidity.RemoveAt(0);
            }
            graph.DrawGraph(listHumidity);
        }
        public void UpdateLED(DeviceData device)
        {
            led.interactable = true;
            if (device.isOn != led.isOn)
            {
                led.Toggle(!led.isOn);
            }
        }

        public void UpdatePump(DeviceData device)
        {
            pump.interactable = true;
            if (device.isOn != pump.isOn)
            {
                pump.Toggle(!pump.isOn);
            }
        }

        public DeviceData onLEDHandler(DeviceData device)
        {
            device.isOn = led.isOn;
            led.interactable = false;
            return device;
        }
        public DeviceData onPumpHandler(DeviceData device)
        {
            device.isOn = pump.isOn;
            pump.interactable = false;
            return device;
        }
    }
}