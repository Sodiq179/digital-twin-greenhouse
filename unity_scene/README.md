# Unity Greenhouse Monitoring Project

This Unity project implements a **greenhouse monitoring system** with MQTT integration for receiving sensor data, 3D greenhouse models, and UI interaction scripts.

## 📂 Project Structure

```
unity_scene/
├── Assets/
│   ├── HDRPDefaultResources/               # HDRP defaults (keep only if using HDRP)
│   ├── Materials/                          # Materials used by models/scenes
│   ├── Models/                             # FBX/GLB assets
│   ├── Plugins/                            # Third‑party code (e.g., MQTT libs)
│   ├── Scenes/                             # Your Unity scenes
│   ├── Scripts/                            # C# scripts
│   ├── TextMesh Pro/                       # TMP package assets
│   └── UniversalRenderPipelineGlobalSettings.asset  # URP global settings (if using URP)
├── Packages/                               # Package Manager manifest/lock
├── ProjectSettings/                        # Project configuration
└── UserSettings/                           # Editor prefs (optional)
```

> Note: `Library/`, `Temp/`, `Logs/` are intentionally **not** in version control; Unity regenerates them.

## 🛠 Requirements

- Unity **2021.3 LTS** or later
- HDRP or URP depending on your graphics pipeline setup
- MQTT broker for receiving sensor data

## ⚙️ Setup Instructions

1. Clone this repository:
   ```bash
   git clone <repo-url>
   ```
2. Open the project in Unity.
3. If prompted, allow Unity to regenerate the `Library/` folder (it’s excluded from version control).
4. Open a scene from the `Scenes/` folder to start.
5. Configure MQTT settings in the relevant scripts under `Scripts/Integration/`.

## 🚀 Features

- **MQTT Integration**: Receives real-time sensor data.
- **3D Models**: Includes greenhouse, strawberry plants, and sensor models.
- **UI Components**: Display live sensor readings and status.
- **Terrain Assets**: For creating realistic farm/greenhouse environments.

## 📜 Notes

- The `Library/` folder is excluded because Unity auto-generates it.
- Large build or cache files are not stored in this repository.
- To keep the repo lightweight, only essential assets and source code are included.