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
        // Start is called before the first frame update
        void Start()
        {
            temperature = GameObject.Find("/Canvas/LayerDashboard/Temperature/Data").GetComponent<Text>();
            humidity = GameObject.Find("/Canvas/LayerDashboard/Humidity/Data").GetComponent<Text>();
            led = GameObject.Find("/Canvas/LayerDashboard/LED").GetComponent<ToggleSwitch>();
            pump = GameObject.Find("/Canvas/LayerDashboard/Pump").GetComponent<ToggleSwitch>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateStatus(StatusData status)
        {
            temperature.text = status.Data.temperature.ToString();
            humidity.text = status.Data.humidity.ToString();
            if (status.Device.LED.isOn != led.isOn)
            {
                led.Toggle(!led.isOn);
            }
            if (status.Device.Pump.isOn != pump.isOn)
            {
                led.Toggle(!pump.isOn);
            }
        }
    }
}