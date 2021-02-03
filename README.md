# WyvernDeathmatch
## Third-Person Multiplayer Shooter

### Introduction
This game is a third-person multiplayer shooter with flight mechanics. Players have two weapons at their disposal.

### Controls
- Move - WASD
- Jump/Ascend - SPACE
- Descend - LEFT-CTRL
- Drop - 2x LEFT-CTRL
- Rotate Camera - Move Mouse
- Zoomed Aim - RIGHT MOUSE BUTTON
- Shoot - LEFT MOUSE BUTTON
- Switch Weapons - SCROLL UP/DOWN

### Future Plans
- Revise movement, camera, and shooting systems.
    - Stop camera form clipping through objects.
    - Make ADS dependent on weapon rather than player.
    - Change weapon implementation to allow for easier weapon creation.
    - ADS will affect spread.
- Code refactoring, comments, and cleanup.
- Add player spawning and death.
- Add in obstacles and graybox testbed level.
- Add in player models and animations.


### Change History (Organized By Date)
#### 02/03/2021
- Overhauled weapon system to use individual prefabs and ScriptableObjects. This makes it easier to add and modify weapons.
- Added player placeholder model.
- Changed bullet prefab to use a more bullet-shaped model and a trail renderer for a tracer effect.
- Removed flight stamina deplection. Sprinting and stamina are a mess right now and I'm not sure if I want to keep them as game mechanics. Leaning towards removing both types of stamina.
    - Stamina just slows the game down, and this game is supposed to be fast paced.
- Locked frame rate to 60 fps supposedly (on my 144hz monitor it runs at 144 fps ¯\_(ツ)_/¯). Either way, the framerate and stuttering issues should be resolved.

#### 10/17/2020
- Started new level greybox.
- Redesigned movement and camera system.
- Cleaned up commented-out code and added comments to movement and camera system files.
- Fixed jerky camera movement.
- Fixed bug that caused second weapon's ammo count to appear on scene load.

#### 08/12/2020
- Built draft 1 of level greybox.

#### 07/27/2020
- Implemented object pools for player projectiles.
- Fixed a bug where continuing to sprint after stamina reaches zero would result in the player continuing to sprint with no stamina regeneration. Reaching zero stamina now invokes a wait-time before you can begin sprinting again.

#### 07/08/2020
- Player no longer transitions to onGround state if they hit the side of a landable object.
- Player now smoothly descends slopes.
- Projectiles now spawn at an empty game object parented to the player object.
- Projectiles despawn when leaving a soft boundary.
- Aim down sight's camera offset can now have different x and y values.
- Added ammo and reloading mechanics and UI.
- Moved shooting functionality into Weapon.cs so that it is no longer split between Weapon.cs and PlayerShoot.cs.
- Added "drop" mechanic to flight. Double-tap LEFT-CTRL to transition to the falling state.
- Pressing SHIFT now only depletes stamina if the player is moving.

#### 06/10/2020
- Sprint implemented.
- Stamina and flight stamina and respective UI implemented.
- Falling out of the sky on flight stamina depletion implemented.

#### 05/26/2020
- Initial commit.
- Movement and shooting mechanics implemented.