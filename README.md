# 3D Top-Down Shooter Defense (Work in Progress)
This is a personal project currently under development. It is a 3D Top-Down Shooter with Tower Defense mechanics, where players must survive infinite waves of enemies by building defenses and upgrading their arsenal.

Project Started: Late January 2026
README Last Updated: February 5, 2026
---
## Game Concept
The gameplay is divided into two main phases managed by a custom Game State system::
**Preparation Phase**
- The player can build towers, traps, and obstacles.
- Weapons and upgrades can be acquired.
- The player decides when to start the next wave (or waits for an auto-start timer).

**Combat Phase**
- Enemies spawn in infinite waves with increasing difficulty.
- Enemy stats and compositions scale over time.
- The goal is to survive as many waves as possible.
---
## Current Progress
This project was created with the goal of practicing **gameplay architecture and system design**, rather than focusing only on visual polish.
Key systems implemented so far:
### Game Manager
- Central authority using a State-based approach.
- Controls transitions between Preparation and Combat phases.
- Decoupled from UI and gameplay systems via events.
### Wave System
- Infinite wave progression.
- Enemy composition and difficulty scale dynamically over time.
- Supports grouped enemy spawns per wave instead of single-unit spawns.
### Building System
- Grid-less placement system using raycasts on the ground.
- Preview-based placement with validation logic.
- Designed to support future expansion (blocking paths, traps, NavMesh obstacles).
### Combat & Weapons
- Multiple weapon types implemented.
- Data-driven weapon configuration using ScriptableObjects.
- Designed to support inventory switching and future grenade systems.
### Assets
- Third-party assets used for characters and environments.
- Custom animation controllers shared between player and enemies using Animator Override Controllers.
---
## Future Implementation
The project is still a few weeks away from completion. The next steps include:
- UI/UX implementation (shop, HUD, menu, wave info).
- Additional mechanics (grenades, traps, boss enemy).
- Visual polish (VFX, game feel, audio).
- Save system (JSON-based) for progression.
- Further balancing of difficulty curves and economy.
---
## Note:
This repository is public to showcase my development process and architectural decisions.
Once the project reaches a more complete state, this README will be expanded with:
- Detailed system breakdowns.
- Code structure explanations.
- Design decisions.
