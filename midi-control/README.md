# MIDI control

This folder contains a standalone midi client that can be used to control which camera goes program, preview etc.

There is also an Bitfocus Companion module available in a different repo:
https://github.com/Miniontoby/companion-module-vrchat-midi-multicammixer


## Requirements

It does require [loopMIDI](https://www.tobias-erichsen.de/software/loopmidi.html) with **feedback detection turned off** to be installed.
After installation make sure to restart your computer.

After that, in the loopMIDI settings, add a new port with the name `loopMIDIPort` (just remove the space from the name, unless you want troubles)

Then go to Steam, go to your library, go to VRChat, then Manage and then Properties.
Then there should be an input field for startup/launch options. Add `--midi=loopMIDIPort` into that text field!

Then (re)start VRChat.


## Usage

First install the packages using `npm install` (requires NodeJS to be installed).
Then just run `node standalone_client.js` to run the client.

You can pass the loopMIDI port as the first argument.  
When running in editor, you should pass `true` after the loopMIDI port to tell the script to use the editor logs.
