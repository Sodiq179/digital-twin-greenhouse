using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class SensorDataFetcher : MonoBehaviour
{
    public TextMeshProUGUI temperatureText;
    public TextMeshProUGUI humidityText;
    public TextMeshProUGUI filteredTemperatureText; // New text object for filtered temperature
    public TextMeshProUGUI filteredHumidityText; // New text object for filtered humidity

    private List<string> temperatureSensorIdsToFilter;
    private List<string> humiditySensorIdsToFilter;

    [System.Serializable]
    public class Reading
    {
        public string sensorId;
        public float temperature;
        public float humidityPercent;
    }

    [System.Serializable]
    public class SensorReading
    {
        public string _id;
        public string dateTime;
        public List<Reading> readings;
    }

    [System.Serializable]
    public class SensorData
    {
        public string status;
        public int results;
        public Data data;
    }

    [System.Serializable]
    public class Data
    {
        public List<SensorReading> sensorReadings;
    }

    [System.Serializable]
    public class OuterObject
    {
        public int statusCode;
        public string body;
        public Headers headers;
    }

    [System.Serializable]
    public class Headers
    {
        public string ContentType;
    }

    [System.Serializable]
    public class BodyObject
    {
        public string temperature;
        public string humidity;
    }

    public void FetchSensorData(int month, string monthString)
    {
        StartCoroutine(GetSensorData(month, monthString));
    }

    private IEnumerator GetSensorData(int month, string monthString)
    {
        string url = $"http://52.55.28.0:4000/sensors/sensor_data/2020/{month}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            SensorData sensorData = JsonUtility.FromJson<SensorData>(request.downloadHandler.text);
            CalculateAndDisplayAverages(sensorData);
            yield return StartCoroutine(FetchSensorIdsAndCalculateFilteredAverages(monthString, sensorData));
        }
    }

    private void CalculateAndDisplayAverages(SensorData sensorData)
    {
        float totalTemperature = 0f;
        float totalHumidity = 0f;
        int totalCount = 0;

        foreach (var sensorReading in sensorData.data.sensorReadings)
        {
            foreach (var reading in sensorReading.readings)
            {
                totalTemperature += reading.temperature;
                totalHumidity += reading.humidityPercent;
                totalCount++;
            }
        }

        float averageTemperature = totalTemperature / totalCount;
        float averageHumidity = totalHumidity / totalCount;

        temperatureText.text = $"{averageTemperature:F2}°C";
        humidityText.text = $"{averageHumidity:F2}%";
    }

    private IEnumerator FetchSensorIdsAndCalculateFilteredAverages(string month, SensorData sensorData)
    {
        string url = $"https://43kygdipy5.execute-api.eu-north-1.amazonaws.com/Prod/Values?month={month}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            OuterObject response = JsonUtility.FromJson<OuterObject>(request.downloadHandler.text);
            BodyObject bodyObject = JsonUtility.FromJson<BodyObject>(response.body);
            temperatureSensorIdsToFilter = bodyObject.temperature.Split(',').Select(id => id.Trim()).ToList();
            humiditySensorIdsToFilter = bodyObject.humidity.Split(',').Select(id => id.Trim()).ToList();
            CalculateAndDisplayFilteredAverages(sensorData);
        }
    }

    private void CalculateAndDisplayFilteredAverages(SensorData sensorData)
    {
        float totalTemperature = 0f;
        float totalHumidity = 0f;
        int totalTemperatureCount = 0;
        int totalHumidityCount = 0;

        foreach (var sensorReading in sensorData.data.sensorReadings)
        {
            foreach (var reading in sensorReading.readings)
            {
                if (temperatureSensorIdsToFilter.Contains(reading.sensorId))
                {
                    totalTemperature += reading.temperature;
                    totalTemperatureCount++;
                }
                if (humiditySensorIdsToFilter.Contains(reading.sensorId))
                {
                    totalHumidity += reading.humidityPercent;
                    totalHumidityCount++;
                }
            }
        }

        if (totalTemperatureCount > 0)
        {
            float averageTemperature = totalTemperature / totalTemperatureCount;
            filteredTemperatureText.text = $"{averageTemperature:F2}°C";
        }
        else
        {
            filteredTemperatureText.text = "No data";
        }

        if (totalHumidityCount > 0)
        {
            float averageHumidity = totalHumidity / totalHumidityCount;
            filteredHumidityText.text = $"{averageHumidity:F2}%";
        }
        else
        {
            filteredHumidityText.text = "No data";
        }
    }
}
