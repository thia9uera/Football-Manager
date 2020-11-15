using UnityEngine;
using System.IO;

public class DebugTextFile : MonoBehaviour
{
	public static DebugTextFile Instance;
	
	private string str;
	
	private void Awake()
	{
		if(Instance == null) Instance = this;
	}
	
	public void DebugPlayInfo(PlayInfo _playInfo, TeamData _homeTeam, TeamData _awayTeam)
	{
		string fileName = _homeTeam.Name.ToUpper() +"_vs_"+_awayTeam.Name.ToUpper() + ".txt";
		
		if(_playInfo.Turn == 0)
		{
			str = fileName.Replace(".txt", "");
				str += "\n\n";
		}
		
		str += "TURN: " + _playInfo.Turn + " (" + _playInfo.Zone.ToString() + ")";
		if(_playInfo.Attacker != null) 
		{
			str += "\nATTACKER: " + _playInfo.Attacker.FullName + "  (" + _playInfo.Attacker.Team.Name + ")";
			str += "\nROLL: " + _playInfo.AttackerRoll;
			str += "\nATTACKING BONUS: " + _playInfo.AttackingBonus;
			str += "\nACTION: " + _playInfo.OffensiveAction.ToString();
			str += "\nSUCCESSFUL: " + _playInfo.IsActionSuccessful;
		}
		else
		{
			str += "\nNO ATTACKER";
		}
		str += "\n";
		if(_playInfo.Defender != null)
		{
			str += "\nDEFENDER: " + _playInfo.Defender.FullName + "  (" + _playInfo.Defender.Team.Name + ")";
			str += "\nROLL: " + _playInfo.DefenderRoll;
			str += "\nACTION: " + _playInfo.DefensiveAction.ToString();
			str += "\nDEFENDED: " + _playInfo.IsActionDefended;
		}
		else
		{
			str += "\nNO DEFENDER";
		}
		str += "\n";
		if(_playInfo.Assister != null)str += "\nASSISTER: " + _playInfo.Assister.FullName;
		str += "\nMARKING TYPE: " + _playInfo.Marking.ToString();
		str += "\nEVENT: " + _playInfo.Event.ToString();
		str += "\nOFFENSE EXCITMENT: " + _playInfo.OffenseExcitment;
		str += "\nDEFENSE EXCITMENT: " + _playInfo.DefenseExcitment;
		str += "\nTARGET ZONE: " + _playInfo.TargetZone;
		str += "\nCOUNTER ATTACK: " + _playInfo.CounterAttack;
		str += "\n\n--------------------------------------------------------------------------\n\n";
		
		CreateTextFile(str, fileName);
	}
	
	public void CreateTextFile(string _text, string _fileName)
	{
		string folderName = CombinePaths(Application.persistentDataPath, "LogFiles");
		//create Folder
		if (!Directory.Exists (folderName)) {
			Directory.CreateDirectory (folderName);
		}
		 
		File.WriteAllText(CombinePaths(folderName, _fileName), _text);
	}

	private static string CombinePaths(string str_1, string str_2)
    {
        string str = "";

        str = Path.Combine(str_1, str_2);
        str = str.Replace("\\", "/");

        return str;
    }
}
