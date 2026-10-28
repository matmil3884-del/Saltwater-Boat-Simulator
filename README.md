# Saltwater Boat Simulator

A Unity starter project for an Android-focused, cinematic 3D boating game: open-water exploration, missions, boat handling, credits, upgrades, and unlockable boat classes.

## Project status

**Prototype source code only — not a finished game or APK yet.** The scripts are a foundation. A Unity Editor environment is still needed to create the scene, add boat/ocean assets, configure physics and touch UI, test on a device, and build an APK.

## Included scripts

- `Assets/Scripts/BoatController.cs` — basic throttle, reverse, steering, and speed limits
- `Assets/Scripts/BoatBuoyancy.cs` — simple multi-point buoyancy approximation
- `Assets/Scripts/BoatCameraFollow.cs` — smooth chase camera
- `Assets/Scripts/MobileBoatControls.cs` and `HoldButton.cs` — touch hold controls
- `Assets/Scripts/BoatGameManager.cs` — credits and mission messages
- `Assets/Scripts/MissionBuoy.cs` and `ExplorationZone.cs` — exploration rewards
- `Assets/Scripts/BoatUpgradeSystem.cs` — engine/handling upgrades and boat unlocks
- `Assets/Scripts/CinematicOceanMotion.cs` — subtle visual motion (not realistic wave simulation)

## Start here

1. Use Unity 2022.3 LTS or a compatible Unity 6 version with Android Build Support, Android SDK & NDK Tools, and OpenJDK.
2. Create a **3D (URP)** project.
3. Copy this repository's `Assets` folder into the Unity project.
4. Follow [Android_Setup_Guide.md](Android_Setup_Guide.md) to assemble a test scene and wire touch controls.
5. Test the prototype before attempting an APK build.

## Design target

Landscape Android; open-world coastal exploration; speedboats and fishing boats unlocked through progression; mobile-friendly cinematic visuals. Realistic water, boat models, islands, sounds, menus, and persistent saves must be added as the project develops.

## Security

Never commit keystore files, passwords, signing keys, or other secrets to this repository.