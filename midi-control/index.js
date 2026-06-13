/*@cc_on
@if (@_jscript)
	var objShell = new ActiveXObject('Shell.Application');
	var pathSelf = WScript.ScriptFullName;
	objShell.ShellExecute('cmd.exe', '/C node.exe ' + pathSelf + ' & pause');
	WScript.Quit();
@else @*/

const midi = require('@julusian/midi');
const fs = require('fs');
const path = require('path');
const os = require('os');
const readline = require('readline');

const CONTROL_CHANNEL = 15;
const CONTROL_NOTE = 126;

class MIDIMultiCamMixer {
	constructor(deviceName = 'loopMIDIPort', useEditorLogs = false) {
		this.deviceName = deviceName;
		this.useEditorLogs = useEditorLogs;
		this.output = new midi.Output();
		this.logStream = null;
		this.lastUpdate = 0;
		this.watchdogInterval = null;
		this.connected = false;

		this.CurrentProgram = 0;
		this.CurrentPreview = 0;
	}

	GetMidiDevices() {
		const count = this.output.getPortCount();
		const devices = [];
		for (let i = 0; i < count; i++) {
			devices.push({ index: i, name: this.output.getPortName(i) });
		}
		return devices;
	}

	MidiConnectDevice(deviceName, useEditorLogs = undefined) {
		if (useEditorLogs === true || useEditorLogs === false) this.useEditorLogs = useEditorLogs;
		if (this.connected) {
			this.output.closePort();
			this.connected = false;
		}

		const count = this.output.getPortCount();
		for (let i = 0; i < count; i++) {
			if (this.output.getPortName(i).includes(deviceName)) {
				this.output.openPort(i);
				this.connected = true;
				console.log(`[MIDIMultiCamMixer] Connected to: ${this.output.getPortName(i)}`);
				return true;
			}
		}

		console.error(`[MIDIMultiCamMixer] Device not found: ${deviceName}`);
		return false;
	}

	Construct() {
		MidiConnectDevice(this.deviceName);
		this._findVRCLog();
		this._Reset();

		// Watchdog loop — mirrors the original's per-frame watchdog
		this.watchdogInterval = setInterval(() => this._Tick(), 100);
	}

	Deconstruct() {
		if (this.watchdogInterval) clearInterval(this.watchdogInterval);
		if (this.logStream) this.logStream.close();
		if (this.connected) this.output.closePort();
	}

	SetCurrentProgram(value) {
		this.CurrentProgram = value & 0x3F;
		this._SendChannelValue(0, this.CurrentProgram);
	}

	SetCurrentPreview(value) {
		this.CurrentPreview = value & 0x3F;
		this._SendChannelValue(1, this.CurrentPreview);
	}

	Cut() {
		const oldProgram = this.CurrentProgram;
		const oldPreview = this.CurrentPreview;
		this.SetCurrentPreview(0);
		this.SetCurrentProgram(oldPreview);
		this.SetCurrentPreview(oldProgram);
	}

	_Tick() {
		if (this._isMidiReady()) {
			this._midiWatchdog();
			this.lastUpdate = Date.now();
		} else {
			const elapsed = (Date.now() - this.lastUpdate) / 1000;
			if (elapsed > 1) {
				this.lastUpdate = Date.now();
				this._Reset();
			}
		}
	}

	_Reset() {
		this._findVRCLog();
		this._midiKnock();
		this._midiWatchdog();
	}

	_SendChannelValue(channel, velocity) {
		if (!this.connected) return;
		velocity = Math.min(velocity, 50);
		this._SendMidiControl((velocity << 1) | (channel & 0x1));
	}

	_SendMidiControl(code) {
		if (!this.connected) return;
		this.output.sendMessage([0xB0 | (CONTROL_CHANNEL & 0xF), CONTROL_NOTE, code & 0x7F]);
	}

	_midiKnock() {
		this._SendMidiControl(102); // KnockStart
		this._SendMidiControl(119); // KnockMiddle
		this._SendMidiControl(108); // KnockFinish
	}

	_midiWatchdog() {
		this._SendMidiControl(127); // Watchdog
	}

	_isMidiReady() {
		if (!this.logStream || !this.connected) return false;

		try {
			const stat = fs.statSync(this.logStream.path);
			const newBytes = stat.size - this.logStream.bytesRead;
			if (newBytes <= 0) return false;

			const buf = Buffer.alloc(newBytes);
			const fd = fs.openSync(this.logStream.path, 'r');
			fs.readSync(fd, buf, 0, newBytes, this.logStream.bytesRead);
			fs.closeSync(fd);
			this.logStream.bytesRead += newBytes;

			return buf.includes('MIDIMIXERREADY');
		} catch {
			return false;
		}
	}

	_findVRCLog() {
		this.logStream = null;

		const vrcPath = path.join(os.homedir(), 'AppData', 'LocalLow', 'VRChat', 'VRChat');
		let logs = [];
		try {
			logs = fs.readdirSync(vrcPath)
				.filter(f => f.match(/^output_log_.*\.txt$/))
				.map(f => path.join(vrcPath, f))
				.sort();
		} catch {
			console.warn('[MIDIMultiCamMixer] Could not find VRC log directory.');
			return;
		}

		if (logs.length === 0) return;
		if (this.useEditorLogs) logs[logs.length - 1] = path.join(os.homedir(), 'AppData', 'Local', 'Unity', 'Editor', 'Editor.log');

		const latest = logs[logs.length - 1];
		const size = fs.statSync(latest).size;

		this.logStream = { path: latest, bytesRead: size > 0 ? size - 1 : 0 };
	}
}

// CLI usage
const mixer = new MIDIMultiCamMixer(); // set to true for editor logs

const devices = mixer.GetMidiDevices();
console.log('Available MIDI devices:');
devices.forEach(d => console.log(`  [${d.index}] ${d.name}`));

mixer.MidiConnectDevice(process.argv[2] || 'loopMIDIPort', process.argv[3] === 'true');
mixer._findVRCLog();
mixer._Reset();
mixer.watchdogInterval = setInterval(() => mixer._Tick(), 100);

const rl = readline.createInterface({ input: process.stdin });
console.log('Commands: program <0-50> | preview <0-50> | cut | quit');
rl.on('line', (line) => {
	const [cmd, val] = line.trim().split(' ');
	const n = parseInt(val);
	if (cmd === 'program') mixer.SetCurrentProgram(n);
	else if (cmd === 'preview') mixer.SetCurrentPreview(n);
	else if (cmd === 'cut') mixer.Cut();
	else if (cmd === 'quit') { mixer.Deconstruct(); process.exit(0); }
});


/*@end @*/
