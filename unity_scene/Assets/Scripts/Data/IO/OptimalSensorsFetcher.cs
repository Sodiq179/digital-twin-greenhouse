using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class OptimalSensorsFetcher : MonoBehaviour
{
    private void Start()
    {
        int selectedMonth = SceneDataManager.Instance != null ? SceneDataManager.Instance.SelectedMonth : 0;
        string monthName = GetMonthName(selectedMonth);
        if (!string.IsNullOrEmpty(monthName))
        {
            StartCoroutine(FetchOptimalSensors(monthName));
        }
        else
        {
            Debug.LogError("Invalid month selected.");
        }
    }

    private IEnumerator FetchOptimalSensors(string month)
    {
        string url = $"https://43kygdipy5.execute-api.eu-north-1.amazonaws.com/Prod/Values?month={month}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");
            }
            else
            {
                string response = webRequest.downloadHandler.text;
                Debug.Log($"Received Optimal Sensor IDs: {response}");

                ProcessOptimalSensors(response);
            }
        }
    }

    private void ProcessOptimalSensors(string response)
    {
        var outerObject = JsonUtility.FromJson<OuterObject>(response);
        var bodyObject = JsonUtility.FromJson<BodyObject>(outerObject.body);

        var temperatureSensorIds = bodyObject.temperature.Split(new[] { ',', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        var humiditySensorIds = bodyObject.humidity.Split(new[] { ',', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        var temperatureSensorSet = new HashSet<string>(temperatureSensorIds);
        var humiditySensorSet = new HashSet<string>(humiditySensorIds);

        // Color temperature sensors red
        foreach (string sensorId in temperatureSensorIds)
        {
            UpdateSensorColor(sensorId, Color.red);
        }

        // Color humidity sensors green and check for sensors in both lists
        foreach (string sensorId in humiditySensorIds)
        {
            if (temperatureSensorSet.Contains(sensorId))
            {
                UpdateSensorColor(sensorId, Color.magenta); // Purple for sensors in both
            }
            else
            {
                UpdateSensorColor(sensorId, Color.green);
            }
        }
    }

    private void UpdateSensorColor(string sensorId, Color color)
    {
        // Find the GameObject with the name matching the sensorId
        GameObject sensorObject = GameObject.Find(sensorId);
        if (sensorObject != null)
        {
            Renderer sensorRenderer = sensorObject.GetComponent<Renderer>();
            if (sensorRenderer != null)
            {
                sensorRenderer.material.color = color;
            }
        }
        else
        {
            Debug.LogWarning($"Sensor GameObject with name {sensorId} not found.");
        }
    }

    private string GetMonthName(int month)
    {
        switch (month)
        {
            case 2: return "February";
            case 3: return "March";
            case 4: return "April";
            case 5: return "May";
            case 6: return "June";
            case 7: return "July";
            default: return string.Empty; // Or handle invalid month
        }
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
}
