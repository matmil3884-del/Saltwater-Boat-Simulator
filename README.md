# Saltwater Fishing Simulator

An Android-focused Unity fishing and boating game prototype. Drive a small boat around a coastal world, cast a line, wait for a bite, reel in fish, earn credits, and build a fishing log while exploring.

## Current prototype features

- Mobile boat steering, throttle and reverse controls
- Simple buoyancy and chase camera
- Tap-to-cast and reel-in fishing loop
- Five catchable saltwater species: spotted seatrout, redfish, sheepshead, flounder, and tarpon
- Variable fish weights, weighted rarity, catch log count, and credit rewards
- Coast exploration buoys and credit rewards
- Editor scene generator and GitHub Actions Android APK workflow

## Project status

**This is prototype source code, not a finished game or a verified APK.** The generated boat, ocean, islands, and bobber are simple placeholder geometry. Fish are currently represented by catch results in the fishing log rather than animated 3D fish models. A successful Unity build and real-device test are still required.

## How fishing works

1. Launch the scene and use the on-screen LEFT, RIGHT, THROTTLE, and REVERSE buttons to move the boat.
2. Tap **CAST LINE**.
3. Wait for the bite message, then tap **REEL IN!** before the countdown ends.
4. Each landed fish has a species, random weight, and credit reward. If the countdown expires, the fish escapes.
5. Explore the coast and collect mission buoys for extra credits.

## Android build setup

The build uses Unity 2022.3.62f1 and GameCI. It requires a valid Unity license and GitHub Actions configuration.

1. In GitHub, open **Settings → Secrets and variables → Actions** for this repository.
2. Configure only the Unity license credentials needed for your eligible Unity account and CI setup. Never paste credentials into source files, issues, or chat; only use GitHub's secret-value fields, and only if you trust the workflow and its dependencies.
3. Open **Actions → Build Saltwater Fishing Simulator APK → Run workflow**.
4. If the run fails, open it and inspect the first failing step. A successful build should provide the `Saltwater-Fishing-Simulator-APK` artifact containing `SaltwaterFishingSimulator.apk`.

The old `unity-request-activation-file` action is deprecated and removed. See the [GameCI activation guide](https://game.ci/docs/github/activation/).

## Next development steps

- Replace the placeholder hull with a detailed fishing boat and add an unlockable fleet
- Add visible fish models, bite animations, fishing rod/line, and splash effects
- Add fishing hotspots, tackle and bait selection, fish size records, missions, and a saved inventory
- Upgrade the ocean, sky, islands, weather, lighting, sound, wakes, and spray
- Test performance and touch UI on the target Android phone

## Security

Never commit keystore files, passwords, signing keys, or other secrets to this repository.
