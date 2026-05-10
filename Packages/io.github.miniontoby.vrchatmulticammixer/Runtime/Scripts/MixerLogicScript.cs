using UdonSharp;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MixerLogicScript : UdonSharpBehaviour
{
    private Color ColorDimGray { get; } = new Color(0.1568f, 0.1568f, 0.1568f, 1f); // 40, 40, 40


    [Header("Inputs")]
	[SerializeField] private CameraComponent input1 = null;
	[SerializeField] private CameraComponent input2 = null;
	[SerializeField] private CameraComponent input3 = null;
	[SerializeField] private CameraComponent input4 = null;
	[SerializeField] private CameraComponent input5 = null;
	[SerializeField] private CameraComponent input6 = null;
	[SerializeField] private CameraComponent input7 = null;
	[SerializeField] private CameraComponent input8 = null;

	[Header("Internal")]
    [SerializeField] private RenderTexture internalRenderTexture;
    [SerializeField] private RenderTexture internalFallbackRenderTexture;
    [SerializeField] private Camera internalProgramCamera;

    [SerializeField] private Button internalInput1Button;
	[SerializeField] private Button internalInput2Button;
	[SerializeField] private Button internalInput3Button;
	[SerializeField] private Button internalInput4Button;
	[SerializeField] private Button internalInput5Button;
	[SerializeField] private Button internalInput6Button;
	[SerializeField] private Button internalInput7Button;
	[SerializeField] private Button internalInput8Button;
	[SerializeField] private Button internalInputMP1Button;
	[SerializeField] private Button internalInputMP2Button;
	[SerializeField] private Button internalInputSRCButton; // Not really used yet, but sure
	[SerializeField] private Button internalInputBlackButton;

	[SerializeField] private Button internalTransitionCutButton;
	[SerializeField] private Button internalTransitionAutoButton;
	[SerializeField] private Button internalTransitionFTBButton;

	[SerializeField] private OutlinedButtonComponent internalOutput1Button;
	[SerializeField] private OutlinedButtonComponent internalOutput2Button;
	[SerializeField] private OutlinedButtonComponent internalOutput3Button;
	[SerializeField] private OutlinedButtonComponent internalOutput4Button;
	[SerializeField] private OutlinedButtonComponent internalOutput5Button;
	[SerializeField] private OutlinedButtonComponent internalOutput6Button;
	[SerializeField] private OutlinedButtonComponent internalOutput7Button;
	[SerializeField] private OutlinedButtonComponent internalOutput8Button;
	[SerializeField] private OutlinedButtonComponent internalOutputCleanButton;
	[SerializeField] private OutlinedButtonComponent internalOutputPVRButton;
	[SerializeField] private OutlinedButtonComponent internalOutputMVButton;
	[SerializeField] private OutlinedButtonComponent internalOutputPGMButton;

	[UdonSynced]
	private byte _currentProgram = (byte)MixerStateEnum.InputBlack;

	public MixerStateEnum CurrentProgram
	{
		set
		{
			Button currentProgramInput = GetButton(_currentProgram);
			if (currentProgramInput != null)
			{
                CinemachineVirtualCamera currentProgramCamera = GetVirtualCamera(_currentProgram);
				if (currentProgramCamera != null)
				{
					currentProgramCamera.gameObject.SetActive(false);
					currentProgramInput.image.color = _currentPreview != _currentProgram ? Color.white : Color.green;
				}
				else if (_currentProgram <= (byte)MixerStateEnum.Input8)
				{
					currentProgramInput.image.color = Color.gray;
				}
			}

			Button newProgramInput = GetButton(value);
			if (newProgramInput != null)
			{
                CinemachineVirtualCamera newProgramCamera = GetVirtualCamera(value);
				if (newProgramCamera != null)
				{
					_currentProgram = (byte)value;
					newProgramCamera.gameObject.SetActive(true);
					newProgramInput.image.color = Color.red;
				}
			}
		}
		get => (MixerStateEnum)_currentProgram;
	}

	[UdonSynced]
	private byte _currentPreview = (byte)MixerStateEnum.InputBlack;

	public MixerStateEnum CurrentPreview
	{
		set
		{
			Button currentPreviewInput = GetButton(_currentPreview);
			if (currentPreviewInput != null)
			{
				if (_currentPreview != _currentProgram)
					currentPreviewInput.image.color = GetVirtualCamera(_currentPreview) != null ? Color.white : Color.gray;
				// else it should stay red
			}

			Button newPreviewInput = GetButton(value);
			if (newPreviewInput != null)
			{
				if (GetVirtualCamera(value) != null)
				{
					_currentPreview = (byte)value;
					if (_currentPreview != _currentProgram)
						newPreviewInput.image.color = Color.green;
					// else it should stay red

					internalTransitionAutoButton.enabled = internalTransitionCutButton.enabled = _currentPreview != _currentProgram;
					internalTransitionAutoButton.image.color = internalTransitionCutButton.image.color = _currentPreview != _currentProgram ? Color.white : Color.gray;
				}
			}
		}
		get => (MixerStateEnum)_currentPreview;
	}


    // This is a local setting
    private MixerStateEnum _currentDisplayed;
    public MixerStateEnum CurrentDisplayed
    {
        set
        {
            OutlinedButtonComponent currentButton = GetButton(_currentDisplayed, true);
            if (currentButton != null)
            {
                currentButton.text.color = currentButton.outline.color = Color.gray;
                CameraComponent currentCameraComponent = GetCameraComponent(_currentDisplayed);
                if (currentCameraComponent != null && currentCameraComponent.virtualCamera != null)
                {
                    currentCameraComponent.previewCamera.gameObject.SetActive(false);
                    currentCameraComponent.previewCamera.targetTexture = null;
                }
                else if (_currentDisplayed == MixerStateEnum.OutputClean)
                {
                    // TODO
                }
                else if (_currentDisplayed == MixerStateEnum.OutputPVR)
                {
                    // TODO
                }
                else if (_currentDisplayed == MixerStateEnum.OutputMV)
                {
                    // TODO
                }
                else if (_currentDisplayed == MixerStateEnum.OutputPGM)
                {
                    internalProgramCamera.targetTexture = internalFallbackRenderTexture;
                }
            }

            OutlinedButtonComponent newButton = GetButton(value, true);
			if (newButton != null)
			{
                CameraComponent cameraComponent = GetCameraComponent(value);
                if (cameraComponent != null && cameraComponent.virtualCamera != null)
                {
                    cameraComponent.previewCamera.targetTexture = internalRenderTexture;
                    cameraComponent.previewCamera.gameObject.SetActive(true);
                }
                else if (value == MixerStateEnum.OutputClean)
				{
					// TODO
				}
				else if (value == MixerStateEnum.OutputPVR)
				{
					// TODO
				}
				else if (value == MixerStateEnum.OutputMV)
				{
					// TODO
				}
				else if (value == MixerStateEnum.OutputPGM)
				{
					internalProgramCamera.targetTexture = internalRenderTexture;
				}
				else
				{
					return;
                }
                newButton.text.color = newButton.outline.color = Color.white;
            }
			_currentDisplayed = value;
        }
        get => _currentDisplayed;
    }

    void Start()
	{
		for (byte currentIndex = (byte)MixerStateEnum.Input1; currentIndex <= (byte)MixerStateEnum.OutputPGM; currentIndex++)
		{
            Button inputButton = GetButton(currentIndex);
			if (inputButton != null)
			{
				bool hasVirtualCamera = GetVirtualCamera(currentIndex) != null || currentIndex > (byte)MixerStateEnum.Input8;
				inputButton.enabled = hasVirtualCamera;
                inputButton.image.color = hasVirtualCamera ? Color.white : Color.gray;
            }

            OutlinedButtonComponent outputButton = GetButton(currentIndex, true);
            if (outputButton != null)
            {
                outputButton.text.color = outputButton.outline.color = Color.gray;
            }
        }

		CurrentDisplayed = MixerStateEnum.OutputPGM;
    }


	#region Custom Event Listeners
	public void OnButtonInput1Clicked()
	{
		if (input1 != null)
		{
			CurrentPreview = MixerStateEnum.Input1;
			RequestSerialization();
		}
    }

    public void OnButtonInput2Clicked()
	{
		if (input2 != null)
		{
			CurrentPreview = MixerStateEnum.Input2;
			RequestSerialization();
		}
	}

    public void OnButtonInput3Clicked()
	{
		if (input3 != null)
		{
			CurrentPreview = MixerStateEnum.Input3;
			RequestSerialization();
		}
    }

    public void OnButtonInput4Clicked()
	{
		if (input4 != null)
		{
			CurrentPreview = MixerStateEnum.Input4;
			RequestSerialization();
		}
	}

	public void OnButtonInput5Clicked()
	{
		if (input5 != null)
		{
			CurrentPreview = MixerStateEnum.Input5;
			RequestSerialization();
		}
	}

	public void OnButtonInput6Clicked()
	{
		if (input6 != null)
		{
			CurrentPreview = MixerStateEnum.Input6;
			RequestSerialization();
		}
	}

	public void OnButtonInput7Clicked()
	{
		if (input7 != null)
		{
			CurrentPreview = MixerStateEnum.Input7;
			RequestSerialization();
		}
	}

	public void OnButtonInput8Clicked()
	{
		if (input8 != null)
		{
			CurrentPreview = MixerStateEnum.Input8;
			RequestSerialization();
		}
	}

	public void OnButtonInputMP1Clicked()
	{
		//if (inputMP1 != null)
		//{
		CurrentPreview = MixerStateEnum.InputMP1;
		RequestSerialization();
		//}
	}

	public void OnButtonInputMP2Clicked()
	{
		//if (inputMP2 != null)
		//{
		CurrentPreview = MixerStateEnum.InputMP2;
		RequestSerialization();
		//}
	}

	public void OnButtonInputSRCClicked()
	{
		//if (inputSRC != null)
		//{
		//CurrentPreview = MixerStateEnum.InputSRC;
		//RequestSerialization();
		//}
	}

	public void OnButtonInputBlackClicked()
	{
		//if (inputBlack != null)
		//{
		CurrentPreview = MixerStateEnum.InputBlack;
		RequestSerialization();
		//}
	}


    public void OnButtonOutput1Clicked()
    {
        if (input1 != null)
        {
            CurrentDisplayed = MixerStateEnum.Input1;
        }
    }

    public void OnButtonOutput2Clicked()
    {
        if (input2 != null)
        {
            CurrentDisplayed = MixerStateEnum.Input2;
        }
    }

    public void OnButtonOutput3Clicked()
    {
        if (input3 != null)
        {
            CurrentDisplayed = MixerStateEnum.Input3;
        }
    }

    public void OnButtonOutput4Clicked()
    {
        if (input4 != null)
        {
            CurrentDisplayed = MixerStateEnum.Input4;
        }
    }

    public void OnButtonOutput5Clicked()
    {
        if (input5 != null)
        {
            CurrentDisplayed = MixerStateEnum.Input5;
        }
    }

    public void OnButtonOutput6Clicked()
    {
        if (input6 != null)
        {
            CurrentDisplayed = MixerStateEnum.Input6;
        }
    }

    public void OnButtonOutput7Clicked()
    {
        if (input7 != null)
        {
            CurrentDisplayed = MixerStateEnum.Input7;
        }
    }

    public void OnButtonOutput8Clicked()
    {
        if (input8 != null)
        {
            CurrentDisplayed = MixerStateEnum.Input8;
        }
    }

    public void OnButtonOutputCleanClicked()
    {
        CurrentDisplayed = MixerStateEnum.OutputClean;
    }

    public void OnButtonOutputPVRClicked()
    {
        CurrentDisplayed = MixerStateEnum.OutputPVR;
    }

    public void OnButtonOutputMVClicked()
    {
        CurrentDisplayed = MixerStateEnum.OutputMV;
    }

    public void OnButtonOutputPGMClicked()
    {
		CurrentDisplayed = MixerStateEnum.OutputPGM;
    }

    public void OnButtonTransitionCutClicked()
	{
		MixerStateEnum oldProgram = CurrentProgram;
		MixerStateEnum oldPreview = CurrentPreview;
		CurrentPreview = MixerStateEnum.None;
		CurrentProgram = oldPreview;
		CurrentPreview = oldProgram;
		RequestSerialization();
	}

	public void OnButtonTransitionAutoClicked()
	{
		MixerStateEnum oldProgram = CurrentProgram;
		MixerStateEnum oldPreview = CurrentPreview;
		CurrentPreview = MixerStateEnum.None;
		CurrentProgram = oldPreview;
		CurrentPreview = oldProgram;
		// @TODO make this auto, instead of cut
		RequestSerialization();
	}
	#endregion


	#region Utilities
	private CameraComponent GetCameraComponent(MixerStateEnum input)
	{
		if (input == MixerStateEnum.Input1) return input1;
		else if (input == MixerStateEnum.Input2) return input2;
		else if (input == MixerStateEnum.Input3) return input3;
		else if (input == MixerStateEnum.Input4) return input4;
		else if (input == MixerStateEnum.Input5) return input5;
		else if (input == MixerStateEnum.Input6) return input6;
		else if (input == MixerStateEnum.Input7) return input7;
		else if (input == MixerStateEnum.Input8) return input8;
		return null;
	}

	private CinemachineVirtualCamera GetVirtualCamera(MixerStateEnum input)
	{
        CameraComponent cameraComponent = GetCameraComponent(input);
		if (cameraComponent != null)
			return cameraComponent.virtualCamera;
		return null;
    }

    private CinemachineVirtualCamera GetVirtualCamera(byte input)
    {
        return GetVirtualCamera((MixerStateEnum)input);
    }


    private OutlinedButtonComponent GetButton(MixerStateEnum input, bool wantsOutput)
    {
		if (!wantsOutput) return null;
        if (input == MixerStateEnum.Input1) return internalOutput1Button;
        else if (input == MixerStateEnum.Input2) return internalOutput2Button;
        else if (input == MixerStateEnum.Input3) return internalOutput3Button;
        else if (input == MixerStateEnum.Input4) return internalOutput4Button;
        else if (input == MixerStateEnum.Input5) return internalOutput5Button;
        else if (input == MixerStateEnum.Input6) return internalOutput6Button;
        else if (input == MixerStateEnum.Input7) return internalOutput7Button;
        else if (input == MixerStateEnum.Input8) return internalOutput8Button;
        else if (input == MixerStateEnum.OutputClean) return internalOutputCleanButton;
        else if (input == MixerStateEnum.OutputPVR) return internalOutputPVRButton;
        else if (input == MixerStateEnum.OutputMV) return internalOutputMVButton;
        else if (input == MixerStateEnum.OutputPGM) return internalOutputPGMButton;
        return null;
    }

    private Button GetButton(MixerStateEnum input)
	{
		if (input == MixerStateEnum.Input1) return internalInput1Button;
		else if (input == MixerStateEnum.Input2) return internalInput2Button;
		else if (input == MixerStateEnum.Input3) return internalInput3Button;
		else if (input == MixerStateEnum.Input4) return internalInput4Button;
		else if (input == MixerStateEnum.Input5) return internalInput5Button;
		else if (input == MixerStateEnum.Input6) return internalInput6Button;
		else if (input == MixerStateEnum.Input7) return internalInput7Button;
		else if (input == MixerStateEnum.Input8) return internalInput8Button;

		else if (input == MixerStateEnum.InputMP1) return internalInputMP1Button;
		else if (input == MixerStateEnum.InputMP2) return internalInputMP2Button;
		else if (input == MixerStateEnum.InputSRC) return internalInputSRCButton;
		else if (input == MixerStateEnum.InputBlack) return internalInputBlackButton;
        return null;
	}

    private OutlinedButtonComponent GetButton(byte input, bool wantsOutput)
    {
        return GetButton((MixerStateEnum)input, wantsOutput);
    }

    private Button GetButton(byte input)
	{
		return GetButton((MixerStateEnum)input);
	}
	#endregion
}
