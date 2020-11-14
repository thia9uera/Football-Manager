using UnityEngine;
using UnityEngine.UI;

public class DefaultTab : MonoBehaviour
{
	public ScreenType Screen;
	
	[SerializeField] private Button button = null;
	
	private TabsController controller = null;
	
	public void Initialize()
	{
		controller = GetComponentInParent<TabsController>();
	}
	
	public void OnValueChanged()
	{
		controller.OnTabSelected(Screen);
	}
	
	public void Select()
	{
		button.Select();
	}
	
	public void Enable(bool _value)
	{
		button.interactable = _value;
	}
}
