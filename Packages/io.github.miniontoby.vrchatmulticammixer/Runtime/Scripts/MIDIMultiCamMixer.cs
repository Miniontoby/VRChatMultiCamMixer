using UdonSharp;
using UnityEngine;
using System;
using VRC.SDKBase;


[DefaultExecutionOrder(1)]
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class MIDIMultiCamMixer : UdonSharpBehaviour
{
	public MixerLogicScript mixerLogic;

	private float lastUpdate = 0;

	bool state = false;
	bool previousState = false;
	int knockState = 0;

	Component[] eventObjects = new Component[0];
	string[] eventCallbacks = new string[0];

	void Start()
	{
		state = false;
	}

	public bool GetState()
	{
		return state;
	}

	public void _RegisterEvent(Component obj, string callback)
	{
		eventObjects = Add(eventObjects, obj);
		eventCallbacks = Add(eventCallbacks, callback);
	}

	void ProcessEvents()
	{
		for (int i = 0; i < eventObjects.Length; i++)
		{
			if (!eventObjects[i]) continue;
			UdonSharpBehaviour behaviour = (UdonSharpBehaviour)eventObjects[i];
			if (Utilities.IsValid(behaviour))
				behaviour.SendCustomEvent(eventCallbacks[i]);
		}
	}

	public override void MidiControlChange(int channel, int number, int value)
	{
		if (mixerLogic == null) return;
		if (channel != 15 || number != 126) return;

		if (!state)
		{
			if (knockState == 0 && value == 102)
				knockState = 1;
			else if (knockState == 1 && value == 119)
				knockState = 2;
			else if (knockState == 2 && value == 108)
				MidiStart();
			else
			{
				knockState = 0;
				return;
			}
		}
		else if (value >= 0 && value <= 101) // 0 to 101 -> 101 >> 1 = 50 -> 0 to 50 = 50 inputs max
		{

			int address = value & 0x1;
			int velocity = value >> 1;

			if (address == 0)
			{
				mixerLogic.CurrentProgram = (MixerStateEnum)(byte)(velocity & 0x3F);
				Networking.SetOwner(Networking.LocalPlayer, mixerLogic.gameObject);
				mixerLogic.RequestSerialization();
				Debug.Log("[MIDIMultiCamMixer] CurrentProgram: " + mixerLogic.CurrentProgram.ToString());
			}
			else if (address == 1)
			{
				mixerLogic.CurrentPreview = (MixerStateEnum)(byte)(velocity & 0x3F);
				Networking.SetOwner(Networking.LocalPlayer, mixerLogic.gameObject);
				mixerLogic.RequestSerialization();
				Debug.Log("[MIDIMultiCamMixer] CurrentPreview: " + mixerLogic.CurrentPreview.ToString());
			}
		}

		if (value == 127)
		{
			lastUpdate = Time.fixedTime;
			Debug.Log("MIXERREADY");
		}
	}

	void Update()
	{
		if (mixerLogic == null) return;

		if (state && lastUpdate > Time.fixedTime - 5)
			return;
		else
			MidiEnd();

		if (previousState != state)
		{
			previousState = state;
			ProcessEvents();
		}
	}

	void MidiStart()
	{
		knockState = 3;
		state = true;
		Debug.Log("[MIDIMultiCamMixer] Unlocked and ready.");
	}

	void MidiEnd()
	{
		state = false;
		knockState = 0;
	}

	private string[] Add(string[] inputArray, string toAdd)
	{
		string[] output = new string[inputArray.Length + 1];
		Array.Copy(inputArray, output, inputArray.Length);
		output[inputArray.Length] = toAdd;
		return output;
	}

	private Component[] Add(Component[] inputArray, Component toAdd)
	{
		Component[] output = new Component[inputArray.Length + 1];
		Array.Copy(inputArray, output, inputArray.Length);
		output[inputArray.Length] = toAdd;
		return output;
	}
}