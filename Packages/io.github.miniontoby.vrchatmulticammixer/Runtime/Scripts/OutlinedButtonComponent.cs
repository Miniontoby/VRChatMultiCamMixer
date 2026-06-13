
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class OutlinedButtonComponent : UdonSharpBehaviour
{
	[Tooltip("Will autodetect if not specified")]
	[SerializeField] private Image outline = null;
	[Tooltip("Will autodetect if not specified")]
	[SerializeField] private TextMeshProUGUI text = null;

	private void Start()
	{
		if (outline == null)
			outline = GetComponentInChildren<Image>();
		if (text == null)
			text = GetComponentInChildren<TextMeshProUGUI>();
	}

	public void SetColor(Color color)
	{
		if (text != null && outline != null)
			text.color = outline.color = color;
	}
}
