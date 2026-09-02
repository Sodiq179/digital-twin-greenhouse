import os
import re
import random
from collections import Counter

import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from scipy.interpolate import griddata
from mpl_toolkits.mplot3d import Axes3D


# 1. CORE MATH & ALGORITHMS


def compute_psychrometrics(T_C, RH):
    """
    Computes psychrometric properties given temperature and relative humidity.

    This function calculates the dew point, humidity ratio, enthalpy, and 
    specific volume assuming a standard atmospheric pressure of 101.325 kPa.

    Parameters
    ----------
    T_C : float or array-like
        Temperature in degrees Celsius (°C).
    RH : float or array-like
        Relative humidity expressed as a percentage (%).

    Returns
    -------
    dew_point : float or array-like
        Dew point temperature in degrees Celsius (°C).
    W : float or array-like
        Humidity ratio (kg of water vapor per kg of dry air).
    h : float or array-like
        Enthalpy of the moist air (kJ/kg).
    sv : float or array-like
        Specific volume of the moist air (m³/kg).
    """
    P_atm = 101.325  # kPa
    T_K = T_C + 273.15
    A = 6.1121
    m = 17.62
    Tn = 243.12

    Pws = A * np.exp((m * T_C) / (T_C + Tn))
    Pw = RH / 100.0 * Pws

    alpha = np.log(Pw / A)
    dew_point = (Tn * alpha) / (m - alpha)
    W = 0.622 * (Pw / (P_atm * 10 - Pw))
    h = 1.006 * T_C + W * (2501 + 1.86 * T_C)
    sv = (0.287042 * T_K * (1 + 1.6078 * W)) / P_atm

    return dew_point, W, h, sv

def thompson_sampling_mean_crossing(df):
    """
    Applies Thompson Sampling to rank and select optimal sensors based on mean-crossing rewards.

    This reinforcement learning function evaluates sensor data by calculating the mean 
    threshold for each sensor column. It assigns a binary reward of 1 if the sensor's reading 
    is above or below the mean, and 0 otherwise. Using a Beta distribution to balance 
    exploration and exploitation, it iterates through the observations to determine the most 
    representative sensors.

    Parameters
    ----------
    df : pandas.DataFrame
        A DataFrame containing continuous sensor readings. Rows represent individual 
        observations (timestamps) and columns represent the individual sensors (arms).

    Returns
    -------
    top_sensors : list of tuples
        A list of tuples structured as `(sensor_index, selection_count)`, sorted in 
        descending order from the most to least frequently selected sensor.
    counts : collections.Counter
        A Counter object mapping each specific `sensor_index` to its total number 
        of selections during the sampling process.
    """
    N, d = df.shape
    if N == 0 or d == 0:
        return [], Counter()

    selected_sensors = []
    rewards_1 = [0] * d
    rewards_0 = [0] * d

    # Precompute mean thresholds
    thresholds = df.mean()

    # Determine if each value is either above or below the mean
    crossing_flags = (df.gt(thresholds, axis=1) | df.lt(thresholds, axis=1)).astype(int)

    for n in range(N):
        ad = 0
        max_beta = 0
        for i in range(d):
            beta_sample = random.betavariate(rewards_1[i] + 1, rewards_0[i] + 1)
            if beta_sample > max_beta:
                ad = i
                max_beta = beta_sample
        selected_sensors.append(ad)

        reward = crossing_flags.iloc[n, ad]  # 1 if crossing, else 0
        if reward == 1:
            rewards_1[ad] += 1
        else:
            rewards_0[ad] += 1

    counts = Counter(selected_sensors)
    return counts.most_common(), counts


# 2. DATA PROCESSING & VISUALIZATION


def prepare_psychro_merged(df, sensor_coords):
    """
    Calculates average psychrometric variables from time-series data and merges 
    them with sensor spatial coordinates.

    This function scans the input DataFrame for temperature and humidity columns 
    associated with specific sensors. It calculates the corresponding dew point, 
    humidity ratio, enthalpy, and specific volume, averages these values across 
    the time-series, and joins the aggregated results with the physical sensor coordinates.

    Parameters
    ----------
    df : pandas.DataFrame
        Time-series environmental data. Columns must be formatted specifically as 
        `<Sensor_ID>. Temp. (°C)` and `<Sensor_ID>. Humidity (%)` for the 
        function to successfully extract the data.
    sensor_coords : pandas.DataFrame
        Spatial coordinate data for the sensors. Must contain a `Sensor` column 
        (matching the `<Sensor_ID>` from `df`) to act as the merge key, typically 
        accompanied by `X`, `Y`, and `Z` coordinate columns.

    Returns
    -------
    merged_df : pandas.DataFrame
        A single DataFrame containing the original sensor coordinates seamlessly 
        merged with the averaged temperature, relative humidity, and computed 
        psychrometric variables for each detected sensor.
    """
    psychro_vars, avg_temp_hum = [], []
    for col in [c.split('.')[0] for c in df.columns if '. Temp' in c]:
        temp_col = f'{col}. Temp. (°C)'
        hum_col = f'{col}. Humidity (%)'
        if temp_col in df.columns and hum_col in df.columns:
            T, RH = df[temp_col], df[hum_col]
            dp, W, h, sv = compute_psychrometrics(T, RH)
            psychro_vars.append({'Sensor': col, 'Dew Point': dp.mean(), 'Humidity Ratio': W.mean(), 'Enthalpy': h.mean(), 'Specific Volume': sv.mean()})
            avg_temp_hum.append({'Sensor': col, 'Temperature': T.mean(), 'Relative Humidity': RH.mean()})
            
    df_psychro = pd.DataFrame(psychro_vars)
    df_avg = pd.DataFrame(avg_temp_hum)
    merged = pd.merge(sensor_coords, df_psychro, on='Sensor')
    return pd.merge(merged, df_avg, on='Sensor')

