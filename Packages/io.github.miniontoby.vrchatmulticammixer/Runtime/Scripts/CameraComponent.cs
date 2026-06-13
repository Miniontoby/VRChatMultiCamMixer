using Cinemachine;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class CameraComponent : UdonSharpBehaviour
{
	[Tooltip("Assign a pickup for it to get its interact and use text automatically get updated to say the number of the camera input. Autodetected if not supplied")]
	public VRCPickup pickup = null;
	public CinemachineVirtualCamera virtualCamera = null;
	public Camera previewCamera = null;
	public ScreenComponent previewScreen = null;
	public TallyLightComponent tallyLight = null;

	private void Start()
	{
		if (pickup == null)
			pickup = GetComponentInChildren<VRCPickup>();
		if (virtualCamera == null)
			virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
		if (previewCamera == null)
			previewCamera = GetComponentInChildren<Camera>();
		if (previewScreen == null)
			previewScreen = GetComponentInChildren<ScreenComponent>();
		if (tallyLight == null)
			tallyLight = GetComponentInChildren<TallyLightComponent>();
	}

	public override void OnPickup()
	{
		if (previewCamera != null)
		{
			previewCamera.gameObject.SetActive(true);
			if (previewScreen != null && previewScreen.renderTexture != null)
			{
				previewScreen.gameObject.SetActive(true);
				previewCamera.targetTexture = previewScreen.renderTexture;
			}
		}
	}

	public override void OnDrop()
	{
		if (previewScreen != null)
		{
			previewScreen.gameObject.SetActive(false);
		}
		if (previewCamera != null)
		{
			previewCamera.gameObject.SetActive(false);
			previewCamera.targetTexture = null;
		}
	}
}
