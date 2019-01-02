using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentSelection : MonoBehaviour
{
    [SerializeField]
    TournamentCard cardTemplate;

    [SerializeField]
    Transform content;

    List<GameObject> itemList;
    List<TournamentData> fileList;


    private void Start()
    {
        itemList = new List<GameObject>();
        Populate();
    }

    public void Populate()
    {
        if (itemList.Count > 0)
        {
            foreach (GameObject go in itemList)
            {
                Destroy(go);
            }
            itemList.Clear();
        }

        fileList = new List<TournamentData>(Resources.LoadAll<TournamentData>("Tournaments"));

        foreach (TournamentData file in fileList)
        {
            TournamentCard item = Instantiate(cardTemplate, content);
            item.Populate(file);
            itemList.Add(item.gameObject);
        }
    }
}
