


# Digital Twin Greenhouse with Thompson Sampling

This repository contains the complete code, Unity 3D environment, and documentation for the paper:

**"A Digital-Twin Framework for Dynamic Sensor Selection in a Strawberry Greenhouse"**

The framework integrates a Unity 3D digital-twin simulation of a 56-node sensor network with a Thompson Sampling-based reinforcement learning (RL) model to dynamically select a minimal yet representative subset of sensors for environmental monitoring.

---

## Overview

### Key Components
1. **Unity 3D Simulation** – Models the strawberry greenhouse and sensor placement in a 3D environment.  
2. **Reinforcement Learning Notebook** – A single Jupyter Notebook implementing Thompson Sampling for sensor selection.  

---

## Repository Structure

```

digital-twin-greenhouse/
│
├── blender/                    # Source assets + exports
│   ├── models/                 # .blend sources
│   ├── textures/               # PNG/EXR
│   ├── exports/                # FBX/GLB imported into Unity
│   └── README.md
│
├── unity\_scene/                # Unity project files
│   ├── Assets/
│   ├── ProjectSettings/
│   └── Scenes/
│
├── rl\_notebook/                # Reinforcement learning module
│   └── thompson\_sampling.ipynb
│
│
├── LICENSE
├── README.md
└── CODE\_AVAILABILITY.md

````

---

## Installation & Setup

### 1. Unity Environment
1. Install [Unity Hub](https://unity.com/download) (tested on **Unity 2022.x**).  
2. Open the `unity_scene/` folder as a Unity project.  
3. Run `MainScene.unity` to launch the simulation.  

---

### 2. Reinforcement Learning Notebook
Open the Jupyter Notebook:  

```bash
jupyter notebook rl_notebook/thompson_sampling.ipynb
````




---

## Code Availability Statement

The Unity 3D simulation environment, Jupyter Notebook (reinforcement learning model), and supporting documentation are provided for **peer review** in this private repository.
Upon publication, the repository will be made publicly accessible under the MIT License.

---

## License

This repository is licensed under the MIT License – see the `LICENSE` file for details.

---

## Contact

**Sodiq Damilola Babawale**
Email: [babawalesodiq996@gmail.com](mailto:babawalesodiq996@gmail.com)
Affiliation: University of Ibadan

```

---


```
