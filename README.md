# UdonTether
A swinging grapple system for VRChat. Requires UdonSharp.

## Preview Video
[![Alt text](https://img.youtube.com/vi/h9a5vj0PBxQ/0.jpg)](https://www.youtube.com/watch?v=h9a5vj0PBxQ)

## Download
Get it in the [releases](https://github.com/squiddingme/UdonTether/releases).

## Basic Setup
1. Import VRChat SDK3 to project (don't forget to let the SDK set up your layers and collision matrix)
2. Import [UdonSharp](https://github.com/Merlin-san/UdonSharp/releases) to project
3. Import Tether Grapple package to project
4. Compile all UdonSharp programs by using the button in the inspector of any Udon C# Program Asset
5.	Drag TetherVRPickupPrefab into your scene to add a permanently accessible grapple device for VR users. It is accessible by grabbing behind the player's head.
6.	Drag TetherDesktopPrefab into your scene to add a permanently accessible grapple device for desktop users. It is accessible in desktop mode by pressing R.
7.	If you don't plan on modifying the scripts at all, this and some configuration of the TetherProperties scripts on the root objects of these two prefabs is enough to have a functional grappling world.

More instructions can be found in readme.pdf in release packages.

## License
This work is licensed under the terms of the MIT license.