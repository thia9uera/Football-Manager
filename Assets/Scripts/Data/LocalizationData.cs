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

    public string GetLongPositionString(PlayerAttributes.PlayerPosition _pos)
    {
        string color = "#8033cc";

        switch(_pos)
        {
            case PlayerAttributes.PlayerPosition.Goalkeeper: color = "#8033cc"; break;
            case PlayerAttributes.PlayerPosition.Defender: color = "#5299cc"; break;
            case PlayerAttributes.PlayerPosition.Midfielder: color = "#cc8f14"; break;
            case PlayerAttributes.PlayerPosition.Forward: color = "#e55c5c"; break;
        }

        LocalizedString str = "<color=" + color + ">pos_" + positionKeys[(int)_pos] + "</color>";

        return  str;
    }

    public string GetShortPositionString(PlayerAttributes.PlayerPosition _pos)
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
}
