using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

public class DataController : MonoBehaviour
{
	public static DataController Instance;
	private string saveFolder;
    private string userFolder;

    private int totalPlayers;
    private int totalTeams;
    private int totalTournaments;

    private int playersLoaded;
    private int teamsLoaded;
    private int tournamentsLoaded;

    private bool isLoadingGame;
	private bool isLoadingPlayers;
    private bool isLoadingTeams;
    private bool isLoadingTournaments;
	
	private MainController mainController;
	
	private string extension = ".auxter";
    
	private void Awake()
	{
		if(Instance == null) Instance = this;
	}

    private void Start()
    {
	    saveFolder = CombinePaths(Application.persistentDataPath, "SaveFiles/");
	    mainController = MainController.Instance;
    }

	public void CreateUserData(string _coachName, string _teamName)
    {
        UserData userData = new UserData
        {
	        Name = _coachName,
	        TeamName = _teamName,
	        Id = System.Guid.NewGuid().ToString(),
	        CurrentYear = 2020,
	    	CurrentDay = 0	        
        };
        mainController.User = userData;
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
            SaveGame(true);
        }
    }

    #region SAVE
    public void SaveGame(bool _loadAfter = false)
	{
		mainController.User.LastTimeSaved = System.DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm");
    	
		StartCoroutine("SaveFlow", _loadAfter);
	}
    
	private IEnumerator SaveFlow(bool _loadAfter)
	{
		yield return StartCoroutine("SaveUser");
		yield return StartCoroutine("SavePlayers");
		yield return StartCoroutine("SaveTeams");
		yield return StartCoroutine("SaveTournaments");
		
		       
		if(_loadAfter) LoadGame(mainController.User.Id);
		else print("GAME SAVED");

		print(saveFolder);
	}
	
	public void QuickSave()
	{
		StartCoroutine("QuickSaveFlow");
	}
	
	private IEnumerator QuickSaveFlow()
	{
		yield return StartCoroutine("SaveTeamPlayer", mainController.UserTeam.AllPlayers);
		yield return StartCoroutine("SaveTeam", mainController.UserTeam);
		
		print("GAME SAVED");
	}

	private IEnumerator SaveUser()
    {
        UserData user = mainController.User;
	    SaveData(user, "UserData");
	    yield return false;
    }
    
	private IEnumerator SavePlayers()
	{
		foreach (PlayerData player in mainController.AllPlayers)
        {
            SaveData(player.Attributes, player.Id, "Players");
        }
	    yield return false;
	}
    
	private IEnumerator SaveTeamPlayer(List<PlayerData> _playerList = null)
	{
		foreach (PlayerData player in _playerList)
		{
			SaveData(player.Attributes, player.Id, "Players");
		}
		yield return false;
	}

	private IEnumerator SaveTeams()
	{
		foreach(TeamData team in mainController.AllTeams)
		{
			SaveData(team.Attributes, team.Id, "Teams");
		}
	    yield return false;
	}
    
	private IEnumerator SaveTeam(TeamData _team)
	{
		SaveData(_team.Attributes, _team.Id, "Teams");
		yield return false;
	}

    private IEnumerator SaveTournaments()
    {
        foreach (TournamentData tournament in mainController.AllTournaments)
        {
            SaveData(tournament.Attributes, tournament.Id, "Tournaments");
        }
	    yield return false;
    }
    #endregion

    #region LOAD
    public void LoadGame(string _user)
    {
        isLoadingGame = true;
        userFolder = CombinePaths(saveFolder, _user);
	    StartCoroutine("LoadFlow");
    }
    
	private IEnumerator LoadFlow()
	{
		yield return StartCoroutine("LoadUser");
		yield return StartCoroutine("LoadPlayers");
		yield return StartCoroutine("LoadTeams");
		yield return StartCoroutine("LoadTournaments");
		
		CalendarController.Instance.InitializeCalendar();
		ScreenController.Instance.ShowScreen(ScreenType.Manager);
	}

	public IEnumerator LoadUser()
    {
	    mainController.User = LoadData<UserData>("UserData");
	    yield return null;
    }

	private IEnumerator LoadPlayers()
    {
        mainController.AllPlayers = new List<PlayerData>();
        isLoadingPlayers = true;
        string[] files = Directory.GetFiles(CombinePaths(userFolder, "Players"));
        totalPlayers = files.Length;
        foreach (string file in files)
        {
            PlayerAttributes data = LoadFile<PlayerAttributes>(file);
            PlayerData player = ScriptableObject.CreateInstance<PlayerData>();
            player.Attributes = data;

            mainController.AllPlayers.Add(player);
            playersLoaded++;
        }
	    yield return null;
    }

	private IEnumerator LoadTeams()
    {
        mainController.AllTeams = new List<TeamData>();
        isLoadingTeams = true;
        string[] files = Directory.GetFiles(CombinePaths(userFolder, "Teams"));
        totalTeams = files.Length;
        foreach (string file in files)
        {
            TeamAttributes data = LoadFile<TeamAttributes>(file);
            TeamData team = ScriptableObject.CreateInstance<TeamData>();
            team.Attributes = data;
            team.Initialize(true);

            if (team.IsUserControlled) mainController.UserTeam = team;
            mainController.AllTeams.Add(team);
            teamsLoaded++;
        }
	    yield return null;
    }

	private IEnumerator LoadTournaments()
    {
	    mainController.AllTournaments = new List<TournamentData>();
	    mainController.ActiveTournaments.Clear();
        isLoadingTournaments = true;
        string[] files = Directory.GetFiles(CombinePaths(userFolder, "Tournaments"));
	    totalTournaments = files.Length;
	    
        foreach (string file in files)
        {
            TournamentAttributes data = LoadFile<TournamentAttributes>(file);
            TournamentData tournament = ScriptableObject.CreateInstance<TournamentData>();
            tournament.Attributes = data;
            tournament.LoadTeams();
	        mainController.AllTournaments.Add(tournament);
                    
	        if(mainController.UserTeam.TournamentStatistics.ContainsKey(data.Id))
	        {
	        	mainController.ActiveTournaments.Add(tournament.Id);
	        }
            tournamentsLoaded++;
        }
	    yield return null;
    }

	public UserData[] GetSaveFiles()
    {
        if (saveFolder == null) saveFolder = CombinePaths(Application.persistentDataPath, "SaveFiles/");
        List<UserData> datas = new List<UserData>();
        if (!Directory.Exists(saveFolder)) return datas.ToArray();

        string[] dirs = Directory.GetDirectories(saveFolder);
        foreach (string folder in dirs)
        {
            UserData data = LoadFile<UserData>(CombinePaths(folder, "UserData" + extension));
            datas.Add(data);
        }

        return datas.ToArray();
    }
    #endregion

    #region GENERICS
    public void SaveData<T>(T _data, string _name, string _subfolder = "")
    {
       string folder = CombinePaths(userFolder, _subfolder);

       if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

       string path = Path.Combine(folder, _name + extension);
       string jsonString = JsonUtility.ToJson(_data);
       using (StreamWriter streamWriter = File.CreateText(path))
       {
          streamWriter.Write(jsonString);
       }

       /*
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        path = Path.Combine(folder, _name + extension);
        using (FileStream fileStream = File.Open(path, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, _data);
            fileStream.Position = 0;
        }
        */
    }

    public T LoadData<T>(string _fileName)
    {
        string subfolder = "";

        if (typeof(T) == typeof(PlayerData)) subfolder = "/Players/";
        else if (typeof(T) == typeof(TeamData)) subfolder = "/Teams/";
        else if (typeof(T) == typeof(TournamentData)) subfolder = "/Tournaments/";

        string path;
        string folder = CombinePaths(userFolder, subfolder);

        path = CombinePaths(folder, _fileName + extension);
        T file;
        using (StreamReader streamReader = File.OpenText(path))
        {
            string jsonString = streamReader.ReadToEnd();
            file = JsonUtility.FromJson<T>(jsonString);
        }

        /*
         BinaryFormatter binaryFormatter = new BinaryFormatter();

         using (FileStream fileStream = File.Open(path, FileMode.Open))
         {
             file =  (T)binaryFormatter.Deserialize(fileStream);
             fileStream.Position = 0;
         }
         */
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

        /*
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(_path, FileMode.Open))
        {
            file = (T)binaryFormatter.Deserialize(fileStream);
        }
        */
        return file;
    }

	private static string RemoveSpaces(string _str)
    {
        string str = _str.Replace(" ", "_");
        return str;
    }

	private static string CombinePaths(string str_1, string str_2)
    {
        string str = "";

        str = Path.Combine(str_1, str_2);
        str = str.Replace("\\", "/");

        return str;
    }
    #endregion

    public void WipeSaveData()
    {
        Directory.Delete(saveFolder, true);
    }
    
	public void DeleteSavedData(string _fileName)
	{
		string path = CombinePaths(saveFolder, _fileName);
		DeleteDirectory(path);
	}
	
	public static void DeleteDirectory(string target_dir)
	{
		string[] files = Directory.GetFiles(target_dir);
		string[] dirs = Directory.GetDirectories(target_dir);

		foreach (string file in files)
		{
			File.SetAttributes(file, FileAttributes.Normal);
			File.Delete(file);
		}

		foreach (string dir in dirs)
		{
			DeleteDirectory(dir);
		}

		Directory.Delete(target_dir, false);
	}

    void FinishLoadingGame()
    {
        isLoadingGame = false;
	    //ScreenController.Instance.ShowScreen(ScreenType.MainMenu);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            SaveGame();

    }
}