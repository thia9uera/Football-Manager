using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFieldView : MonoBehaviour
{
    [SerializeField]
    GameObject[] fieldAreas;

    public void UpdateFieldArea(int _area)
    {
        for(int i = 0; i < 31; i++)
        {
            GameObject obj = fieldAreas[i];

            obj.SetActive(false);

            if (i == _area)
            {             
                obj.SetActive(true);
            }
        }        
    }

}
