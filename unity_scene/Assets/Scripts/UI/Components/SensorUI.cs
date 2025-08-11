using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensorUI : MonoBehaviour
{
    
    public TextMeshProUGUI sensorText;

    public void SetSensorData(string sensorId, float temperature, int humidityPercent)
    {
        sensorText.text = $"Sensor: {sensorId}\nTemperature: {temperature}°C\nHumidity: {humidityPercent}%";
    }
}
