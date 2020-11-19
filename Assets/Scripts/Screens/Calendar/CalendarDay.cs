using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CalendarDay : MonoBehaviour
{
	[SerializeField] private TMP_Text label = null;
	[SerializeField] private TMP_Text dayLabel = null;
	[SerializeField] private Image frameImage = null;

	public void Populate(MatchDay _data)
	{
		dayLabel.text = _data.Date.Day.ToString();

		TeamData adversaryTeam = null;
		string userTeamId = MainController.Instance.UserTeam.Id;
		string adversaryId;
		string tournamentName;
		MatchData matchData;
		if(_data.IsUserMatchDay(out adversaryId, out tournamentName, out matchData))
		{
			adversaryTeam = MainController.Instance.GetTeamById(adversaryId);
			string teamColor = ColorUtility.ToHtmlStringRGB(adversaryTeam.PrimaryColor);
			string grayColor = ColorUtility.ToHtmlStringRGB(GameData.Instance.Colors.GrayText);
			label.text = "<color=#" + teamColor + ">" + adversaryTeam.Name + "</color>" ;
			label.text += "\n<color=#"+ grayColor + ">" + tournamentName + "</color>";
		}
		else
		{
			label.text = "";
		}

		DateTime currentDate = CalendarController.Instance.CurrentDate;

		if(_data.Date == currentDate) frameImage.color = GameData.Instance.Colors.PlayerColor;
		else if(_data.Date > currentDate) frameImage.color = GameData.Instance.Colors.White;
		else frameImage.color =  GameData.Instance.Colors.MediumGray;



	}
}
