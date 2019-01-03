using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TournamentLeaderboardItem : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI posLabel, nameLabel, ptsLabel;

    [SerializeField]
    Image frameImage;

    [SerializeField]
    Color colorGray;

    public void Populate(int _pos, string _name, string _points, bool _isUser)
    {
        posLabel.text = _pos.ToString();
        nameLabel.text = _name;
        ptsLabel.text = _points;

        if (_pos % 2 == 0) frameImage.color = colorGray;

        if (_isUser) frameImage.color = Color.yellow;
    }
}
