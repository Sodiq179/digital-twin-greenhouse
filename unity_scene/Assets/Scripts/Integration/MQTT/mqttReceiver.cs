/*
The MIT License (MIT)

Copyright (c) 2018 Giovanni Paolo Vigano'

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class mqttReceiver : M2MqttUnityClient
{
    [Header("MQTT topics")]
    [Tooltip("Set the topic to subscribe. !!!ATTENTION!!! multi-level wildcard # subscribes to all topics")]
    public string topicSubscribe = "#"; // topic to subscribe. !!! The multi-level wildcard # is used to subscribe to all the topics. Attention i if #, subscribe to all topics. Attention if MQTT is on data plan
    [Tooltip("Set the topic to publish (optional)")]
    public string topicPublish = ""; // topic to publish
    public string messagePublish = ""; // message to publish

    [Tooltip("Set this to true to perform a testing cycle automatically on startup")]
    public bool autoTest = false;

    public TMP_Text avgTempText;
    public TMP_Text avgHumText;

    private List<float> temperatureData = new List<float>();
    private List<float> humidityData = new List<float>();

    private bool isCollectingData = false;
    private float collectionTime = 30f;

    //using C# Property GET/SET and event listener to reduce Update overhead in the controlled objects
    private string m_msg;
    
    public string msg
    {
        get
        {
            return m_msg;
        }
        set
        {
            if (m_msg == value) return;
            m_msg = value;
            if (OnMessageArrived != null)
            {
                OnMessageArrived(m_msg);
            }
        }

    
    
    }

    public event OnMessageArrivedDelegate OnMessageArrived;
    public delegate void OnMessageArrivedDelegate(string newMsg);

    //using C# Property GET/SET and event listener to expose the connection status
    private bool m_isConnected;

    public bool isConnected
    {
        get
        {
            return m_isConnected;
        }
        set
        {
            if (m_isConnected == value) return;
            m_isConnected = value;
            if (OnConnectionSucceeded != null)
            {
                OnConnectionSucceeded(isConnected);
            }
        }
    }
    public event OnConnectionSucceededDelegate OnConnectionSucceeded;
    public delegate void OnConnectionSucceededDelegate(bool isConnected);

    // a list to store the messages
    private List<string> eventMessages = new List<string>();

    public void Publish()
    {
        client.Publish(topicPublish, System.Text.Encoding.UTF8.GetBytes(messagePublish), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        Debug.Log("Test message published");
    }
    public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }

    protected override void OnConnecting()
        {
            base.OnConnecting();
        }

    protected override void OnConnected()
        {
            base.OnConnected();
            isConnected=true;

            if (autoTest)
            {
                Publish();
            }
        }

    protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.Log("CONNECTION FAILED! " + errorMessage);
        }

    protected override void OnDisconnected()
        {
            Debug.Log("Disconnected.");
            isConnected=false;
        }

    protected override void OnConnectionLost()
        {
            Debug.Log("CONNECTION LOST!");
        }

    protected override void SubscribeTopics()
        {
            client.Subscribe(new string[] { topicSubscribe }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

    protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(new string[] { topicSubscribe });
        }

    protected override void Start()
        {
            base.Start();
        }

    protected override void DecodeMessage(string topic, byte[] message)
        {
            //The message is decoded
            msg = System.Text.Encoding.UTF8.GetString(message);

            // Update UI with received data
            UpdateUI(msg);

            Debug.Log("Received: " + msg);
            Debug.Log("from topic: " + m_msg);

            

            StoreMessage(msg);
            if (topic == topicSubscribe)
            {
                if (autoTest)
                {
                    autoTest = false;
                    Disconnect();
                }
            }
        }

    private void StoreMessage(string eventMsg)
        {
            if (eventMessages.Count > 50)
            {
                eventMessages.Clear();
            }
            eventMessages.Add(eventMsg);
        }

    protected override void Update()
        {
            base.Update(); // call ProcessMqttEvents()

        }

    private void OnDestroy()
        {
            Disconnect();
        }

    private void OnValidate()
        {
            if (autoTest)
            {
                autoConnect = true;
            }
        }

    private void UpdateUI(string message)
    {
        message = message.Replace("{", "").Replace("}", "").Replace("'", "");

        string[] lines = message.Split(',');
        if (lines.Length >= 2)
        {
            string temperatureLine = lines[0].Trim();
            string humidityLine = lines[1].Trim();

            string[] tempParts = temperatureLine.Split(':');
            string[] humParts = humidityLine.Split(':');

        
            // Remove leading and trailing single quotes
            string temperature = tempParts[1].Trim('\'').Trim();
            string humidity = humParts[1].Trim('\'').Trim();

            // Convert to float
            if (float.TryParse(temperature, out float tempValue))
            {
                temperatureData.Add(tempValue);
            }

            if (float.TryParse(humidity, out float humValue))
            {
                humidityData.Add(humValue);
            }

            Debug.Log("temp: " + temperature);
            Debug.Log("hum: " + humidity);
        
        }

        // Start collecting data if not already collecting
        if (!isCollectingData)
        {
            StartCoroutine(CollectData());
        }
    }

    private IEnumerator CollectData()
    {
        isCollectingData = true;
        yield return new WaitForSeconds(collectionTime);

        // Calculate averages
        float avgTemp = CalculateAverage(temperatureData);
        float avgHum = CalculateAverage(humidityData);

        // Update UI with averages
        if (avgTempText != null)
        {
            avgTempText.text = avgTemp.ToString("F2"); // Format to 2 decimal places
        }

        if (avgHumText != null)
        {
            avgHumText.text = avgHum.ToString("F2"); // Format to 2 decimal places
        }

        // Clear data for next collection period
        temperatureData.Clear();
        humidityData.Clear();

        isCollectingData = false;
    }

    private float CalculateAverage(List<float> data)
    {
        if (data.Count == 0)
        {
            return 0;
        }

        float sum = 0;
        foreach (float value in data)
        {
            sum += value;
        }

        return sum / data.Count;
    }

}

