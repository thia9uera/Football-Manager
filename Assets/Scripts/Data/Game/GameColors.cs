using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameColors", menuName = "Data/Game Colors", order = 1)]
public class GameColors : ScriptableObject
{
	public Color PlayerColor;
	public Color White;
	public Color LightGray;
	public Color MediumGray;
	public Color GrayText;
	
	[Space(10)]
	public Color Positive;
	public Color Negative;
	public Color Warning;
	
	[Space(10)]
	public Color Goalkeeper;
	public Color Defender;
	public Color Midfielder;
	public Color Attacker;
	
	[Space(20)]
	public List<Color> TeamColors;
	
	public string GoalkeeperHex { get { return ColorUtility.ToHtmlStringRGB(Goalkeeper); }}
	public string DefenderHex { get { return ColorUtility.ToHtmlStringRGB(Defender); }}
	public string MidfielderHex { get { return ColorUtility.ToHtmlStringRGB(Midfielder); }}
	public string AttackerHex { get { return ColorUtility.ToHtmlStringRGB(Attacker); }}
}
