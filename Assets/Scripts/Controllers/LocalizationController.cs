using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

public class LocalizationController : MonoBehaviour
{
	public static LocalizationController Instance;
	
	[SerializeField] private LocalizationData data = null;
	
	public enum Language
	{
		English,
		Portuguese,
	}
	
	private void Awake()
	{
		if (Instance == null) Instance = this;
	}

	public string GetLanguageString(Language _language)
	{
		string language = "English";

		switch (_language)
		{
			case Language.English: language = "English"; break;
			case Language.Portuguese: language = "Portuguese"; break;
		}

		return language;
	}

	public void Initialize()
	{
		CurrentLanguage = Language.English;
	}

	[SerializeField]
	private Language currentlanguage;
	public Language CurrentLanguage
	{
		set
		{
			currentlanguage = value;
			LocalizationManager.CurrentLanguage = GetLanguageString(currentlanguage);
			SetRandomNamesData();
		}
		get
		{
			return currentlanguage;
		}
	}
	
	public string GetLongPositionString(PlayerPosition _pos)
	{		
		LocalizedString locStr = "pos_" + data.PositionKeys[(int)_pos];
		return locStr.ToString().Replace("{0}", GetPositionColor(_pos));
	}
	
	public string GetShortPositionString(PlayerPosition _pos)
	{
		LocalizedString locStr = "posTag_" + data.PositionKeys[(int)_pos];
		return locStr.ToString().Replace("{0}", GetPositionColor(_pos));
	}
	
	private string GetPositionColor(PlayerPosition _pos)
	{
		string color = "#8033cc";

		switch(_pos)
		{
			case PlayerPosition.Goalkeeper: color = "#8033cc"; break;
			case PlayerPosition.Defender: color = "#5299cc"; break;
			case PlayerPosition.Midfielder: color = "#cc8f14"; break;
			case PlayerPosition.Forward: color = "#bf4830"; break;
		}
		
		return color;
	}

	public string GetZoneString(Zone _zone)
	{
		int zone = (int)_zone;
		string str = "";

		if(zone > 22)
		{
			str = Localize("zone_Attack");
		}
		else if(zone > 7)
		{
			str = Localize("zone_Midfield");
		}
		else
		{
			str = Localize("zone_Defense");
		}

		return str;
	}

	public string Localize(string _text)
	{
		LocalizedString str = _text;
		if(string.IsNullOrEmpty(str)) Debug.Log("STRING NOT FOUND: " + _text);
		return str;
	}

	string[] shortNotation = new string[12] { "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc" };

	public string FormatBigNumber(float target)
	{
		float value = target;
		int baseValue = 0;
		string notationValue = "";
		string toStringValue;

		if (value >= 10000) // I start using the first notation at 10k
		{
			value /= 1000;
			baseValue++;
			while (Mathf.Round((float)value) >= 1000)
			{
				value /= 1000;
				baseValue++;
			}

			if (baseValue < 2)
				toStringValue = "N1"; // display 1 decimal while under 1 million
			else
				toStringValue = "N2"; // display 2 decimals for 1 million and higher

			if (baseValue > shortNotation.Length) return null;
			else notationValue = shortNotation[baseValue];
		}
		else toStringValue = "N0"; // string formatting at low numbers
		return value.ToString(toStringValue) + notationValue;
	}
	
	[System.Serializable]
	public class RandomNameLoc
	{
		public LocalizationController.Language Language;
		public RandomNamesData Data;
	}
	
	public void SetRandomNamesData()
	{
		RandomNamesData rnData = data.RandomNameLocalizations[0].Data;;
		foreach(RandomNameLoc loc in data.RandomNameLocalizations)
		{
			if(loc.Language == LocalizationController.Instance.currentlanguage) rnData = loc.Data;
		}
		randomNameData = rnData;
	}
	
	
	private RandomNamesData randomNameData;
	public string GetRandomFullName()
	{
		return GetRandomFirstName() +" "+ GetRandomLastName();
	}
	
	public string GetRandomFirstName()
	{
		return randomNameData.FirstNames[Random.Range(0, randomNameData.FirstNames.Count)];
	}
	
	public string GetRandomLastName()
	{
		return randomNameData.LastNames[Random.Range(0, randomNameData.LastNames.Count)];
	}
	
	public string GetRandomTeamName()
	{
		return randomNameData.TeamNames[Random.Range(0, randomNameData.TeamNames.Count)];
	}
}
