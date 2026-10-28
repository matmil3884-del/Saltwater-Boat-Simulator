# Saltwater Fishing Simulator

An Android-focused Unity fishing and boating game prototype. Drive a starter skiff around a coastal world, cast a line, land saltwater fish, earn credits, upgrade fishing gear, and unlock different boats as you progress.

## Progression features in the prototype

- Starter skiff available from the beginning
- Unlockable speedboat and dedicated fishing boat
- Boat selection changes hull appearance, handling, acceleration, top speed, and mass
- Engine and handling upgrades purchased with in-game credits
- Five catchable saltwater species with variable weights and weighted rarity
- Fishing pole upgrades reduce wait times, increase the bite window, and improve credit rewards
- Mobile touch controls, simple buoyancy, chase camera, mission buoys, and exploration rewards
- Editor scene generator and GitHub Actions Android APK workflow

## How to play

1. Use LEFT, RIGHT, THROTTLE, and REVERSE to drive.
2. Tap CAST LINE, wait for the bite, then tap REEL IN before the timer expires.
3. Land fish and complete buoy missions to earn credits.
4. Use the **BOAT SHOP & UPGRADES** panel to improve your engine and handling, unlock boats, and select your active boat.
5. Use the fishing panel's **UPGRADE POLE** button to improve bite timing and catch rewards.

## Project status

**This is prototype source code, not a finished game or a verified APK.** Boats, ocean, islands, and bobber use simple placeholder geometry. Fish catches are currently represented as fishing results rather than visible animated fish models. Boat switching changes stats and the placeholder hull appearance; detailed unique 3D boat models and persistent progression still need to be added. A successful Unity build and real-device test are still required.

## Android build setup

The build uses Unity 2022.3.62f1 and GameCI. It requires a valid Unity license and GitHub Actions configuration.

1. In GitHub, open **Settings → Secrets and variables → Actions** for this repository.
2. Configure only the Unity license credentials needed for your eligible Unity account and CI setup. Never paste credentials into source files, issues, or chat; only use GitHub's secret-value fields, and only if you trust the workflow and its dependencies.
3. Open **Actions → Build Saltwater Fishing Simulator APK → Run workflow**.
4. If the run fails, open it and inspect the first failing step. A successful build should provide the `Saltwater-Fishing-Simulator-APK` artifact containing `SaltwaterFishingSimulator.apk`.

The old `unity-request-activation-file` action is deprecated and removed. See the [GameCI activation guide](https://game.ci/docs/github/activation/).

## Next upgrades

- Unique detailed 3D models for each boat, plus motors, consoles, seats, rod holders, and storage
- Visible fish models, rod and line animations, bite effects, and landing minigames
- Rod, reel, line strength, bait, and lure inventories
- Fishing hotspots, tides, weather, tournaments, licenses, and bigger trophy fish
- Save-game progression and performance tuning for Android

## Security

Never commit keystore files, passwords, signing keys, or other secrets to this repository.
