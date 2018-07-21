using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localization : MonoBehaviour
{
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

    public Language CurrentLanguage = Language.English;
}
