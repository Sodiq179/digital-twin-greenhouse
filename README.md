# Digital Twin Greenhouse with Thompson Sampling

This repository contains the complete code, Unity 3D environment, and AWS deployment scripts for the paper:

**"A Digital-Twin Framework for Dynamic Sensor Selection in a Strawberry Greenhouse"**

The framework integrates a Unity 3D digital-twin simulation of a 56-node sensor network with a Thompson Sampling-based reinforcement learning (RL) algorithm to dynamically select a minimal yet representative subset of sensors for environmental monitoring.

---

## Overview

### Key Components:
1. **Unity 3D Simulation** – Models the strawberry greenhouse and sensor placement in a 3D environment.
2. **Reinforcement Learning Module** – Implements Thompson Sampling to select active sensors dynamically.
3. **AWS Deployment** – Serverless cloud deployment for real-time decision-making.

---

## Repository Structure

```
digital-twin-greenhouse/
│
├── blender/                    # NEW: source assets + exports
│   ├── models/                 # .blend sources (keep editable masters)
│   ├── textures/               # PNG/EXR; no packed data
│   ├── exports/                # FBX/GLB actually imported into Unity
│   └── README.md               # Blender→Unity export steps (below)
│
├── unity_scene/                   # Unity project files
│   ├── Assets/
│   ├── ProjectSettings/
│   └── Scenes/
│
├── rl_module/                     # Python reinforcement learning module
│   ├── main.py
│   ├── thompson_sampling.py
│   ├── utils/
│   ├── requirements.txt
│   └── README.md
│
├── aws_deployment/                # AWS Lambda / serverless configuration
│   ├── serverless.yml
│   ├── lambda_function.py
│   └── README.md
│
├── data/                          # Example dataset (synthetic)
│   ├── sensor_metadata.csv
│   └── README.md
│
├── docs/                          # Documentation and figures
│   ├── block_diagram.png
│   └── system_overview.md
│
├── LICENSE
├── README.md                      # Main project description
└── CODE_AVAILABILITY.md           # Statement to match manuscript section
```

---

##  Installation & Setup

### 1️⃣ Unity Environment
1. Install [Unity Hub](https://unity.com/download) (tested on **Unity 2022.x**).
2. Open the `unity_scene/` folder as a Unity project.
3. Open and run `MainScene.unity` to launch the simulation.

---

### 2️⃣ Python RL Module
#### Install Dependencies:
```bash
cd rl_module
pip install -r requirements.txt
```

#### Run the RL Simulation:
```bash
python main.py --episodes 10
```
This will:
- Simulate 10 episodes of sensor selection.
- Output selected sensors, energy savings, and coverage accuracy.

---

### 3️⃣ AWS Deployment
#### Requirements:
- [Serverless Framework](https://www.serverless.com/framework/docs/getting-started/)
- AWS account and credentials (not included in repo).

#### Deployment:
```bash
cd aws_deployment
npm install -g serverless
serverless deploy
```

---

## Example Output

Sample console output after running `main.py`:
```
Episode 1: Selected sensors = [3, 14, 25, 41, 55]
Coverage Accuracy: 94.8%
```

---

## Code Availability Statement

The complete Unity 3D simulation environment, Python RL module, and AWS deployment scripts are provided for peer review in this private repository. Upon publication, the repository will be made publicly accessible under the MIT License.

---

## License
This repository is licensed under the MIT License – see the `LICENSE` file for details.

---

##  Contact
**Sodiq Damilola Babawale**  
Email: babawalesodiq996@gmail.com 
Affiliation: University of Ibadan
