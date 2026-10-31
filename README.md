# Boating & Fishing Simulator

An Android-focused Unity boating and fishing game prototype. Explore coastal waters, lakes, and rivers; operate and upgrade boats; cast lines; land fish; earn credits; and build your fishing career.

## Current prototype features

- Starter skiff, unlockable speedboat, and dedicated fishing boat
- Boat selection changes hull appearance, handling, acceleration, top speed, and mass
- Engine and handling upgrades purchased with in-game credits
- Five catchable saltwater species with variable weights and weighted rarity
- Fishing pole upgrades reduce wait times, increase the bite window, and improve credit rewards
- Mobile touch controls, simple buoyancy, chase camera, mission buoys, and exploration rewards
- Visible bobber, fishing line, and animated underwater fish silhouettes
- Editor scene generator and GitHub Actions Android APK workflow

## Expanded game direction

The project is being expanded into a broader boating and fishing simulator while keeping the existing Unity project and progress. Planned gameplay includes:

- Freshwater lakes and rivers alongside saltwater coastal and offshore areas
- More boats: bass boats, pontoons, center consoles, offshore boats, and personal watercraft
- More fish species with habitat-specific availability, size ranges, rarity, and trophy catches
- Fishing equipment: rods, reels, line strength, hooks, bait, and lures
- Casting and retrieval choices, fish-fighting mechanics, and visible catch/landing sequences
- Weather, wind, waves, time of day, tides, and changing fishing conditions
- Boat ramps, docks, marinas, tournaments, licenses, and fishing challenges
- Persistent career progression, fish records, boat ownership, and equipment inventory

These are expansion goals, not claims that every feature is already implemented.

## How to play in the current prototype

1. Use LEFT, RIGHT, THROTTLE, and REVERSE to drive.
2. Tap CAST LINE, wait for the bite, then tap REEL IN before the timer expires.
3. Land fish and complete buoy missions to earn credits.
4. Use the BOAT SHOP & UPGRADES panel to improve your engine and handling, unlock boats, and select your active boat.
5. Use the fishing panel's UPGRADE POLE button to improve bite timing and catch rewards.

## Project status

**This is prototype source code, not a finished game or a verified APK.** Boats, ocean, islands, and bobber use simple placeholder geometry. Fish catches are represented as fishing results with basic underwater silhouettes rather than detailed animated fish models. Detailed environments, unique boat models, expanded fishing equipment, and persistent progression still need to be built. A successful Unity build and real-device test are still required.

## Android build setup

The build uses Unity 2022.3.62f1 and GameCI. It requires a valid Unity license and GitHub Actions configuration.

1. In GitHub, open Settings -> Secrets and variables -> Actions for this repository.
2. Configure only the Unity license credentials needed for your eligible Unity account and CI setup. Never paste credentials into source files, issues, or chat; only use GitHub's secret-value fields, and only if you trust the workflow and its dependencies.
3. Open Actions -> Build Saltwater Fishing Simulator APK -> Run workflow.
4. If the run fails, open it and inspect the first failing step. A successful build should provide the Saltwater-Fishing-Simulator-APK artifact containing SaltwaterFishingSimulator.apk.

The old unity-request-activation-file action is deprecated and removed. See the GameCI activation guide: https://game.ci/docs/github/activation/.

## Security

Never commit keystore files, passwords, signing keys, or other secrets to this repository.
