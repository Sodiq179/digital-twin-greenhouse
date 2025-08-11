using UnityEngine;
using TMPro;

public class DropdownScript : MonoBehaviour
{
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private OptimalSensorsData optimalSensorsData;
    [SerializeField] private SensorDataFetcher sensorDataFetcher;

    //[SerializeField] private GraphPlotter graphPlotter;

    public void DropdownSample(int index)
    {
        string month = "";
        int monthDigit = 0;

        switch (index)
        {
            case 0: month = "February"; monthDigit = 2; break;
            case 1: month = "March"; monthDigit = 3; break;
            case 2: month = "April"; monthDigit = 4; break;
            case 3: month = "May"; monthDigit = 5; break;
            case 4: month = "June"; monthDigit = 6; break;
            case 5: month = "July"; monthDigit = 7; break;
            // Add cases for other months if needed
        }

        numberText.text = $"Month: {month}";
        optimalSensorsData.FetchData(month);

        if (SceneDataManager.Instance != null)
        {
            SceneDataManager.Instance.SelectedMonth = monthDigit;
            //SceneDataManager.Instance.SelectedMonthStr = month;
        }

        sensorDataFetcher.FetchSensorData(monthDigit, month);
        //graphPlotter.FetchAndPlotData(month);
    }
}
