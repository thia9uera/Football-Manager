using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DataController : MonoBehaviour
{
    string[] saveFolders = new string[3]{"save_0", "save_1", "save_2"};
    string saveFolder;
    string userFolder;

    private void Start()
    {
        saveFolder = CombinePaths(Application.persistentDataPath, "SaveFiles/");
    }

    public void CreateUserData(string _name)
    {
        UserData userData = new UserData();
        userData.Name = _name;
        MainController.Instance.User = userData;
        userFolder = CombinePaths(saveFolder, _name);
        if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);
        if (Directory.Exists(userFolder))
        {
            EditorUtility.DisplayDialog("File name already exists",
                                        "Please choose a different name for you save file.",
                                        "OK");
        }
        else Directory.CreateDirectory(userFolder);
    }

    #region SAVE
    public void SaveGame()
    {
        SaveUser();
        SavePlayers();
        SaveTeams();
        SaveTournaments();
        print("GAME SAVED");
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
            SaveData(player, RemoveSpaces(player.FirstName + player.LastName));
        }
    }

    public void SaveTeams()
    {
        foreach (TeamData team in MainController.Instance.AllTeams)
        {
            SaveData(team, RemoveSpaces(team.Name));
        }
    }

    public void SaveTournaments()
    {
        foreach (TournamentData tournament in MainController.Instance.AllTournaments)
        {
            SaveData(tournament, RemoveSpaces(tournament.Name));
        }
    }
    #endregion

    #region LOAD
    public void LoadGame(string _user)
    {
        userFolder = _user;
        LoadUser();
        LoadPlayers();
        LoadTeams();
        LoadTournaments();
    }

    public void LoadUser()
    {
        MainController.Instance.User = LoadData<UserData>("UserData");
    }

    public void LoadPlayers()
    {
        foreach(PlayerData p in MainController.Instance.AllPlayers)
        {
            PlayerData player = p;
            player = LoadData<PlayerData>(RemoveSpaces(player.FirstName + player.LastName));
        }
    }

    public void LoadTeams()
    {
        foreach (TeamData t in MainController.Instance.AllTeams)
        {
            TeamData team = t;
            team = LoadData<TeamData>(RemoveSpaces(team.Name));
        }
    }

    public void LoadTournaments()
    {
        foreach (TournamentData t in MainController.Instance.AllTournaments)
        {
            TournamentData tournament = t;
            tournament = LoadData<TournamentData>(RemoveSpaces(tournament.Name));
        }
    }

    public List<string> GetSaveFiles()
    {
        List<string> strs = new List<string>();

        if (!Directory.Exists(saveFolder)) return strs;

        string[] dirs = Directory.GetDirectories(saveFolder);
        foreach (string dir in dirs)
        {
            UserData data = LoadFile<UserData>(CombinePaths(dir, "UserData.txt"));
            
            strs.Add(data.Name);
        }

        return strs;
    }
    #endregion

    #region GENERICS
    public void SaveData<T>(T _data, string _name)
    {
        string path = "";
        string subfolder = "";

        if (typeof(T) == typeof(PlayerData))
        {
            subfolder = "/Players/";
        }
        else if (typeof(T) == typeof(TeamData))
        {
            subfolder = "/Teams/";
        }
        else if (typeof(T) == typeof(TournamentData)) subfolder = "/Tournaments/";

        string folder = CombinePaths(userFolder, subfolder);

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            SaveGame();
    }
}