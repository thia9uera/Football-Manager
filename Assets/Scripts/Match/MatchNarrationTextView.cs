using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using I2.Loc;

public class MatchNarrationTextView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI zone;
    [SerializeField] private Image frame;

	public PlayInfo PlayInfo;

    public void Populate(string _text, Color _frameColor, Color _textColor, string _zone = "")
    {
        label.text = _text;
        label.color = _textColor;
        zone.text = _zone;

	    frame.color = _frameColor;
    }

    public void OnClick()
    {
        if(PlayInfo == null)
        {
            print("PLAY INFO IS NULL");
            return;
        }

        if (PlayInfo.Attacker != null)
        {
            print("ATTACKER: " + PlayInfo.Attacker.FullName);
            print("OFF ACTION: " + PlayInfo.OffensiveAction.ToString());
            print("SUCCESS: " + PlayInfo.IsActionSuccessful);
            print("ATTACKER ROLL: " + PlayInfo.AttackerRoll);
        }
        else
        {
            print("NO ATTACKER");
        }

        print(" ---");

        if (PlayInfo.Defender != null)
        {
	        print("DEFENDER: " + PlayInfo.Defender.FullName);
            print("DEF ACTION: " + PlayInfo.DefensiveAction.ToString());
            print("DEFENSE SUCCESS: " + PlayInfo.IsActionDefended);
            print("DEFENDER ROLL: " + PlayInfo.DefenderRoll);
        }
        else
        {
            print("NO DEFENDER");
        }

        print(" ---");

        print("MARKING TYPE: " + PlayInfo.Marking.ToString());
        print("EVENT: " + PlayInfo.Event.ToString());
	    if (PlayInfo.Assister != null) print("ASSISTER: " + PlayInfo.Assister.FullName);
        print("EXCITMENT: " + PlayInfo.Excitment);
        print("ZONE (ABSOLUTE): " + PlayInfo.Zone.ToString());
        print("TARGET ZONE (ABSOLUTE): " + PlayInfo.TargetZone.ToString());
        print("COUNTER ATTACK: " + PlayInfo.CounterAttack);
    }
}
