# Digital Twin Greenhouse with Thompson Sampling

This repository contains the complete code, Unity 3D environment, and documentation for the paper:

**"A Digital-Twin Framework for Dynamic Sensor Selection in a Strawberry Greenhouse"**

The framework integrates a Unity 3D digital-twin simulation of a 56-node sensor network with a Thompson Sampling-based reinforcement learning (RL) model to dynamically select a minimal yet representative subset of sensors for environmental monitoring.

---

## Overview

### Key Components
1. **Unity 3D Simulation** – Models the strawberry greenhouse and sensor placement in a 3D environment.  
2. **Reinforcement Learning Notebook** – A single Jupyter Notebook implementing Thompson Sampling for sensor selection. 
3. **Python Optimization Package (`opt_rl_package`)** – A pip-installable Python package extracted from the notebook that streamlines psychrometric calculations, 3D visualizations, and the Thompson Sampling pipeline for use in external scripts.

---

## Repository Structure

```text
digital-twin-greenhouse/
│
├── blender/                     # Source assets + exports
│   ├── models/                  # .blend sources
│   ├── textures/                # PNG/EXR
│   ├── exports/                 # FBX/GLB imported into Unity
│   └── README.md
│
├── unity_scene/                 # Unity project files
│   ├── Assets/
│   ├── ProjectSettings/
│   └── Scenes/
│
├── opt_rl_package/              # Installable Python package
│   ├── src/opt_rl_package/      # Core package logic (core.py)
│   ├── tests/                   # Pytest unit tests
│   └── pyproject.toml           # Package build configuration
│
├── LICENSE
├── README.md
└── CODE_AVAILABILITY.md
