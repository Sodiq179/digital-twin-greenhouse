# Blender → Unity Export Guide

This folder contains the **Blender source models**, textures, and exported assets used for the Digital Twin Greenhouse.

---

## 📂 Folder Structure
```
models/     # Original .blend files (editable masters)
textures/   # Associated textures (PNG, JPEG, EXR, etc.)
exports/    # Exported FBX or GLB files ready for Unity import
```

---

## 🛠 Workflow Overview

1. **Modeling in Blender**
   - Create or edit greenhouse geometry and sensor objects in Blender.
   - Keep scale consistent: **Metric units**, Unit Scale = **1.0** (1m = 1 Unity unit).

2. **Texture Setup**
   - Use PBR textures named clearly: `*_BaseColor`, `*_Normal`, `*_Roughness`, `*_Metallic`, `*_AO`.
   - Set color maps to **sRGB**, other maps to **Non-Color Data**.

3. **Export to Unity**
   - File → Export → FBX (preferred) or GLB.
   - **FBX Export Settings:**
     - Main: `Apply Unit` ✓, `Apply Transform` ✓
     - Transform: Forward = **-Z Forward**, Up = **Y Up**
     - Geometry: Smoothing = **Face**
     - Path Mode: **Copy** (keep textures external)
   - Save exports in the `exports/` folder.

4. **Import to Unity**
   - Place exported FBX/GLB files into `unity_scene/Assets/Models/`.
   - In Unity’s FBX importer:
     - Scale Factor = **1.0**
     - Normals = Import
   - Assign materials and link textures.

---

## 🔄 Updating Models
- Always keep original `.blend` files in `models/`.
- Re-export to `exports/` whenever changes are made.
- Use **Git LFS** for large `.blend`, `.fbx`, `.glb`, and texture files.

---

## 📜 Licensing
- All models and textures created in this project are licensed under the same terms as the main repository.
- Document any third-party assets in `docs/asset-attributions.md`.