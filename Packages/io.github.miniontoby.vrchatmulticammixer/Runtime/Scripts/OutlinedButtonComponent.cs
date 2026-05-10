
using TMPro;
using UdonSharp;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class OutlinedButtonComponent : UdonSharpBehaviour
{
    public Image outline;
    public TextMeshProUGUI text;
}
