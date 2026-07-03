# io.github.miniontoby.vrchatmulticammixer by Miniontoby

A drag & drop multi-camera management system for VRChat worlds, allowing to do in-game video switching between multiple camera's to make multi cam livestreams easier to manage.


## Available Prefabs

### Video Switchers

Models:
- [ ] Atem Mini Pro
  - Inputs: 4
- [x] Atem Mini Extreme
  - Inputs: 8
- [ ] Atem Constellation HD's (unsure if people need more than 8 cameras with Mini Extreme mixer. Like surely no-one would need 40 inputs, right?):
  - [ ] Atem 1 M/E Constellation HD
    - Inputs: 10
  - [ ] ATEM 2 M/E Constellation HD
    - Inputs: 20
  - [ ] ATEM 4 M/E Constellation HD
    - Inputs: 40

Features:
- [x] Input preview selection
  - Input button becomes green when in preview
- [x] Cutting from preview into program
  - Input button becomes red when in program
- [ ] Transition/auto from preview into program
- [x] Output preview selection:
  - [x] Program
  - [ ] Preview
  - [x] Specific cameras
  - [ ] Multi View
- [x] Remote control thru MIDI for using external controller instead of pressing buttons in-game
  - Uses ControlChange messages on channel 15 and control 126
  - There's code available of a NodeJS standalone app and there's a [Companion module](https://github.com/miniontoby/companion-module-vrchat-midi-multicammixer) available.


### Camera's

Models:
- [x] Static camera:
  - Just a static camera for static shots.
  - Cannot be operated
  - Example usecases: Drumming booth camera
- [x] Tripod camera:
  - A camera on a tripod
  - Can be operated to rotate around
  - Example usecases: talent framing shots or crowd overhead shots
- [ ] Dolly camera:
  - A camera on rails
  - Can be operated to rotate around and to move along the rails left to right and can be moved up and down a bit
  - Example usecases: more interesting talent framing shots or crowd overhead shots
- [ ] Crane camera:
  - A camera on a crane
  - Can be operated to rotate around and to move with the crane
  - Example usecases: really interesting crowd shots
- [x] Handheld camera:
  - A camera that you can hold
  - Can be operated to move and rotate around freely
  - Example usecases: more dynamic shots, moving shots

Features:
- [x] Preview screen when operating camera
- [x] Tally light on camera
  - Red on program
  - Green on preview
  - White on 'standby'
  - Gray on not connected
- [ ] Zoom control
- [ ] Focus control


## Usage/installation in Unity

First install the package to your world, either by adding the [VCC Repository](https://miniontoby.github.io/VRChatMultiCamMixer/) to your VCC/Alcom, or by importing the unitypackage from the [Github Releases](https://github.com/Miniontoby/VRChatMultiCamMixer/releases) page.  
After doing that, you must drop in a mixer from the `Runtime/Prefabs/Mixers` folder in the VRChatMultiCamMixer package folder. You will likely want to edit the Culling Mask of the Program Camera to hide the UI layers!   
After that, you can drop in camera's from the `Runtime/Prefabs/Cameras` folder in the VRChatMultiCamMixer package folder. You likely want to edit the Culling Mask of each of the Preview Camera's as well to hide UI related stuff, this is NOT done for you.  
After that, click on your mixer and then drag and drop the camera's you placed in your world, into the Input 1 thru 8 slots of the mixer properties (depending on which mixer you're using), this way the mixer knows which camera should be linked to which button.  
After that, you can go test in the editor if it works, and if it works, you can go and upload your world to test it in VRChat!  

For streaming the program feed, you'll likely want to get yourself a screenspace override shader. (As of writing this README, I am unsure if I should provide a shader for that. Lemme know in the issues tab!)  
After getting that, you make a new material with that shader and then set `Runtime/RenderTextures/ProgramCamera-RT.rendertexture` as the texture for that material.  
And then you put that material onto a sphere or cube or whatever you want, and then you can put your VRChat camera inside of that object, and then use the Spout streaming feature to import the feed into like OBS.  


### Access control

You will need to install any access control package, like AccessTXL or something similar. As long as it supports turning on/off gameObjects when a user is on a list, you should be good to use it.

To make sure not everyone can just change the selected camera on the mixer itself, you should place the mixer into a place that is blocked by a collider.  
With a group toggle (AccessTXL), you then disable that collider when the user is on the access list.  
Do not disable the whole mixer, cause that can cause issues.

To make sure people without permissions cannot change the program/preview via MIDI regardless of being able to get to the mixer, you should expand the Mixer in the Hierachy, and then disable the MIDI listener gameObject by default.  
And then do a group toggle (AccessTXL) to enable the MIDI listener gameObject when the user is on the access list.  
As long as the MIDI listener gameObject is disabled/not active, it *should* not be able to receive and process MIDI events.

Also for the camera's themselves, if you disable the pickup script specifically (not the whole camera object, but just the pickup), and then again with a group toggle enable again when user is on access list.  


## Usage in world

It works like a real life switcher. Except the camera inputs are in the game and you can switch inside the game, enabling you to only use one spout output inside a camera cube/sphere.

You select a button with a number, which links to a camera in-game. When you then click the Cut button, it will make that camera go live.  
When you then select another button with a number, then it will put that camera into preview, and then when you hit Cut again, it will make that camera go live, and the camera that was live, will now be in preview.  

There's also smaller buttons under the category "Outputs", which allow you to preview any camera at any given time. The button PGM will put you back to the Live feed.


### MIDI protocol

There's also a MIDI control protocol to control the mixer from outside of VRChat. Inside of the [../../midi-control](../../midi-control) folder there's a very basic setup with a command line based control system.  
But for more practical usages, there's an external repository that adds support for controlling via a Bitfocus Companion module (find it in the same midi-control folder).
Companion is a free and open source software which can be used to do work with streamdecks and similar devices.

The README.md in the midi-control folder will explain how to set up MIDI control and how to get and set up the companion module in more detail.

