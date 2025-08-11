using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class OptimalSensorsData : MonoBehaviour
{
    public TextMeshProUGUI sensorTextDataTemp;
    public TextMeshProUGUI sensorTextDataTempCount;
    public TextMeshProUGUI sensorTextDataHumidity;
    public TextMeshProUGUI sensorTextDataHumidityCount;
    public TextMeshProUGUI sensorOptimal; // New TextMeshProUGUI element

    public void FetchData(string month)
    {
        StartCoroutine(FetchDataCoroutine(month));
    }

    private IEnumerator FetchDataCoroutine(string month)
    {
        string url = $"https://43kygdipy5.execute-api.eu-north-1.amazonaws.com/Prod/Values?month={month}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError ||
                webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                try
                {
                    string jsonResponse = webRequest.downloadHandler.text;
                    Debug.Log($"Received Data: {jsonResponse}");
                    ProcessData(jsonResponse);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Processing Error: {e.Message}");
                }
            }
        }
    }

    private void ProcessData(string json)
    {
        try
        {
            var outerObject = JsonUtility.FromJson<OuterObject>(json);
            var bodyObject = JsonUtility.FromJson<BodyObject>(outerObject.body);

            if (bodyObject == null || string.IsNullOrEmpty(bodyObject.temperature) || string.IsNullOrEmpty(bodyObject.humidity))
            {
                Debug.LogError("Invalid Data Format");
                return;
            }

            var temperatureData = bodyObject.temperature.Split(',');
            int tempCount = temperatureData.Length;

            var humidityData = bodyObject.humidity.Split(',');
            int humidityCount = humidityData.Length;

            if (tempCount == humidityCount)
            {
                sensorTextDataTempCount.text = "Avg. Temp";
                sensorTextDataHumidityCount.text = "Avg. Rel. Hum.";
                sensorOptimal.text = $"Optimal Sensors. ({tempCount})";
            }
            else
            {
                sensorTextDataTempCount.text = $"Avg. Temp. ({tempCount})";
                sensorTextDataHumidityCount.text = $"Avg. Rel. Hum. ({humidityCount})";
                sensorOptimal.text = $"Optimal Sensors";
            }

            sensorTextDataTemp.text = $"Number of Sensors: {tempCount}\n\nTemperature Sensors: \n{string.Join(", ", temperatureData)}";
            sensorTextDataHumidity.text = $"Number of Sensors: {humidityCount}\n\nHumidity Sensors: \n{string.Join(", ", humidityData)}";

            Debug.Log($"Optimal Temperature Sensors: {string.Join(", ", temperatureData)}");
            Debug.Log($"Optimal Humidity Sensors: {string.Join(", ", humidityData)}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing JSON data: {e.Message}");
        }
    }

    [Serializable]
    public class OuterObject
    {
        public int statusCode;
        public string body;
        public Headers headers;
    }

    [Serializable]
    public class Headers
    {
        public string ContentType;
    }

    [Serializable]
    public class BodyObject
    {
        public string temperature;
        public string humidity;
    }
}
