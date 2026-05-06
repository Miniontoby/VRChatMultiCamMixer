using UnityEditor;

namespace io.github.miniontoby.vrchatmulticammixer
{
	public class ExampleEditorScript
	{
		[MenuItem("VRMCPM/Control Panel")]
		static void ControlPanel()
		{
			EditorUtility.DisplayDialog("Example Script", "Opened This Dialog", "OK");
		}
	}
}