def plot_selected_months(selected_data, variable, label):
    """
    Generates 3D surface plots for a selected environmental variable across different seasonal months.

    This function iterates through a dictionary of seasonal datasets, extracts the spatial 
    coordinates (X and Y) alongside the specified variable, and applies cubic interpolation 
    to map the data onto a 50x50 3D mesh grid. It generates a 2x2 subplot figure to display 
    the resulting surfaces side-by-side.

    Parameters
    ----------
    selected_data : dict
        A dictionary where keys are string labels (e.g., 'February (Winter)') and values 
        are pandas DataFrames. Each DataFrame must contain 'X', 'Y', and the target `variable` columns.
    variable : str
        The exact column name in the DataFrames representing the metric to map onto the 
        Z-axis (e.g., 'Temperature', 'Humidity Ratio').
    label : str
        The formatted string used for the Z-axis label and the individual subplot titles 
        (e.g., 'Temperature (°C)').

    Returns
    -------
    None
        This function does not return an object. It directly renders and displays the 
        matplotlib 3D surface figure.
    """
    fig = plt.figure(figsize=(18, 14))
    for i, (month_label, data) in enumerate(selected_data.items()):
        x, y, z = data['X'].values, data['Y'].values, data[variable].values
        grid_x, grid_y = np.mgrid[min(x):max(x):50j, min(y):max(y):50j]
        grid_z = griddata((x, y), z, (grid_x, grid_y), method='cubic')
        ax = fig.add_subplot(2, 2, i + 1, projection='3d')
        surf = ax.plot_surface(grid_x, grid_y, grid_z, cmap='viridis', edgecolor='k', linewidth=0.3)
        ax.set_title(f'{month_label} - {label}')
        ax.set_xlabel('X (m)')
        ax.set_ylabel('Y (m)')
        ax.set_zlabel(label)
        fig.colorbar(surf, ax=ax, shrink=0.5, aspect=10)
    plt.tight_layout()
    plt.show()


# 3. HIGH-LEVEL PIPELINES

def load_and_plot_seasonal_data(sensor_coord_path, monthly_files_dict):
    """
    Loads seasonal data, merges psychrometric calculations, and plots the environmental variables.

    This pipeline function automates the data ingestion and visualization process. It reads 
    sensor spatial coordinates from an Excel file, iterates through a dictionary of seasonal 
    environmental datasets, and merges them using psychrometric calculations. Finally, it 
    automatically generates 3D surface plots for Temperature, Relative Humidity, Dew Point, 
    Humidity Ratio, Enthalpy, and Specific Volume before returning the combined dataset.

    Parameters
    ----------
    sensor_coord_path : str
        The local file path to the Excel file containing the sensor coordinates.
    monthly_files_dict : dict
        A dictionary mapping string labels to their respective Excel file paths 
        (e.g., {'February (Winter)': 'path/to/Smartfarm_Feb.xlsx'}).

    Returns
    -------
    seasonal_data : dict
        A dictionary where the keys are the string labels provided in `monthly_files_dict` 
        and the values are the fully processed pandas DataFrames containing both the 
        spatial coordinates and the computed psychrometric variables.
    """
    sensor_coords = pd.read_excel(sensor_coord_path)
    sensor_coords['Sensor'] = sensor_coords['Sensor'].str.strip()

    seasonal_data = {}

    for label, filepath in monthly_files_dict.items():
        df = pd.read_excel(filepath, sheet_name="Sheet1")
        df.columns = df.columns.str.strip().str.replace('  ', ' ', regex=False)
        df['Date/ Time'] = pd.to_datetime(df['Date/ Time'])
        merged = prepare_psychro_merged(df, sensor_coords)
        seasonal_data[label] = merged

    variables = ['Temperature', 'Relative Humidity', 'Dew Point', 'Humidity Ratio', 'Enthalpy', 'Specific Volume']
    titles = ['Temperature (°C)', 'Relative Humidity (%)', 'Dew Point (°C)',
              'Humidity Ratio (kg/kg dry air)', 'Enthalpy (kJ/kg)', 'Specific Volume (m³/kg)']

    for var, title in zip(variables, titles):
        plot_selected_months(seasonal_data, var, title)
        
    return seasonal_data

