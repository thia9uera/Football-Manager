using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CalendarController : MonoBehaviour
{   
	public static CalendarController Instance;
	
	public List<MatchDay> MatchDays;

    public int CurrentDay;
	public int CurrentYear;
    
	private const int TOTAL_DAYS_PER_YEAR = 336;

	private void Awake()
	{
		if(Instance == null) Instance = this;
	}

	private void Start()
    {
	    //int y = int.Parse(DateTime.Now.ToString("yy"));
	    //InitializeCalendar(y);
    }
	
	public void InitializeCalendar()
	{
		UserData userData = MainController.Instance.User;
		
		CurrentDay = userData.CurrentDay;
		CurrentYear = userData.CurrentYear;
		Debug.Log("Current Year: " + CurrentYear);
	    MatchDays = new List<MatchDay>();
	    
	    DateTime date;
	    MatchDay matchDay;
	    
        int month = 1;
	    int day = 1;
	    
        for (int i = 0; i < TOTAL_DAYS_PER_YEAR; ++i)
        {
	        date = new DateTime(CurrentYear, month, day);
	        matchDay = new MatchDay(date);
            MatchDays.Add(matchDay);
            if(day == 28)
            {
                day = 0;
                month++;
            }
	        day++;
        }
        
		foreach(string tournamentId in MainController.Instance.ActiveTournaments)
		{
			TournamentData tournament = MainController.Instance.GetTournamentById(tournamentId);
			foreach(MatchData matchData in tournament.Matches)
			{
				MatchDays[matchData.Day].MatchList.Add(matchData);
			}
		}		
	}
	
	public void UpdateCalendar()
	{
		MatchDay matchDay = MatchDays[CurrentDay];
		MatchData nextMatch = null;
		
		MainController.Instance.User.CurrentDay = CurrentDay;
		
		if(matchDay.HasUnplayedMatches(out nextMatch))
		{
			if(nextMatch.IsUserMatch) 
			{
				DataController.Instance.SaveGame();
				ScreenController.Instance.ShowScreen(ScreenType.Manager);
			}
			else MatchController.Instance.Populate(nextMatch, true);
		}
		else 
		{
			GoToNextBusyDay();
		}
	}
	
	private void GoToNextBusyDay()
	{
		bool finishYear = true;
		MatchData nextMatch;
		for(int i = CurrentDay; i < TOTAL_DAYS_PER_YEAR; i++)
		{
			if(MatchDays[i].HasUnplayedMatches(out nextMatch))
			{
				CurrentDay = i;		
				finishYear = false;
				break;
			}
		}

		if(finishYear) FinishYear();
		else UpdateCalendar();
	}
	
	private void FinishYear()
	{
		//TODO Finish year
		Debug.Log("END OF SEASON");
	}
	
	public MatchData NextUserMatchData
	{
		get 
		{
			MatchDay matchDay = null;
			MatchData nextMatch = null;
			string adversaryId, tournamentName;
			for(int i = CurrentDay; i < TOTAL_DAYS_PER_YEAR; i++)
			{
				matchDay = MatchDays[i];
				if( matchDay.IsUserMatchDay(out adversaryId, out tournamentName, out nextMatch))
				{
					//CurrentDay = i;
					break;
				}
			}
			return nextMatch;
		}
	}
    
	public string GetDay(int _day)
	{
		return MatchDays[_day].Date.ToString("dd MMM yyyy");
	}

    public string GetCurrentDate(string _format)
    {
	    return MatchDays[CurrentDay].Date.ToString(_format);
    }
    
	public DateTime CurrentDate
	{
		get {return MatchDays[CurrentDay].Date;}
	}
	
	public string GetMonthString(int _value)
	{
		string str = "";
		switch(_value)
		{
		case 1	: str = "month_January";	break;
		case 2	: str = "month_February";	break;
		case 3	: str = "month_March";		break;
		case 4	: str = "month_April";		break;
		case 5	: str = "month_May";		break;
		case 6	: str = "month_June";		break;
		case 7	: str = "month_July";		break;
		case 8	: str = "month_August";		break;
		case 9	: str = "month_September";	break;
		case 10 : str = "month_October";	break;
		case 11 : str = "month_November";	break;
		case 12 : str = "month_December";	break;
		}
	    
		return LocalizationController.Instance.Localize(str);
	}
	
    public string GetCurrentMonth()
	{
	    int month =  MatchDays[CurrentDay].Date.Month;
		return GetMonthString(month);	    	    
    }
    
	public string GetCurrentMonthAndYear()
	{
		return GetCurrentMonth() + " " + CurrentYear;
	}

    public string GetCurrentDay()
    {
	    return MatchDays[CurrentYear].Date.ToString("dd");
    }
    
	public string GetWeekDay(int _day)
	{
		int value = _day % 7;
		string str = "Doomsday";
		
		switch(value)
		{
			case 0 : str = "weekday_Monday";	break;
			case 1 : str = "weekday_Tuesday";	break;
			case 2 : str = "weekday_Wednesday";	break;
			case 3 : str = "weekday_Thrusday";	break;
			case 4 : str = "weekday_Friday";	break;
			case 5 : str = "weekday_Saturday";	break;
			case 6 : str = "weekday_Sunday";	break;
		}
		
		return LocalizationController.Instance.Localize(str);
	}
}

[System.Serializable]
public class MatchDay
{
	public DateTime Date;
	public List<MatchData> MatchList;
	public string TournamentName;
	public bool IsUserMatchDay(out string _adversaryId, out string _tournamentName, out MatchData _match)
	{

		bool value = false;
		string userTeamId = MainController.Instance.UserTeam.Id;
		_adversaryId = "";
		_tournamentName = "";
		_match = null;
		foreach(MatchData match in MatchList)
		{
			TournamentName = _tournamentName = match.TournamentName;
			if(match.HomeTeam.TeamId == userTeamId)
			{
				_adversaryId = match.AwayTeam.TeamId;	
				_match = match;
				value = true;
				break;
			}
			else if (match.AwayTeam.TeamId == userTeamId)
			{
				_adversaryId = match.HomeTeam.TeamId;
				_match = match;
				value = true;
				break;
			}
		}
		return value;

	}
	public bool IsEmpty { get { return MatchList.Count == 0; }}
	public bool HasUnplayedMatches(out MatchData _match)
	{
		_match = null;
		if(IsEmpty) return false;
		
		foreach(MatchData match in MatchList)
		{
			if(!match.isPlayed) 
			{
				_match = match;
				return true;
			}
		}
		return false;
	}
	
	public MatchDay(DateTime _date, MatchData _match = null, string _tournamentName = null)
	{
		Date = _date;
		MatchList = new List<MatchData>();
	}
	
}

public enum WeekDay
{
	Monday,
	Tuesday,
	Wednesday,
	Thursday,
	Friday,
	Saturday,
	Sunday
}

public enum Month
{
	January,
	February,
	March,
	April,
	May,
	June, 
	July,
	August,
	Septembet,
	October,
	November,
	Decemember
}


