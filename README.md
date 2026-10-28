# Saltwater Boat Simulator

A Unity starter project for an Android-focused boating game: open-water exploration, missions, boat handling, credits, upgrades, and unlockable boat classes.

## Project status

**Prototype source code only — not a finished game or APK yet.** The scripts and Editor scene generator are a foundation. A successful Unity build and device test are still required. Current boat, island, and water visuals are placeholders, not cinematic-quality final assets.

## Included

- Boat movement and simple buoyancy approximation
- Smooth chase camera and mobile touch controls
- Credits, mission buoys, and exploration rewards
- Engine/handling upgrades and boat-unlock progression
- Editor menu command to generate a starter scene
- GitHub Actions workflow to attempt an Android APK build

## Android build setup

The build uses Unity 2022.3.62f1 and GameCI. It requires a valid Unity license and GitHub Actions secrets.

1. In GitHub, open **Settings → Secrets and variables → Actions** for this repository.
2. Add repository secrets named `UNITY_EMAIL` and `UNITY_PASSWORD` for the Unity account used by GameCI. Optionally add `UNITY_LICENSE` if you already have a valid Unity license file.
3. Never paste credentials into source files, issues, or chat. Only add them in GitHub's secret-value fields. Use a Unity account and license eligible for CI use.
4. Open **Actions → Build Saltwater Android APK → Run workflow**.
5. If the run fails, open the run and inspect the first failing step/log; a successful build should provide the `Saltwater-Boat-Simulator-APK` artifact.

The old `unity-request-activation-file` action is deprecated and fails, so it has been removed from the setup flow. See the current [GameCI activation guide](https://game.ci/docs/github/activation/).

## Important limitations

A build may still fail because of Unity license/account restrictions, package or editor compatibility, or project compilation issues. GitHub Actions does not provide a visual Unity Editor. The starter scene generator creates simple primitive placeholder geometry; realistic ocean shaders, detailed boat models, wakes/spray, audio, menus, persistent saves, and actual boat selection remain future work.

## Security

Never commit keystore files, passwords, signing keys, or other secrets to this repository.
