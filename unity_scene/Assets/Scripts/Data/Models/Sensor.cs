using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Sensor : MonoBehaviour
{
    public TextMeshProUGUI sensorText;

    private void Start()
    {
        // Initialize the text if needed
        if (sensorText != null)
        {
            sensorText.text = "Waiting for data...";
        }
    }

    // public void SetSensorData(float temp, int humidity)
    public void SetSensorData(float temp, float humidity)
    {
        // Update the sensor object's display or behavior based on the temperature and humidity
        if (sensorText != null)
        {
            DateTime currentTime = DateTime.Now;
            sensorText.text = $"Sensor: {gameObject.name}\nTemperature: {temp}°C\nHumidity: {humidity}%\nUpdated at: {currentTime}";
        }
        Debug.Log($"Sensor {gameObject.name} updated - Temperature: {temp}, Humidity: {humidity}");
    }
}
