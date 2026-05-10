using UdonSharp;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ScreenComponent : UdonSharpBehaviour
{
    public RenderTexture renderTexture = null;
}
