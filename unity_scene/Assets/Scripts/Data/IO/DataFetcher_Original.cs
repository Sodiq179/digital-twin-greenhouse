using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DataFetcher_Original : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(FetchData());
    }

    private IEnumerator FetchData()
    {
        // string url = $"{baseUrl}/{endpoint}";
        String url = "http://52.55.28.0:4000/sensors/sensor_data/2020/2";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log($"Received Data: {jsonResponse}");

                ProcessData(jsonResponse);
            }
        }
    }

    private void ProcessData(string json)
    {
        SensorDataArray sensorDataArray = JsonUtility.FromJson<SensorDataArray>(json);
        SensorReading[] sensorReadings = sensorDataArray.sensorReadings;

        foreach (var sensorReading in sensorReadings)
        {
            UpdateSensor(sensorReading);
        }
    }

    private void UpdateSensor(SensorReading sensorReading)
    {
        string sensorId = sensorReading.readings.sensorId;

        // Find the GameObject with the name matching the sensorId
        GameObject sensorObject = GameObject.Find(sensorId);
        if (sensorObject != null)
        {
            Sensor sensor = sensorObject.GetComponentInChildren<Sensor>();
            if (sensor != null)
            {
                sensor.SetSensorData(sensorReading.readings.temperature, sensorReading.readings.humidityPercent);
            }
        }
        else
        {
            Debug.LogWarning($"Sensor GameObject with name {sensorId} not found.");
        }
    }
}
