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
    }

    void LoadBundles()
    {
        time = Time.time;
        int loaded = 0;
        int total = 0;

#if UNITY_EDITOR

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
        label.text = "SELECT GAME FILE";

        if (buttons != null) foreach (GameObject go in buttons) Destroy(go);
        buttons = new List<GameObject>();

        LoadingFileButton btnNewGame = Instantiate(btnTemplate, content);
        btnNewGame.Label = "NEW GAME";
        btnNewGame.gameObject.SetActive(true);
        buttons.Add(btnNewGame.gameObject);
        btnNewGame.GetComponent<Button>().onClick.AddListener(delegate
        {
            CreateNewFile();

        });

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
        content.gameObject.SetActive(false);
        label.text = "LOADING...";
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
        content.gameObject.SetActive(false);
        label.text = "PREPARING...";

        PlayerData[] players = Tools.GetAtSubfolders<PlayerData>("Data/Players");
        TeamData[] teams = Tools.GetAtFolder<TeamData>("Data/Teams");
        TournamentData[] tournaments = Tools.GetAtFolder<TournamentData>("Data/Tournaments");

        foreach (PlayerData p in players) MainController.Instance.AllPlayers.Add(Instantiate(p));
        foreach (TeamData t in teams)
        {
            MainController.Instance.AllTeams.Add(Instantiate(t));
            t.Initialize();
        }
        foreach (TournamentData t in tournaments) MainController.Instance.AllTournaments.Add(Instantiate(t));

        string str = "SAVE " + (saves.Length + 1);
        MainController.Instance.Data.CreateUserData(str);

        print("NEW SAVE FILE '" + str + "' CREATED");

        //MainController.Instance.Screens.ShowScreen(ScreenType.MainMenu);
    }

    public void DeleteSaveFiles()
    {
        MainController.Instance.Data.WipeSaveData();
        Show();
    }

}
