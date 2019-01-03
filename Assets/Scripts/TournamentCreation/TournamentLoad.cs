using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentLoad : MonoBehaviour
{
    [SerializeField]
    TournamentLoadItem fileTemplate;

    [SerializeField]
    Transform content;

    List<TournamentData> fileList;
    List<GameObject> itemList;

    void Start()
    {
        itemList = new List<GameObject>();
        LoadFiles();
    }


    public void LoadFiles()
    {
        if(itemList.Count > 0)
        {
            foreach(GameObject go in itemList)
            {
                Destroy(go);
            }
            itemList.Clear();
        }

        fileList = new List<TournamentData>(Resources.LoadAll<TournamentData>("Tournaments"));
        foreach (TournamentData file in fileList)
        {
            TournamentLoadItem item = Instantiate(fileTemplate, content);
            item.Populate(file);
            itemList.Add(item.gameObject);
        }
    }
}
