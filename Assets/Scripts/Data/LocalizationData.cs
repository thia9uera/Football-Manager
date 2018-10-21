using UnityEngine;
using System.Collections;
using I2.Loc;

[CreateAssetMenu(fileName = "Localization", menuName = "Localization Data", order = 1)]
public class LocalizationData : ScriptableObject
{
    [HideInInspector]
    public string PLAYER_1, PLAYER_2, TEAM_1, TEAM_2, ZONE, EXTRA_1;
    public enum Language
    {
        English,
        Portuguese,
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
        CurrentLanguage = currentlanguage;
    }

    [SerializeField]
    private Language currentlanguage;
    public Language CurrentLanguage
    {
        set
        {
            currentlanguage = value;
            LocalizationManager.CurrentLanguage = GetLanguageString(currentlanguage);
        }
        get
        {
            return currentlanguage;
        }
    }

    [SerializeField]
    private string[] positionKeys;

    public string GetLongPositionString(PlayerData.PlayerPosition _pos)
    {
        string color = "#8033cc";

        switch(_pos)
        {
            case PlayerData.PlayerPosition.Goalkeeper: color = "#8033cc"; break;
            case PlayerData.PlayerPosition.Defender: color = "#5299cc"; break;
            case PlayerData.PlayerPosition.Midfielder: color = "#cc8f14"; break;
            case PlayerData.PlayerPosition.Forward: color = "#e55c5c"; break;
        }

        LocalizedString str = "<color=" + color + ">pos_" + positionKeys[(int)_pos] + "</color>";

        return  str;
    }

    public string GetShortPositionString(PlayerData.PlayerPosition _pos)
    {
        LocalizedString str = "posTag_" + positionKeys[(int)_pos];

        return str;
    }

    public string GetZoneString(MatchController.FieldZone _zone)
    {
        int zone = (int)_zone;
        string str = "";

        if(zone > 12)
        {
            str = "zone_Attack";
        }
        else if(zone > 3)
        {
            str = "zone_Midfield";
        }
        else
        {
            str = "zone_Defense";
        }

        return str;
    }

    public string Localize(string _text)
    {
        LocalizedString str = _text;
        return str;
    }

}
