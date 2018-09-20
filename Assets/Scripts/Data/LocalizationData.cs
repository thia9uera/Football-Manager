using UnityEngine;
using System.Collections;
using I2.Loc;

[CreateAssetMenu(fileName = "Localization", menuName = "Localization Data", order = 1)]
public class LocalizationData : ScriptableObject
{
    [HideInInspector]
    public string PLAYER_1, PLAYER_2, TEAM_1, TEAM_2;

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
        LocalizedString str = "pos_" + positionKeys[(int)_pos];

        return  str;
    }

    public string GetShortPositionString(PlayerData.PlayerPosition _pos)
    {
        LocalizedString str = "posTag_" + positionKeys[(int)_pos];

        return str;
    }

    public string Localize(string _text)
    {
        LocalizedString str = _text;
        return str;
    }

}
