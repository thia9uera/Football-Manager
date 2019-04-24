#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class NameGenerator : MonoBehaviour
{
    List<PlayerData> fileList;

    [SerializeField]
    NameGeneratorItem itemTemplate;

    [SerializeField]
    Transform content;

    List<GameObject> itemList;

    public List<TeamData> ExcludeList;

    List<PlayerData> playerList;

    [SerializeField]
    PlayerNamesData namesData;
    List<string> firstNames;
    List<string> lastNames;

    private void Awake()
    {
        Clear();
    }

    private void Start()
    {
        //fileList = new List<PlayerData>(Resources.LoadAll<PlayerData>("Players"));
        playerList = new List<PlayerData>(Tools.GetAtSubfolders<PlayerData>("Data/Players"));
        foreach (PlayerData player in playerList)
        {
            if(string.IsNullOrEmpty(player.Id))player.Id = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(player);
        }
        AssetDatabase.SaveAssets();
        //UpdateTeamAttributes();



        //Populate();
        //SetPlayersTeam();
    }

    void UpdateTeamAttributes()
    {
        List<TeamData> list = new List<TeamData>(Tools.GetAtFolder<TeamData>("Data/Teams"));

        foreach(TeamData team in list)
        {
            team.Attributes.Name = team.Name;
            team.Attributes.PrimaryColor = team.PrimaryColor;
            team.Attributes.SecondaryColor = team.SecondaryColor;
            team.Attributes.Strategy = (TeamAttributes.TeamStrategy)team.Strategy;
            team.Attributes.Formation = (FormationData)team.Formation;
            team.Attributes.Tag = team.Tag;
            team.Attributes.Id = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(team);
        }
        AssetDatabase.SaveAssets();
    }

    void UpdatePlayerAttributes()
    {
        foreach (PlayerData player in playerList)
        {
            player.Attributes.FirstName = player.FirstName;
            player.Attributes.LastName = player.LastName;
            player.Attributes.Position = player.Position;
            player.Attributes.Strategy = player.Strategy;
            player.Attributes.Goalkeeping = player.Goalkeeping;
            player.Attributes.Passing = player.Passing;
            player.Attributes.Dribbling = player.Dribbling;
            player.Attributes.Crossing = player.Crossing;
            player.Attributes.Tackling = player.Tackling;
            player.Attributes.Blocking = player.Blocking;
            player.Attributes.Shooting = player.Shooting;
            player.Attributes.Heading = player.Heading;
            player.Attributes.Freekick = player.Freekick;
            player.Attributes.Penalty = player.Penalty;
            player.Attributes.Speed = player.Speed;
            player.Attributes.Strength = player.Strength;
            player.Attributes.Agility = player.Agility;
            player.Attributes.Stamina = player.Stamina;
            player.Attributes.Teamwork = player.Teamwork;
            player.Attributes.Vision = player.Vision;
            player.Attributes.Stability = player.Stability;
            EditorUtility.SetDirty(player);
        }
        AssetDatabase.SaveAssets();
    }

    public void Populate()
    {
        firstNames = new List<string>(namesData.FirstNames);
        lastNames = new List<string>(namesData.LastNames);

        playerList = new List<PlayerData>();
        foreach(PlayerData player in fileList)
        {
            if(!ExcludeList.Contains(player.Team))
            {
                playerList.Add(player);
            }
        }

        foreach(PlayerData player in playerList)
        {
            string firstName = firstNames[Random.Range(0, firstNames.Count)];
            string lastName = lastNames[Random.Range(0, lastNames.Count)];

            player.FirstName = firstName;
            player.LastName = lastName;

            firstNames.Remove(firstName);
            lastNames.Remove(lastName);

            //EditorUtility.SetDirty(player);

            NameGeneratorItem item = Instantiate(itemTemplate, content);
            item.Populate(player, this);
            item.gameObject.SetActive(true);
        }

        //AssetDatabase.SaveAssets();
    }

    public void RemovePlayer(PlayerData _player)
    {
        fileList.Remove(_player);
        Populate();
    }

    void Clear()
    {
        if (itemList == null) itemList = new List<GameObject>();
        foreach (GameObject item in itemList) Destroy(item);
        itemList.Clear();
    }

    void SetPlayersTeam()
    {
        List<TeamData> teams = new List<TeamData>(Resources.LoadAll<TeamData>("Teams"));
        int t = 0;
        int p = 0;
        foreach(TeamData team in teams)
        {
            t++;
            foreach (PlayerData player in team.GetAllPlayers())
            {
                player.Team = team;
                //EditorUtility.SetDirty(player);
                p++;
            }
        }
        //AssetDatabase.SaveAssets();
        print("FINISH UPDATING " + t + " TEAMS AND " + p + " PLAYERS");
    }

    public static T[] GetAtPath<T>(string path)
    {
        ArrayList al = new ArrayList();
        string[] folder = Directory.GetFiles(Application.dataPath + "/" + path);
        foreach (string file in folder)
        {
            string filePath = file.Replace("\\", "/");
            string localPath = "Assets/" + path;
            int index = file.LastIndexOf("/");

            if (index > 0)
            {
                localPath += filePath.Substring(index);
            }
            Object t = AssetDatabase.LoadAssetAtPath(localPath, typeof(T));
            print(localPath);
            if (t != null) al.Add(t);
        }


        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++)
            result[i] = (T)al[i];

        return result;
    }

    public void ResetAllPlayers()
    {

    }
}
#endif
