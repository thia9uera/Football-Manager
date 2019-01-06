using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;

public class LoadingScreen : BaseScreen
{
    [SerializeField]
    TextMeshProUGUI label;

    [SerializeField]
    TMP_InputField inputField;

    [SerializeField]
    Transform content;

    [SerializeField]
    ButtonDefault btnTemplate;

    List<GameObject> buttons;

    float time;
    bool isLoaded;

    public override void Show()
    {
        base.Show();
        LoadBundles();

        List<string> saves = MainController.Instance.Data.GetSaveFiles();
        if (saves.Count > 0) PopulateLoadFiles(saves);

        inputField.text = "Save " + (saves.Count + 1);
    }

    void LoadBundles()
    {
        time = Time.time;
        int loaded = 0;
        int total = 0;

#if UNITY_EDITOR
        PlayerData[] players = Tools.GetAtSubfolders<PlayerData>("Data/Players");
        TeamData[] teams = Tools.GetAtFolder<TeamData>("Data/Teams");
        TournamentData[] tournaments = Tools.GetAtFolder<TournamentData>("Data/Tournaments");

        MainController.Instance.AllPlayers = new List<PlayerData>(players);
        MainController.Instance.AllTeams = new List<TeamData>(teams);
        MainController.Instance.AllTournaments = new List<TournamentData>(tournaments);

        foreach (TeamData team in teams) team.Initialize();

        //isLoaded = true;
        print("LOADED FROM FILES");

        return;
#endif
        AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "data_bundle"));
        if (bundle == null)
        {
            label.text = "Failed to load AssetBundle =[";
            return;
        }

        Object[] list = bundle.LoadAllAssets();
        total = list.Length;
        foreach(Object obj in list)
        {
            loaded++;
            if (obj is PlayerData) MainController.Instance.AllPlayers.Add((PlayerData)obj);
            else if (obj is TeamData) MainController.Instance.AllTeams.Add((TeamData)obj);
            else if (obj is TournamentData) MainController.Instance.AllTournaments.Add((TournamentData)obj);
        }

        if (loaded == total) isLoaded = true;

    }

    void PopulateLoadFiles(List<string> _files)
    {
        if (buttons != null) foreach (GameObject go in buttons) Destroy(go);
        buttons = new List<GameObject>();

        foreach (string str in _files)
        {
            ButtonDefault btn = Instantiate(btnTemplate, content);
            btn.Label = str;
            buttons.Add(btn.gameObject);
            btn.GetComponent<Button>().onClick.AddListener(delegate 
            {
                LoadFile(str);
                
            });
        }
    }

    void LoadFile(string _file)
    {
        MainController.Instance.Data.LoadGame(_file);
    }

    private void Update()
    {
        if(Time.time - time >= 1f)
        {
            if (isLoaded) MainController.Instance.Screens.ShowScreen(ScreenType.MainMenu);
            else time = Time.time;
        }
    }


    public void CreateNewFile()
    {
        MainController.Instance.Data.CreateUserData(inputField.text);

        MainController.Instance.Screens.ShowScreen(ScreenType.MainMenu);
    }
}
