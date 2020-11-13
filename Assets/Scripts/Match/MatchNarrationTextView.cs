using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using I2.Loc;

public class MatchNarrationTextView : MonoBehaviour
{
	[SerializeField] private TMP_Text label;
    [SerializeField] private Image frame;
	[SerializeField] private Image homeColor;
	[SerializeField] private Image awayColor;

	private PlayInfo playInfo;

	public void Populate(string _text, bool _isAway, PlayInfo _play, Color _homeColor, Color _awayColor)
	{
		playInfo = _play;
        label.text = _text;
	    
	    homeColor.gameObject.SetActive(!_isAway);
		awayColor.gameObject.SetActive(_isAway);
	    
		homeColor.color = _homeColor;
		awayColor.color = _awayColor;
    }
    
	public void Populate(string _text, Color _frameColor)
	{
		label.text = _text;
		frame.color = _frameColor;
	    
		homeColor.gameObject.SetActive(false);
		awayColor.gameObject.SetActive(false);
	}
	
	public void Populate(string _text, Color _frameColor, Color _textColor)
	{
		label.text = _text;
		label.color = _textColor;
		frame.color = _frameColor;
	    
		homeColor.gameObject.SetActive(false);
		awayColor.gameObject.SetActive(false);
	}

    public void OnClick()
    {
        if(playInfo == null)
        {
            print("PLAY INFO IS NULL");
            return;
        }

        if (playInfo.Attacker != null)
        {
            print("ATTACKER: " + playInfo.Attacker.FullName);
            print("OFF ACTION: " + playInfo.OffensiveAction.ToString());
            print("SUCCESS: " + playInfo.IsActionSuccessful);
            print("ATTACKER ROLL: " + playInfo.AttackerRoll);
        }
        else
        {
            print("NO ATTACKER");
        }

        print(" ---");

        if (playInfo.Defender != null)
        {
	        print("DEFENDER: " + playInfo.Defender.FullName);
            print("DEF ACTION: " + playInfo.DefensiveAction.ToString());
            print("DEFENSE SUCCESS: " + playInfo.IsActionDefended);
            print("DEFENDER ROLL: " + playInfo.DefenderRoll);
        }
        else
        {
            print("NO DEFENDER");
        }

        print(" ---");

        print("MARKING TYPE: " + playInfo.Marking.ToString());
        print("EVENT: " + playInfo.Event.ToString());
	    if (playInfo.Assister != null) print("ASSISTER: " + playInfo.Assister.FullName);
        print("EXCITMENT: " + playInfo.Excitment);
        print("ZONE (ABSOLUTE): " + playInfo.Zone.ToString());
        print("TARGET ZONE (ABSOLUTE): " + playInfo.TargetZone.ToString());
        print("COUNTER ATTACK: " + playInfo.CounterAttack);
    }
}
