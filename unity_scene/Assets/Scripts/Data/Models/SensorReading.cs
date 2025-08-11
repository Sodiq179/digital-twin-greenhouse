using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SensorReading
{
    public string dateTime;
    public Reading readings;
}

[System.Serializable]
public class Reading
{
    public string sensorId;
    public float temperature;
    public int humidityPercent;
}