using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DataController : MonoBehaviour
{
    string saveFolder;
    string userFolder;

    int totalPlayers;
    int totalTeams;
    int totalTournaments;

    int playersLoaded;
    int teamsLoaded;
    int tournamentsLoaded;

    bool isLoadingGame;
    bool isLoadingPlayers;
    bool isLoadingTeams;
    bool isLoadingTournaments;

    private void Start()
    {
        saveFolder = CombinePaths(Application.persistentDataPath, "SaveFiles/");
    }

    public void CreateUserData(string _name)
    {
        UserData userData = new UserData
        {
            Name = _name,
            Id = System.Guid.NewGuid().ToString()
        };
        MainController.Instance.User = userData;
        userFolder = CombinePaths(saveFolder, userData.Id);
        if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);
        if (Directory.Exists(userFolder))
        {
            EditorUtility.DisplayDialog("File name already exists",
                                        "Please choose a different name for you save file.",
                                        "OK");
        }
        else
        {
            Directory.CreateDirectory(userFolder);
            SaveGame();
        }
    }

    #region SAVE
    public void SaveGame()
    {
        SaveUser();
        SavePlayers();
        SaveTeams();
        SaveTournaments();
        print("GAME SAVED");
        LoadGame(MainController.Instance.User.Id);
    }

    public void SaveUser()
    {
        UserData user = MainController.Instance.User;
        SaveData(user, "UserData");
    }

    public void SavePlayers()
    {
        foreach (PlayerData player in MainController.Instance.AllPlayers)
        {
            SaveData(player.Attributes, player.Id, "Players");
        }
    }

    public void SaveTeams()
    {
        foreach (TeamData team in MainController.Instance.AllTeams)
        {
            SaveData(team.Attributes, team.Id, "Teams");
        }
    }

    public void SaveTournaments()
    {
        foreach (TournamentData tournament in MainController.Instance.AllTournaments)
        {
            SaveData(tournament.Attributes, tournament.Id, "Tournaments");
        }
    }
    #endregion

    #region LOAD
    public void LoadGame(string _user)
    {
        isLoadingGame = true;
        userFolder = CombinePaths(saveFolder, _user);
        LoadUser();
        LoadPlayers();
        //LoadTeams();
        //LoadTournaments();
    }

    public void LoadUser()
    {
        MainController.Instance.User = LoadData<UserData>("UserData");
    }

    public void LoadPlayers()
    {
        MainController.Instance.AllPlayers = new List<PlayerData>();
        isLoadingPlayers = true;
        string[] files = Directory.GetFiles(CombinePaths(userFolder, "Players"));
        totalPlayers = files.Length;
        foreach (string file in files)
        {
            PlayerAttributes data = LoadFile<PlayerAttributes>(file);
            PlayerData player = new PlayerData
            {
                Attributes = data
            };
            MainController.Instance.AllPlayers.Add(player);
            playersLoaded++;
        }
    }

    public void LoadTeams()
    {
        MainController.Instance.AllTeams = new List<TeamData>();
        isLoadingTeams = true;
        string[] files = Directory.GetFiles(CombinePaths(userFolder, "Teams"));
        totalTeams = files.Length;
        foreach (string file in files)
        {
            TeamAttributes data = LoadFile<TeamAttributes>(file);
            TeamData team = new TeamData
            {
                Attributes = data
            };
            team.Initialize(true);
            MainController.Instance.AllTeams.Add(team);
            teamsLoaded++;
        }
    }

    public void LoadTournaments()
    {
        MainController.Instance.AllTournaments = new List<TournamentData>();
        isLoadingTournaments = true;
        string[] files = Directory.GetFiles(CombinePaths(userFolder, "Tournaments"));
        totalTournaments = files.Length;
        foreach (string file in files)
        {
            TournamentAttributes data = LoadFile<TournamentAttributes>(file);
            TournamentData tournament = new TournamentData
            {
                Attributes = data
            };
            tournament.LoadTeams();
            MainController.Instance.AllTournaments.Add(tournament);
            tournamentsLoaded++;
        }
    }

    public UserData[] GetSaveFiles()
    {
        if (saveFolder == null) saveFolder = CombinePaths(Application.persistentDataPath, "SaveFiles/");
        List<UserData> datas = new List<UserData>();
        if (!Directory.Exists(saveFolder)) return datas.ToArray();

        string[] dirs = Directory.GetDirectories(saveFolder);
        foreach (string folder in dirs)
        {
            UserData data = LoadFile<UserData>(CombinePaths(folder, "UserData.txt"));
            datas.Add(data);
        }

        return datas.ToArray();
    }
    #endregion

    #region GENERICS
    public void SaveData<T>(T _data, string _name, string _subfolder = "")
    {
        string path = "";
        string folder = CombinePaths(userFolder, _subfolder);

        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        
        path = Path.Combine(folder, _name + ".txt");
        string jsonString = JsonUtility.ToJson(_data);
        using (StreamWriter streamWriter = File.CreateText(path))
        {
            streamWriter.Write(jsonString);
        }
    }

    public T LoadData<T>(string _fileName)
    {
        string subfolder = "";

        if (typeof(T) == typeof(PlayerData)) subfolder = "/Players/";
        else if (typeof(T) == typeof(TeamData)) subfolder = "/Teams/";
        else if (typeof(T) == typeof(TournamentData)) subfolder = "/Tournaments/";

        string path;
        string folder = CombinePaths(userFolder, subfolder);
        T file;
        path = CombinePaths(folder, _fileName + ".txt");
        using (StreamReader streamReader = File.OpenText(path))
        {
            string jsonString = streamReader.ReadToEnd();
            file = JsonUtility.FromJson<T>(jsonString);
        }

        return file;
    }

    public T LoadFile<T>(string _path)
    {
        T file;
        using (StreamReader streamReader = File.OpenText(_path))
        {
            string jsonString = streamReader.ReadToEnd();
            file = JsonUtility.FromJson<T>(jsonString);
        }

        return file;
    }

    static string RemoveSpaces(string _str)
    {
        string str = _str.Replace(" ", "_");
        return str;
    }

    static string CombinePaths(string str_1, string str_2)
    {
        string str = "";

        str = Path.Combine(str_1, str_2);
        str = str.Replace("\\", "/");

        return str;
    }
    #endregion

    void FinishLoadingGame()
    {
        isLoadingGame = false;
        MainController.Instance.Screens.ShowScreen(BaseScreen.ScreenType.MainMenu);
        print("Finished Loading Game");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            SaveGame();

        if(isLoadingGame)
        {
            if(isLoadingTournaments)
            {
                print("Tournaments loaded: " + tournamentsLoaded + "/" + totalTournaments);
                if (tournamentsLoaded == totalTournaments) FinishLoadingGame();
            }
            else if(isLoadingTeams)
            {
                print("Teams loaded: " + teamsLoaded + "/" + totalTeams);
                if (teamsLoaded == totalTeams) LoadTournaments();
            }
            else if (isLoadingPlayers)
            {
                print("Players loaded: " + playersLoaded + "/" + totalPlayers);
                if (playersLoaded == totalPlayers) LoadTeams();
            }
        }
    }
}