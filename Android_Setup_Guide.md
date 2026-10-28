# Android setup guide

## Important
This repository is a starter-code kit, not a complete Unity project or compiled APK. A Unity Editor environment is required to assemble the scene, test the physics, and build an Android package. GitHub Codespaces by itself is not a drop-in Unity Editor.

## 1. Create the Unity project
1. Install Unity 2022.3 LTS or a compatible Unity 6 version.
2. Include **Android Build Support**, **Android SDK & NDK Tools**, and **OpenJDK**.
3. Create a **3D (URP)** project.
4. Copy this repository's `Assets` folder into the new Unity project's root, merging folders if prompted.
5. Open Unity and resolve any script compilation errors.

## 2. Make a simple test scene
1. Create a large visual ocean surface at Y=0. For initial testing, disable its collider so it does not physically block the boat.
2. Create a `PlayerBoat` root with a Rigidbody, hull collider, `BoatController`, and `BoatBuoyancy`. Point the boat's bow toward local +Z.
3. Set Rigidbody mass near 1000, enable Interpolate, and tune drag.
4. Add four empty child transforms around the hull near the waterline and assign them to the buoyancy points.
5. Add a Camera with `BoatCameraFollow` and assign `PlayerBoat` as Target.
6. Add one `BoatGameManager` to an empty GameObject.
7. Add trigger buoys with `MissionBuoy` and trigger volumes with `ExplorationZone`.

## 3. Mobile touch controls
1. Create a Canvas set to **Screen Space - Overlay** and an EventSystem.
2. Add four large UI Buttons: Left, Right, Throttle, Reverse.
3. Add `MobileBoatControls` to an empty GameObject and assign the boat.
4. Add `HoldButton` to each UI Button, assign the controls object, and select the matching action in the Inspector.
5. Use a Canvas Scaler set to **Scale With Screen Size** and landscape orientation.

## 4. Upgrades
Add `BoatUpgradeSystem` to the boat. UI buttons can call `PurchaseEngineUpgrade`, `PurchaseHandlingUpgrade`, `UnlockSpeedboat`, and `UnlockFishingBoat`. This starter implementation keeps progress in memory only; persistent saving and actual boat selection still need to be implemented.

## 5. Cinematic graphics direction
Use a compatible URP water solution with wave normals/reflections, a high-quality skybox, directional sunlight, shoreline terrain, distant islands, detailed licensed boat models, wake/spray particles, and engine sounds. Begin with mobile-friendly settings, LODs, limited shadow distance, and 30 FPS; profile on the target phone. `CinematicOceanMotion` is only subtle visual movement, not a true ocean simulation.

## 6. Build an Android APK
1. In Unity, select Android under Build Settings / Build Profiles and switch platform.
2. Set a unique package identifier, for example `com.yourstudio.saltwaterboatsim`.
3. Set orientation to Landscape.
4. Configure test/debug signing for local tests. For Google Play release, use a secure release key and keep backups.
5. Build and install the APK, then test touch controls, stability, pause/resume, and performance.

**Never commit keystores or signing passwords to GitHub.**