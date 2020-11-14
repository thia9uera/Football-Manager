#if UNITY_EDITOR
using UnityEngine;
using TMPro;

public class NameGeneratorItem : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI label = null;
	
	private PlayerData data;
	private NameGenerator controller;

    public void Populate(PlayerData _data, NameGenerator _controller)
    {
        data = _data;
        controller = _controller;
        label.text = _data.FirstName + " " + _data.LastName;
    }

    public void ClickHandler()
    {
        controller.RemovePlayer(data);
    }
}
#endif
