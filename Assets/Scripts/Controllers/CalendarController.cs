using System;
using System.Collections.Generic;
using UnityEngine;

public class CalendarController : MonoBehaviour
{   
	public static CalendarController Instance;
	
	public List<MatchDay> Days;

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
		InitializeCalendar(MainController.Instance.User.CurrentYear);
	}

	public void InitializeCalendar(int _year)
	{
	    CurrentDay = 0;
        CurrentYear = _year;
	    Days = new List<MatchDay>();
	    
	    DateTime date;
	    MatchDay matchDay;
	    
        int month = 1;
	    int day = 1;
	    
        for (int i = 0; i < TOTAL_DAYS_PER_YEAR; ++i)
        {
	        date = new DateTime(_year, month, day);
	        matchDay = new MatchDay(date);
            Days.Add(matchDay);
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
				Days[matchData.Day].MatchList.Add(matchData);
			}
		}
	}
	
	public MatchData NextMatch
	{
		get 
		{
			MatchDay matchDay = null;
			MatchData nextMatch = null;
			string adversaryId, tournamentName;
			for(int i = CurrentDay; i < TOTAL_DAYS_PER_YEAR; i++)
			{
				matchDay = Days[i];
				if( matchDay.IsUserMatchDay(out adversaryId, out tournamentName))
				{
					foreach(MatchData matchData in matchDay.MatchList)
					{
						if(matchData.HomeTeam.TeamId == MainController.Instance.UserTeam.Id || matchData.AwayTeam.TeamId == MainController.Instance.UserTeam.Id)
						{
							nextMatch = matchData;
							break;
						}
					}
					CurrentDay = i;
					break;
				}
			}
			return nextMatch;
		}
	}
    
	public string GetDay(int _day)
	{
		return Days[_day].Date.ToString("dd MMM yyyy");
	}

    public string GetCurrentDate(string _format)
    {
	    return Days[CurrentDay].Date.ToString(_format);
    }
    
	public DateTime CurrentDate
	{
		get {return Days[CurrentDay].Date;}
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
	    int month =  Days[CurrentDay].Date.Month;
		return GetMonthString(month);	    	    
    }
    
	public string GetCurrentMonthAndYear()
	{
		return GetCurrentMonth() + " " + CurrentYear;
	}

    public string GetCurrentDay()
    {
	    return Days[CurrentYear].Date.ToString("dd");
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
	public bool IsUserMatchDay(out string adversaryId, out string tournamentName)
	{

		bool value = false;
		string userTeamId = MainController.Instance.UserTeam.Id;
		adversaryId = "";
		tournamentName = "";
		foreach(MatchData match in MatchList)
		{
			TournamentName = tournamentName = match.TournamentName;
			if(match.HomeTeam.TeamId == userTeamId)
			{
				adversaryId = match.AwayTeam.TeamId;				
				value = true;
				break;
			}
			else if (match.AwayTeam.TeamId == userTeamId)
			{
				adversaryId = match.HomeTeam.TeamId;
				value = true;
				break;
			}
		}
		return value;

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


