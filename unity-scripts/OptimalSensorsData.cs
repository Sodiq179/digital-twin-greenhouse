using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class OptimalSensorsData : MonoBehaviour
{
    public TextMeshProUGUI sensorTextData;
    private void Start()
    {
        StartCoroutine(FetchData());
    }

    private IEnumerator FetchData()
    {
        String url = "https://43kygdipy5.execute-api.eu-north-1.amazonaws.com/Prod/Values?month=July";

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
                string jsonResponse = webRequest.downloadHandler.text;
                sensorTextData.text = "Received Data:" + jsonResponse;
                Debug.Log($"Received Data: {jsonResponse}");

                ProcessData(jsonResponse);
            }
        }
    }

    private void ProcessData(string json)
    {
        // Parse the outer JSON object
        var outerObject = JsonUtility.FromJson<OuterObject>(json);

        // Parse the "body" field which is another JSON string
        var bodyObject = JsonUtility.FromJson<BodyObject>(outerObject.body);

        // Access the sensor readings
        var sensorData = bodyObject.selected_values;

        // Print and update sensor readings
        Debug.Log($"Optimal Sensors: {string.Join(", ", sensorData)}");
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
        public string month;
        public string[] selected_values;
    }
}