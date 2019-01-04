using System.Collections;
using System.Collections.Generic;
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
        fileList = new List<PlayerData>(Resources.LoadAll<PlayerData>("Players"));
        Populate();
        //SetPlayersTeam();
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

            EditorUtility.SetDirty(player);

            NameGeneratorItem item = Instantiate(itemTemplate, content);
            item.Populate(player, this);
            item.gameObject.SetActive(true);
        }

        AssetDatabase.SaveAssets();
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
            foreach (PlayerData player in team.Squad)
            {
                player.Team = team;
                EditorUtility.SetDirty(player);
                p++;
            }
            foreach (PlayerData player in team.Substitutes)
            {
                player.Team = team;
                EditorUtility.SetDirty(player);
                p++;
            }
        }
        AssetDatabase.SaveAssets();
        print("FINISH UPDATING " + t + " TEAMS AND " + p + " PLAYERS");
    }
}
