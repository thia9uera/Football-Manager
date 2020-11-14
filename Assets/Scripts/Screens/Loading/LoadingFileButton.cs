using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingFileButton : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI btnLabel = null;
	[SerializeField] private TextMeshProUGUI timeStamp = null;
	
	public string UserId;

    public string Label
    {
        set { btnLabel.text = value; }
    }
    
	public string TimeStamp
	{
		set { timeStamp.text = value; }
	}
	
	public void OnButtonClickHandler()
	{
		ScreenController.Instance.Start.LoadFile(UserId);
	}
	
	public void OnDeleteButtonClickHandler()
	{
		ScreenController.Instance.Start.DeleteSaveFile(this);
	}
}
