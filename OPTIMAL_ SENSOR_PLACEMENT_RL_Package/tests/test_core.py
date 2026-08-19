import pandas as pd
import numpy as np
from collections import Counter

# Import the functions from your package
from opt_rl_package.core import (
    compute_psychrometrics,
    thompson_sampling_mean_crossing,
    prepare_psychro_merged
)

def test_compute_psychrometrics():
    """
    Test the psychrometric calculations with standard room conditions.
    """
    # Given standard conditions: 25°C and 50% Relative Humidity
    T_C, RH = 25.0, 50.0
    
    # Run the function
    dp, W, h, sv = compute_psychrometrics(T_C, RH)
    
    # Assertions to check types
    assert isinstance(dp, float)
    assert isinstance(W, float)
    assert isinstance(h, float)
    assert isinstance(sv, float)
    
    # Basic sanity checks (e.g., dew point should be <= temperature)
    assert dp < T_C 
    assert W > 0
    assert h > 0
    assert sv > 0

def test_thompson_sampling_mean_crossing():
    """
    Test the Thompson Sampling algorithm with a mock DataFrame.
    """
    # Create dummy dataframe mimicking sensor data across 5 timestamps
    data = {
        'A1': [20.5, 22.1, 19.8, 21.0, 23.2],
        'A2': [22.0, 24.5, 21.1, 25.0, 26.3],
        'A3': [18.0, 19.2, 18.5, 20.1, 21.0]
    }
    df = pd.DataFrame(data)
    
    # Run the algorithm
    top_sensors, counts = thompson_sampling_mean_crossing(df)
    
    # Verify the outputs are the correct structure
    assert isinstance(top_sensors, list)
    assert isinstance(counts, Counter)
    
    # Since there are 3 sensors, we should have at most 3 items in our counts
    assert len(counts) <= 3

def test_prepare_psychro_merged():
    """
    Test the data preparation function to ensure it merges coords and calculates variables.
    """
    # 1. Dummy sensor coordinates
    coords_data = {
        'Sensor': ['A1', 'A2'], 
        'X': [0, 10], 
        'Y': [0, 10], 
        'Z': [3, 3]
    }
    sensor_coords = pd.DataFrame(coords_data)
    
    # 2. Dummy environmental data (mimicking the Excel layout)
    env_data = {
        'Date/ Time': ['2020-04-01 12:00', '2020-04-01 12:10'],
        'A1. Temp. (°C)': [25.0, 26.0],
        'A1. Humidity (%)': [50.0, 55.0],
        'A2. Temp. (°C)': [22.0, 23.0],
        'A2. Humidity (%)': [40.0, 45.0]
    }
    df = pd.DataFrame(env_data)
    
    # Run the function
    merged_df = prepare_psychro_merged(df, sensor_coords)
    
    # Verifications
    assert isinstance(merged_df, pd.DataFrame)
    
    # Check if the expected psychrometric columns were successfully generated
    expected_cols = [
        'Sensor', 'X', 'Y', 'Dew Point', 'Humidity Ratio', 
        'Enthalpy', 'Specific Volume', 'Temperature', 'Relative Humidity'
    ]
    for col in expected_cols:
        assert col in merged_df.columns
        
    # We provided data for 2 sensors, so the merged output should have exactly 2 rows
    assert len(merged_df) == 2