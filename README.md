# Zombie Arena
###### Unity first-person-shooter game where the player must fend off hoards of zombies using a variety of realistic weapons.
###### Based on an assignment from Ohio State course CSE 3541: Computer Game and Animation Techniques.
##### Author: [Austin Hendricks](https://github.com/austin-hendricks)
##### Date Completed: April 22, 2023

<br>

[Check it out on YouTube!](https://youtu.be/roXTK8zMuA8)
<br>

---

## Table of Contents

- [Requirements and Import Instructions](#Req)

- [Introduction and Controls](#Intro)

- [Gameplay and Functionality](#Func)

- [Techniques and Authorship](#Auth)

- [Material from Outside Sources](#Ref)

---

## Requirements and Import Instructions <a name="Req"></a>

#### Requirements
- Unity
- New Input System (please install from Unity Registry)
- Cinemachine (please install from Unity Registry)

#### Import Instructions

1. Download `ZombieArena.unitypackage` from this repository
2. Install requirements from Unity Registry
3. Allow restart of editor
4. Navigate to Edit → Project Settings → Player → Configuration and set the field
“Active Input Handling” to “Input System Package (New)” and click Apply to
restart the editor one more time
5. Import `ZombieArena.unitypackage` via Assets → Import Package\
6. Once imported, navigate to Assets → Scenes and open scene titled
“ZombieArena”
7. On the popup, click to import TMP essentials
8. Navigate to Game view and make the aspect 16:9
9. TMP essentials will not work properly until the game is played at least once. This
is due to the nature of the TMP fonts. Please click Play to start the game, and
immediately click Play again or hit the Escape key to stop the game
10. Now, the game is ready to play. Enjoy!


---

## Introduction and Controls <a name="Intro"></a>

#### Brief Description of Implementation
Zombie Arena is a first-person-shooter game designed in Unity where
the player must fend off hoards of zombies using a variety of realistic weapons. 
In this game, the player can move and look around, sprint, jump, operate
weapons, switch between weapons, and collect pickups.

#### Controls
- Move: WASD
- Look: Mouse
- Sprint: Shift
- Jump: Space
- Aim: Right click
- Shoot: Left click
- Reload: R
- Cycle Weapons: Mouse Scroll Wheel
- Switch to Rifle: 1
- Switch to Pistol: 2
- Switch to Rocket Launcher: 3
- Exit Game: Escape

---

## Gameplay and Functionality <a name="Func"></a>

#### Overview of Gameplay
The object of the game is to survive as many waves as possible in the arena.
The arena itself consists of a 100m x 100m pit which the player spawns inside of, and a
spawn platform 12m above the surrounding walls of the pit, which is where the zombies
spawn. Inside the pit are various obstacles for the player and the zombies to maneuver
around. The gameplay of Zombie Arena is separated into waves, with each wave
spawning more zombies than the previous. A wave is complete when all zombies from
that wave are eliminated, at which point a sound is played and a countdown until the
next wave begins. The player loses a life each time they are touched by a zombie, and
the game ends when the player is out of lives. When the game is over, a Game Over
pop-up appears displaying the total waves cleared and total zombies killed, with options
to either quit the game or play again.

#### Zombies
The zombies are operated by a finite state machine, with states for wandering
around, chasing the player, and avoiding obstacles. I added a health attribute to their
script as well as methods for taking damage and dying so that the player can kill the
zombies. I also increased the zombies’ field of view to 360 degrees and the view
distance to 50 meters, so that all zombies except those far from the player will chase.

#### Player / Camera
The player object contains a custom PlayerController script that interfaces with
Unity’s CharacterController, as well as a custom WeaponController script, in order to
move the character around and operate weapons. The camera uses Cinemachine for a
first-person camera that looks around based on mouse input delta. The player starts the
game with 3 lives, and loses a life every time a zombie touches them. The game ends
when the player is out of lives.

#### Weapons
The player has three weapons they can use: a fully-automatic rifle, a
semi-automatic handgun, and a rocket launcher. Each of these weapons are operated
via the WeaponController attached to the player object. With each weapon, the player
may shoot, aim, and reload. The handgun fires a 9mm round and ejects a 9mm casing,
just like many real-life handguns of this style. Likewise, the rifle fires a 5.56mm round
and ejects a 5.56mm casing, just like many real-life rifles of this style. Upon impact,
these bullets leave bullet holes on anything they hit. The casings and bullet holes
remain in the scene for a while and then automatically self-destruct after a certain
amount of time. The rocket launcher fires an RPG-style rocket projectile which emits
sound through the air and explodes upon impact with any surface or zombie, killing any
zombies within 8m of the explosion. Each weapon has simple manually-implemented
animations for recoil, reload, and aim. Additionally, each weapon has sounds for
shooting, reloading, and dry firing. An ammo display at the bottom right of the screen is
constantly updated.

#### Pickups
There are two kinds of pickups that zombies randomly drop during gameplay: life
pickups, and ammo pickups. Both types of pickups are animated with the same floating
animation and collect animation scripts that I animated my coins with in Lab 3. Life
pickups are floating 3D hearts that, upon collection, will give the player another life and
update the health HUD at the top right of the screen. Life pickups are not collected if the
player already has the maximum of 3 lives. Ammo pickups are floating 3D
arrangements of bullets (5.56mm and 9mm) that, upon collection, will max out the
ammo of the player’s currently equipped weapon and update the ammo HUD at the
bottom right of the screen. Ammo pickups are not collected if the player’s currently
equipped weapon already has maximum ammo. Whenever a zombie dies, there is a 5%
chance it will drop an ammo pickup, and a 0.5% chance it will drop a life pickup.

#### Optimizations During Development
I created my environment using hundreds of elements so that the Gridbox
Prototype materials I downloaded would accurately correspond to the measurements it
says on the material (i.e. 1 meter on the arena floor is equal to 1 meter in world space).
In order to optimize my game, I created a custom MeshCombiner script and
downloaded a Mesh Saver script so that I could combine all of the elements of my
environment into just a few distinct meshes and save those meshes to my assets. This
move brought my frame rate from around 70 fps to over 500 fps on my machine.

---

## Techniques Used and Authorship <a name="Auth"></a>

#### Techniques
I used the following techniques in my project, and personally authored my
implementation of all of them:
- Finite State Machine (zombie behavior)
- Object Pooling (zombie spawning)
- Raycast (calculating where to shoot bullet towards)
- OverlapSphere (zombie vision, rocket explosion radius)
- Events (custom EventManager and Events written by me)

#### Authorship
All of the code in my project is personally authored by me except for the
following: MeshSaverEditor.cs and CollectableAnimation.cs. All parts of my project not
authored by me are detailed below in the Materials From Outside Sources section below. 
Everything not listed in that section is authored solely by me.

## Material from Outside Sources <a name="Ref"></a>

#### Code
- MeshSaverEditor.cs --> [GitHub](https://github.com/pharan/Unity-MeshSaver/blob/master/MeshSaver/Editor/MeshSaverEditor.cs)
- CollectableAnimation.cs -–> [Simple Gems Ultimate Pack](https://assetstore.unity.com/packages/3d/props/simple-gems-ultimate-animated-customizable-pack-73764)

#### Meshes and Materials
- Floor and Wall materials –-> [Gridbox Prototype Materials](https://assetstore.unity.com/packages/2d/textures-materials/gridbox-prototype-materials-129127)
- Pistol and Rifle meshes, materials, and sounds –-> [Guns Pack: Low Poly Guns Collection](https://assetstore.unity.com/packages/3d/props/guns/guns-pack-low-poly-guns-collection-192553)
- Rocket Launcher and Rocket Projectile meshes and materials, sounds, and
visual effects –-> [Stylized Rocket Launcher Complete Kit with Visual Effects and
Sound](https://assetstore.unity.com/packages/3d/props/guns/stylized-rocket-launcher-complete-kit-with-visual-effects-and-so-178718)
- Muzzle Flash -–> [Particle Pack](https://assetstore.unity.com/packages/vfx/particles/particle-pack-127325)
- Bullets (9mm and 5.56mm) –-> [Ammunition Pack (Demo)](https://assetstore.unity.com/packages/3d/ammunition-pack-demo-82208)
- Bullet Casings (9mm and 5.56mm) –-> [VFX Bullet Casing](https://assetstore.unity.com/packages/3d/characters/vfx-bullet-casing-120111)
- 3D Low-Poly Heart (Life pickup) –-> [Simple Gems Ultimate Pack](https://assetstore.unity.com/packages/3d/props/simple-gems-ultimate-animated-customizable-pack-73764)
- All other sound effects –-> [Pixabay](https://pixabay.com/sound-effects/)
