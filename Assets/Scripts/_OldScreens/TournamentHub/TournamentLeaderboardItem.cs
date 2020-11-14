using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TournamentLeaderboardItem : MonoBehaviour
{
	[SerializeField] private TMP_Text posLabel = null;
	[SerializeField] private TMP_Text nameLabel = null;
	[SerializeField] private TextMeshProUGUI ptsLabel = null;
	[SerializeField] private Image frameImage  = null;
	[SerializeField] private Color colorGray;

    public void Populate(int _pos, string _name, string _points, bool _isUser)
    {
        posLabel.text = _pos.ToString();
        nameLabel.text = _name;
        ptsLabel.text = _points;

	    //if (_pos % 2 == 0) frameImage.color = GameData.Instance.Colors.LightGray;;

	    if (_isUser) frameImage.color = GameData.Instance.Colors.PlayerColor;
    }
}
