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


## Usage

First install the package to your world, either by adding the [VCC Repository](https://miniontoby.github.io/VRChatMultiCamMixer/) to your VCC/Alcom, or by importing the unitypackage from the [Github Releases](https://github.com/Miniontoby/VRChatMultiCamMixer/releases) page.  
After doing that, you must drop in a mixer from the `Runtime/Prefabs/Mixers` folder in the VRChatMultiCamMixer package folder.  
After that, you can drop in camera's from the `Runtime/Prefabs/Cameras` folder in the VRChatMultiCamMixer package folder. You likely want to edit the Culling Mask to hide UI related stuff, this is NOT done for you.  
After that, click on your mixer and drop in the camera's your placed into the Input 1 thru 8 (depending on which mixer you're using), so that the mixer knows which camera is which input.  
After that, you can go test in the editor if it works, and if it works, you can go and upload your world to test it in VRChat!  

For streaming the program feed, you'll likely want to get yourself a screenspace override shader. (As of writing this README, I am unsure if I should provide a shader for that. Lemme know in the issues tab!)  
After getting that, you make a new material with that shader and then set `Runtime/RenderTextures/ProgramCamera-RT.rendertexture` as the texture for that material.  
And then you put that material onto a sphere or cube or whatever you want, and then you can put your VRChat camera inside of that object, and then use the Spout streaming feature to import the feed into like OBS.  
