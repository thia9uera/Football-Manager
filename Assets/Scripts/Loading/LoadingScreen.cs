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
    LoadingFileButton btnTemplate;

    List<GameObject> buttons;
    UserData[] saves;

    float time;
    bool isLoaded;

    public override void Show()
    {
        base.Show();

        saves = MainController.Instance.Data.GetSaveFiles();
        if (saves.Length > 0)
        {
            PopulateLoadFiles(saves);
        }
        else
        {
            LoadBundles();
        }

        //inputField.text = "Save " + (saves.Count + 1);
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
        print("FILES LOADED");
        Debug.Log(saves);
        CreateNewFile();

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

    void PopulateLoadFiles(UserData[] _users)
    {
        if (buttons != null) foreach (GameObject go in buttons) Destroy(go);
        buttons = new List<GameObject>();

        foreach (UserData user in _users)
        {
            LoadingFileButton btn = Instantiate(btnTemplate, content);
            btn.Label = user.Name;
            btn.gameObject.SetActive(true);
            buttons.Add(btn.gameObject);
            btn.GetComponent<Button>().onClick.AddListener(delegate 
            {
                LoadFile(user.Id);
                
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
        string name = "SAVE " + (saves.Length + 1);
        MainController.Instance.Data.CreateUserData(name);

        print("NEW SAVE FILE '" + name + "' CREATED");

        MainController.Instance.Screens.ShowScreen(ScreenType.MainMenu);
    }

    void PopulateData()
    {

    }
}
