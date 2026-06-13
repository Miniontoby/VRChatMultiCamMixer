using UdonSharp;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class TallyLightComponent : UdonSharpBehaviour
{
    [Tooltip("Should get autodetected")]
    private MeshRenderer meshRenderer = null;

    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        if (meshRenderer == null)
            Debug.LogWarningFormat("[TallyLightComponent] '{0}' could not find its meshRenderer!", gameObject.name);
        else if (meshRenderer.material == null)
            Debug.LogWarningFormat("[TallyLightComponent] '{0}' could not find its meshRenderer.material!", gameObject.name);
    }

    public Color Color
    {
        set
        {
            if (meshRenderer != null && meshRenderer.material != null)
                meshRenderer.material.color = value;
        }
    }
}
