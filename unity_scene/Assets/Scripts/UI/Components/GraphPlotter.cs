using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GraphPlotter : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public RectTransform panelRectTransform;

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

    public void FetchAndPlotData(string month)
    {
        StartCoroutine(GetSensorData(month));
    }

    IEnumerator GetSensorData(string month)
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
            PlotTemperatureGraph(sensorData);
        }
    }

    void PlotTemperatureGraph(SensorData sensorData)
    {
        Dictionary<string, List<float>> dailyTemperatures = new Dictionary<string, List<float>>();

        foreach (var sensorReading in sensorData.data.sensorReadings)
        {
            string date = sensorReading.dateTime.Split('T')[0];
            if (!dailyTemperatures.ContainsKey(date))
            {
                dailyTemperatures[date] = new List<float>();
            }

            foreach (var reading in sensorReading.readings)
            {
                dailyTemperatures[date].Add(reading.temperature);
            }
        }

        List<Vector3> points = new List<Vector3>();
        float panelWidth = panelRectTransform.rect.width;
        float panelHeight = panelRectTransform.rect.height;

        int dayIndex = 0;
        foreach (var date in dailyTemperatures.Keys)
        {
            float averageTemperature = 0;
            if (dailyTemperatures[date].Count > 0)
            {
                averageTemperature = CalculateAverage(dailyTemperatures[date]);
            }

            float xPos = (dayIndex / (float)dailyTemperatures.Keys.Count) * panelWidth - panelWidth / 2f;
            float yPos = (averageTemperature / 100f) * panelHeight - panelHeight / 2f; // Assuming max temperature is 100

            points.Add(new Vector3(xPos, yPos, 0f));
            dayIndex++;
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    float CalculateAverage(List<float> values)
    {
        float total = 0f;
        foreach (float value in values)
        {
            total += value;
        }
        return total / values.Count;
    }
}
