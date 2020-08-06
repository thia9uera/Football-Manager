using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabsView : MonoBehaviour
{
	public void ShowSquadEditScreen()
	{
		ScreenController.Instance.Manager.SquadScreen.Show();
	}
}
