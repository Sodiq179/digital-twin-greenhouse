using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;

public class DataFetcher : MonoBehaviour
{
    private void Start()
    {
        int selectedMonth = SceneDataManager.Instance != null ? SceneDataManager.Instance.SelectedMonth : 0;
        StartCoroutine(FetchData(selectedMonth));
    }

    private IEnumerator FetchData(int month)
    {
        string url = $"http://52.55.28.0:4000/sensors/sensor_data/2020/{month}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                UnityEngine.Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                UnityEngine.Debug.Log($"Received Data: {jsonResponse}");

                ProcessData(jsonResponse);
            }
        }
    }

    private void ProcessData(string json)
    {
        // Parse the JSON string
        var sensorDataArray = JsonUtility.FromJson<RootObject>(json);

        // Access the sensor readings
        var sensorReadings = sensorDataArray.data.sensorReadings;

        // Print and update sensor readings
        foreach (var sensorReading in sensorReadings)
        {
            foreach (var sensorData in sensorReading.readings)
            {
                UnityEngine.Debug.Log($"Sensor ID: {sensorData.sensorId}, Temperature: {sensorData.temperature}, Humidity: {sensorData.humidityPercent}");
                UpdateSensor(sensorData);
            }
        }
    }

    private void UpdateSensor(Reading sensorData)
    {
        string sensorId = sensorData.sensorId;

        // Find the GameObject with the name matching the sensorId
        GameObject sensorObject = GameObject.Find(sensorId);
        if (sensorObject != null)
        {
            Sensor sensor = sensorObject.GetComponentInChildren<Sensor>();
            if (sensor != null)
            {
                sensor.SetSensorData(sensorData.temperature, sensorData.humidityPercent);
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning($"Sensor GameObject with name {sensorId} not found.");
        }
    }

    // Define the classes to deserialize the JSON
    [Serializable]
    public class Reading
    {
        public string sensorId;
        public float temperature;
        public float humidityPercent;
    }

    [Serializable]
    public class SensorReading
    {
        public Reading[] readings;
    }

    [Serializable]
    public class Data
    {
        public SensorReading[] sensorReadings;
    }

    [Serializable]
    public class RootObject
    {
        public Data data;
    }
}
