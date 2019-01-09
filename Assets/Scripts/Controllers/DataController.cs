using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DataController : MonoBehaviour
{
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
        userData.Id = System.Guid.NewGuid().ToString();
        MainController.Instance.User = userData;
        if(saveFolder == null) saveFolder = CombinePaths(Application.persistentDataPath, "SaveFiles/");
        userFolder = CombinePaths(saveFolder, userData.Id);
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
            SaveData(player.Attributes, player.Id, "/Players/");
        }
    }

    public void SaveTeams()
    {
        foreach (TeamData team in MainController.Instance.AllTeams)
        {
            SaveData(team.Attributes, team.Id, "/Teams/");
        }
    }

    public void SaveTournaments()
    {
        foreach (TournamentData tournament in MainController.Instance.AllTournaments)
        {
            SaveData(tournament.Attributes, tournament.Id, "/Tournaments/");
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

    public UserData[] GetSaveFiles()
    {
        Debug.Log("GET FILES");
        List<UserData> datas = new List<UserData>();
        if (!Directory.Exists(saveFolder)) return datas.ToArray();

        string[] dirs = Directory.GetDirectories(saveFolder);
        for (int i= 0; i < dirs.Length; i++)
        {
            UserData data = LoadFile<UserData>(CombinePaths(dirs[i], "UserData.txt"));

            datas.Add(data);
        }

        return datas.ToArray();
    }
    #endregion

    #region GENERICS
    public void SaveData<T>(T _data, string _name, string _subfolder = "")
    {
        string path = "";
        string subfolder = "";

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            SaveGame();
    }
}