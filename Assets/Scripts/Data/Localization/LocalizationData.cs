using UnityEngine;
using System.Collections;
using I2.Loc;

[CreateAssetMenu(fileName = "Localization", menuName = "Data/Localization Data", order = 1)]
public class LocalizationData : ScriptableObject
{
	public string[] PositionKeys;
	public LocalizationController.RandomNameLoc[] RandomNameLocalizations;
}
