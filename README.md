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
    - Fix jerky camera movement.
    - Simplify movement, especially removing unused variables and renaming used variables to make more sense.
    - Change weapon implementation to allow for easier weapon creation.
    - Fix bug that makes second weapon's ammo count appear on scene load.
    - ADS will affect spread.
- Code refactoring, comments, and cleanup.
- Add player spawning and death.
- Remove projectile spawning and use an object pool instead.
- Add in obstacles and graybox testbed level.
- Add in player models and animations.


### Change History (Organized By Date)
#### 7/27/2020
- Implemented object pools for player projectiles.
- Fixed a bug where continuing to sprint after stamina reaches zero would result in the player continuing to sprint with no stamina regeneration. Reaching zero stamina now invokes a wait-time before you can begin sprinting again.

#### 7/08/2020
- Player no longer transitions to onGround state if they hit the side of a landable object.
- Player now smoothly descends slopes.
- Projectiles now spawn at an empty game object parented to the player object.
- Projectiles despawn when leaving a soft boundary.
- Aim down sight's camera offset can now have different x and y values.
- Added ammo and reloading mechanics and UI.
- Moved shooting functionality into Weapon.cs so that it is no longer split between Weapon.cs and PlayerShoot.cs.
- Added "drop" mechanic to flight. Double-tap LEFT-CTRL to transition to the falling state.
- Pressing SHIFT now only depletes stamina if the player is moving.

#### 6/10/2020
- Sprint implemented.
- Stamina and flight stamina and respective UI implemented.
- Falling out of the sky on flight stamina depletion implemented.

#### 5/26/2020
- Initial commit.
- Movement and shooting mechanics implemented.