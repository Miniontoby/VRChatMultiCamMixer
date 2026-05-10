using Cinemachine;
using UdonSharp;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class CameraComponent : UdonSharpBehaviour
{
    public CinemachineVirtualCamera virtualCamera = null;
    public Camera previewCamera = null;
    public ScreenComponent previewScreen = null;

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