def run_thompson_sampling_pipeline(input_folder, output_folder, random_seed=10, plot_visuals=False):
    """
    Batch processes seasonal environmental data to identify and export optimal sensor placements.

    This high-level pipeline function iterates through a designated folder of combined 
    CSV files, applies the Thompson Sampling algorithm using a mean-crossing reward 
    mechanism, and ranks the most representative sensors for key psychrometric variables. 
    It automatically saves the ranked results into both an overarching CSV file and 
    a multi-sheet Excel workbook, with an option to visualize the selection frequencies.

    Parameters
    ----------
    input_folder : str
        The local directory path containing the target '_Combined_FULL.csv' files.
    output_folder : str
        The local directory path where the resulting Excel and CSV reports will be saved.
    random_seed : int, optional
        A numerical seed to ensure the Thompson Sampling results are reproducible (default is 10).
    plot_visuals : bool, optional
        If True, the pipeline will generate and display matplotlib bar charts showing 
        the sensor selection frequencies for each parameter and season (default is False).

    Returns
    -------
    final_df : pandas.DataFrame or None
        A DataFrame containing the consolidated ranking results across all seasons and 
        parameters. Returns None if no valid combined CSV files are found in the input directory.
    """
    os.makedirs(output_folder, exist_ok=True)
    combined_files = [f for f in os.listdir(input_folder) if f.endswith('_Combined_FULL.csv')]
    
    if not combined_files:
        print("⚠️ No combined files found.")
        return None

    all_results = []
    random.seed(random_seed)

    excel_path = os.path.join(output_folder, "All_Seasons_Top_Sensors.xlsx")
    csv_path = os.path.join(output_folder, "All_Seasons_Top_Sensors.csv")

    with pd.ExcelWriter(excel_path, engine='xlsxwriter') as writer:
        for file in combined_files:
            season = file.replace('_Combined_FULL.csv', '')
            print(f"\n🔍 Processing: {season}")

            df = pd.read_csv(os.path.join(input_folder, file))
            df.columns = df.columns.str.replace(r"[\.\s]+", "", regex=True)

            if df.empty:
                print(f"⚠️ Skipping {season} due to empty data.")
                continue

            season_results = []

            for param in ['Temperature', 'Humidity', 'DewPoint', 'HumidityRatio', 'Enthalpy', 'SpecificVolume']:
                param_cols = [col for col in df.columns if param.lower() in col.lower()]
                if not param_cols:
                    continue

                clean_param_cols = []
                for col in param_cols:
                    match = re.search(r'([A-Ha-h][0-9]+)', col)
                    if match:
                        clean_param_cols.append(col)

                if not clean_param_cols:
                    continue

                param_df = df[clean_param_cols]

                # Clean Data
                param_df = param_df.dropna()
                param_df = param_df[(param_df != 0).any(axis=1)]

                if param_df.empty:
                    print(f"⚠️ Skipping {param} in {season} due to invalid or empty sensor data.")
                    continue

                # Run Thompson Sampling
                top_sensors, full_counts = thompson_sampling_mean_crossing(param_df)

                if not top_sensors:
                    continue

                sensor_mapping = {idx: re.search(r'([A-Ha-h][0-9]+)', col).group(1) for idx, col in enumerate(clean_param_cols)}

                for rank, (idx, count) in enumerate(top_sensors, start=1):
                    result_row = {
                        'Season': season,
                        'Parameter': param,
                        'Rank': rank,
                        'Sensor': sensor_mapping.get(idx, f"Sensor{idx}"),
                        'Selection Count': count
                    }
                    season_results.append(result_row)
                    all_results.append(result_row)

                # Plot Selection Frequency (Optional)
                if plot_visuals:
                    plt.figure(figsize=(10, 6))
                    plt.bar([sensor_mapping[i] for i in full_counts.keys()], full_counts.values(), color='teal')
                    plt.title(f"{season} - {param} Sensor Selection (Mean Crossings Only)")
                    plt.xlabel("Sensor ID")
                    plt.ylabel("Selection Count")
                    plt.tight_layout()
                    plt.show()

            if season_results:
                pd.DataFrame(season_results).to_excel(writer, sheet_name=season[:31], index=False)

    # Save Combined Output
    final_df = pd.DataFrame()
    if all_results:
        final_df = pd.DataFrame(all_results)
        final_df.to_csv(csv_path, index=False)
        print(f"\n✅ All results saved to:\n{excel_path}\n{csv_path}")
        print("Thompson Sampling with mean crossing rewards and cleaned data completed.")
        
    return final_df