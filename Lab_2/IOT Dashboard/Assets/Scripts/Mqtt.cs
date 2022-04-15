using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json;

namespace Dashboard
{
    public class Mqtt : M2MqttUnityClient
    {
        [SerializeField]
        public StatusData statusData = new StatusData();
        public DeviceData ledData = new DeviceData();
        public DeviceData pumpData = new DeviceData();
        private System.Random rnd;
        private Dictionary<string, Action<string>> topicHandlers = new Dictionary<string, Action<string>>();

        protected override void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 240;
            this.rnd = new System.Random();

            this.SetUpMQTT();
            this.Connect();

            StartCoroutine(PublishData());
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
            // if (updateUI)
            // {
            //     UpdateUI();
            // }
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            Debug.Log("Connecting\n");
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            DataSession.isConnected = true;
            Debug.Log("Connected\n");
            SubscribeTopics();
        }

        protected override void SubscribeTopics()
        {
            foreach (KeyValuePair<string, Action<string>> entry in topicHandlers)
            {
                string topic = entry.Key;
                if (topic.Length != 0)
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                }
            }
        }

        protected override void UnsubscribeTopics()
        {
            foreach (KeyValuePair<string, Action<string>> entry in topicHandlers)
            {
                string topic = entry.Key;
                if (topic.Length != 0)
                {
                    client.Unsubscribe(new string[] { topic });
                }
            }
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.Log("CONNECTION FAILED! " + errorMessage);
        }

        protected override void OnDisconnected()
        {
            Debug.Log("Disconnected.");
            DataSession.isConnected = false;
        }

        protected override void OnConnectionLost()
        {
            Debug.Log("CONNECTION LOST!");
            DataSession.isConnected = false;
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            if (topicHandlers.ContainsKey(topic))
            {
                topicHandlers[topic](msg);
            }
        }

        IEnumerator PublishData()
        {
            StatusData status = new StatusData();
            DeviceData led = new DeviceData();
            DeviceData pump = new DeviceData();
            status.temperature = "0";
            status.humidity = "0";
            led.name = "LED";
            led.isOn = false;
            pump.name = "Pump";
            pumpData.isOn = false;
            while (!DataSession.isConnected)
            {
                yield return new WaitForSeconds(0.1f);
            }
            string dataToPublish;
            dataToPublish = JsonConvert.SerializeObject(status);
            client.Publish("/bkiot/1911056/status", System.Text.Encoding.UTF8.GetBytes(dataToPublish), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            dataToPublish = JsonConvert.SerializeObject(led);
            client.Publish("/bkiot/1911056/led", System.Text.Encoding.UTF8.GetBytes(dataToPublish), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            dataToPublish = JsonConvert.SerializeObject(pump);
            client.Publish("/bkiot/1911056/pump", System.Text.Encoding.UTF8.GetBytes(dataToPublish), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            while (true)
            {
                status.temperature = rnd.Next(0, 39).ToString();
                status.humidity = rnd.Next(0, 100).ToString();
                dataToPublish = JsonConvert.SerializeObject(status);
                client.Publish("/bkiot/1911056/status", System.Text.Encoding.UTF8.GetBytes(dataToPublish), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                yield return new WaitForSeconds(2f);
            }
        }

        private void ProcessMessageStatus(string msg)
        {
            Debug.Log("Received: " + msg);
            statusData = JsonConvert.DeserializeObject<StatusData>(msg);
            GetComponent<DashboardManager>().UpdateStatus(statusData);
        }

        private void ProcessMessageLed(string msg)
        {
            Debug.Log("Processing LED: " + msg);
            ledData = JsonConvert.DeserializeObject<DeviceData>(msg);
            GetComponent<DashboardManager>().UpdateLED(ledData);
        }

        private void ProcessMessagePump(string msg)
        {
            Debug.Log("Processing pump: " + msg);
            pumpData = JsonConvert.DeserializeObject<DeviceData>(msg);
            GetComponent<DashboardManager>().UpdatePump(pumpData);
        }

        public void PublishLED()
        {
            ledData = GetComponent<DashboardManager>().onLEDHandler(ledData);
            string msg_config = JsonConvert.SerializeObject(ledData);
            client.Publish("/bkiot/1911056/led", System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }
        public void PublishPump()
        {
            pumpData = GetComponent<DashboardManager>().onPumpHandler(pumpData);
            string msg_config = JsonConvert.SerializeObject(pumpData);
            client.Publish("/bkiot/1911056/pump", System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        private void SetUpMQTT()
        {
            this.brokerAddress = DataSession.brokerAddress;
            this.brokerPort = 1883;
            this.mqttUserName = DataSession.username;
            this.mqttPassword = DataSession.password;

            this.topicHandlers["/bkiot/1911056/status"] = ProcessMessageStatus;
            this.topicHandlers["/bkiot/1911056/led"] = ProcessMessageLed;
            this.topicHandlers["/bkiot/1911056/pump"] = ProcessMessagePump;
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnLogOut()
        {
            SceneManager.LoadScene("Connect");
        }
    }
}
