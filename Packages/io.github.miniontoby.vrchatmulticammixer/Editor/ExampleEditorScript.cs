using UnityEditor;

namespace io.github.miniontoby.vrchatmulticammixer
{
	public class ExampleEditorScript
	{
		[MenuItem("Tools/VRMCPM/Control Panel")]
		static void ControlPanel()
		{
			EditorUtility.DisplayDialog("VRChatMultiCamMixer", "Opened This Dialog", "OK");
		}
	}
}
