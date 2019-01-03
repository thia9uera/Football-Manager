using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour
{
    public void ClickHandler()
    {
        MainController.Instance.ShowPreviousScreen();
    }
}
